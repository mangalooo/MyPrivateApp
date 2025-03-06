
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components;
using MyPrivateApp.Components.Account;
using MyPrivateApp.Components.Contact.Classes;
using MyPrivateApp.Components.Farming.Classes;
using MyPrivateApp.Components.FrozenFood.Classes;
using MyPrivateApp.Components.Hunting.Classes;
using MyPrivateApp.Components.Shares.Classes;
using MyPrivateApp.Components.ShoppingList.Classes;
using MyPrivateApp.Components.Trip.Classes;
using MyPrivateApp.Data;
using Hangfire;
using Hangfire.SqlServer;
using MyPrivateApp.Components.FarmWork.Classes;
using MyPrivateApp.Components.Games.ManagerZone.Classes;
using MyPrivateApp.Components.Email.Classes;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Builder;
using MyPrivateApp.Data.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddBlazorBootstrap();

// Email
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Private
builder.Services.AddScoped<IShopingListClass, ShopingListClass>();
builder.Services.AddScoped<IContactClass, ContactClass>();
builder.Services.AddScoped<IFrozenFoodClass, FrozenFoodClass>();
builder.Services.AddScoped<ITripClass, NewModulesClass>();
builder.Services.AddScoped<IFarmingClass, FarmingClass>();
builder.Services.AddScoped<IFarmWorkClass, FarmWorkClass>();

// Hunting
builder.Services.AddScoped<IHuntingMyListClass, HuntingMyListClass>();
builder.Services.AddScoped<IHuntingPreyClass, HuntingPreyClass>();
builder.Services.AddScoped<IHuntingTeamMemberClass, HuntingTeamMemberClass>();
builder.Services.AddScoped<IHuntingTowerInspectionClass, HuntingTowerInspectionClass>();

// Games
builder.Services.AddScoped<IMZPurchasedClass, MZPurchasedClass>();
builder.Services.AddScoped<IMZSoldClass, MZSoldClass>();

// Shares
builder.Services.AddScoped<ISharesPurchasedClass, SharesPurchasedClass>();
builder.Services.AddScoped<ISharesSoldClass, SharesSoldClass>();
builder.Services.AddScoped<ISharesPurchasedFundsClass, SharesPurchasedFundsClass>();
builder.Services.AddScoped<ISharesSoldFundsClass, SharesSoldFundsClass>();
builder.Services.AddScoped<ISharesFeeClass, SharesFeeClass>();
builder.Services.AddScoped<ISharesDividendClass, SharesDividendClass>();
builder.Services.AddScoped<ISharesInterestRatesClass, SharesInterestRatesClass>();
builder.Services.AddScoped<ISharesOtherImportsClass, SharesOtherImportsClass>();
builder.Services.AddScoped<ISharesDepositMoneyClass, SharesDepositMoneyClass>();
builder.Services.AddScoped<ISharesImportsFileClass, SharesImportsFileClass>();
builder.Services.AddScoped<ISharesIndexYearsClass, SharesIndexYearsClass>();

// Other
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireRole("Admin"));

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString, x =>
{
    x.CommandTimeout(180);
}), ServiceLifetime.Transient);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Hangfire
builder.Services.AddHangfire(x => x
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();
builder.Services.AddRazorComponents();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();
//.AddAdditionalAssemblies(typeof(Counter).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

//Hangfire
app.UseHangfireDashboard("/hangfire");

// Logs
ILogger<ContactClass> loggerContact = app.Services.GetRequiredService<ILogger<ContactClass>>();
ILogger<FrozenFoodClass> loggerFrozenFood = app.Services.GetRequiredService<ILogger<FrozenFoodClass>>();

// Mapper
IMapper mapper = app.Services.GetRequiredService<IMapper>();
MapperConfiguration config = new(cfg =>
{
    cfg.AddProfile<ContactMappingProfileClass>();
    cfg.AddProfile<FarmingMappingProfileClass>();
    cfg.AddProfile<FarmWorkMappingProfileClass>();
    cfg.AddProfile<FrozenFoodMappingProfileClass>();
    cfg.AddProfile<MZMappingProfileClass>();
});
mapper = config.CreateMapper();

// Email
IConfiguration configuration = app.Services.GetRequiredService<IConfiguration>();
IEmailSender emailSender = app.Services.GetRequiredService<IEmailSender>();

async Task<LastEmailSent?> Get(ApplicationDbContext db, int? id)
{
    if (id == null) throw new ArgumentNullException(nameof(id));

    return await db.LastEmailSent.FirstOrDefaultAsync(r => r.Id == id)
           ?? throw new Exception("Frysvaran hittades inte i databasen!");
}


// Get ApplicationDbContext from the request services
app.Use(async (context, next) =>
{
    ApplicationDbContext db = context.RequestServices.GetRequiredService<ApplicationDbContext>() ??
        throw new InvalidOperationException("Program filen. Felmeddelande: Failed to retrieve ApplicationDbContext from the service provider.");

    LastEmailSent? lastEmailSentBirthday = await Get(db, 1);

    if (lastEmailSentBirthday == null)
        db.LastEmailSent.Add(new LastEmailSent { Time = DateTime.UtcNow });
    else
    {
        if ((DateTime.UtcNow - lastEmailSentBirthday.Time).Hours >= 2)
        {
            // Sends automatic email if a contact has birthday
            IContactClass contactClass = context.RequestServices.GetRequiredService<IContactClass>();
            await contactClass.GetBirthday();
            lastEmailSentBirthday.Time = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
    }

    LastEmailSent? lastEmailSentFrozenFood = await Get(db, 2);

    if (lastEmailSentFrozenFood == null)
        db.LastEmailSent.Add(new LastEmailSent { Time = DateTime.UtcNow });
    else
    {
        if ((DateTime.UtcNow - lastEmailSentFrozenFood.Time).Hours >= 2)
        {
            // Sends automatic email if a the frozen food has past time.
            IFrozenFoodClass frozenFoodClass = context.RequestServices.GetRequiredService<IFrozenFoodClass>();
            frozenFoodClass.GetOutgoingFrosenFood();
            lastEmailSentFrozenFood.Time = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
    }

    await next();
});

app.Run();