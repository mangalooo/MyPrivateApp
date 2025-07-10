
using AutoMapper;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components;
using MyPrivateApp.Components.Account;
using MyPrivateApp.Components.Contact.Classes;
using MyPrivateApp.Components.Email.Classes;
using MyPrivateApp.Components.Farming.Classes;
using MyPrivateApp.Components.FarmWork.Classes;
using MyPrivateApp.Components.FrozenFood.Classes;
using MyPrivateApp.Components.Games.ManagerZone.Classes;
using MyPrivateApp.Components.Hunting.Classes;
using MyPrivateApp.Components.Layout.Classes;
using MyPrivateApp.Components.Shares.Classes;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ShoppingList.Classes;
using MyPrivateApp.Components.Trip.Classes;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models;
using MyPrivateApp.Data.Models.Hunting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Example for ASP.NET Core (Startup.cs or Program.cs)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict; // Or Lax/None as needed
    options.Secure = CookieSecurePolicy.Always;
});

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
builder.Services.AddScoped<ITripClass, TripClass>();

// Farm work
builder.Services.AddScoped<IFarmingClass, FarmingClass>();
builder.Services.AddScoped<IFarmWorksPlanningClass, FarmWorksPlanningClass>();


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
builder.Services.AddScoped<ISharesErrorHandlingClass, SharesErrorHandlingClass>();

// Other
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DetailedErrors = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Enforces Secure attribute
    options.Cookie.SameSite = SameSiteMode.Strict; // Ensures cookies are sent only in same-site requests
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireRole("Admin"));

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

builder.WebHost.UseWebRoot("wwwroot");
builder.WebHost.UseStaticWebAssets();

WebApplication app = builder.Build();

// Map static assets
app.MapStaticAssets();

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

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

//Hangfire
app.UseHangfireDashboard("/hangfire");

// Logs
ILogger<ContactClass> loggerContact = app.Services.GetRequiredService<ILogger<ContactClass>>();
ILogger<FarmingClass> loggerFarmingClass = app.Services.GetRequiredService<ILogger<FarmingClass>>();
ILogger<FarmWorkClass> loggerFarmWorkClass = app.Services.GetRequiredService<ILogger<FarmWorkClass>>();
ILogger<FarmWorksPlanningClass> loggerFarmWorkPlanningClass = app.Services.GetRequiredService<ILogger<FarmWorksPlanningClass>>();
ILogger<MZPurchasedClass> loggerMZPurchasedClass = app.Services.GetRequiredService<ILogger<MZPurchasedClass>>();
ILogger<MZSoldClass> loggerMZSoldClass = app.Services.GetRequiredService<ILogger<MZSoldClass>>();
ILogger<FrozenFoodClass> loggerFrozenFood = app.Services.GetRequiredService<ILogger<FrozenFoodClass>>();
ILogger<HuntingMyList> loggerHuntingMyList = app.Services.GetRequiredService<ILogger<HuntingMyList>>();
ILogger<ShopingList> loggerShopingList = app.Services.GetRequiredService<ILogger<ShopingList>>();
ILogger<Trips> loggerTrips = app.Services.GetRequiredService<ILogger<Trips>>();

// Shares logs
ILogger<SharesDepositMoneyClass> loggerDepositMoney = app.Services.GetRequiredService<ILogger<SharesDepositMoneyClass>>();
ILogger<SharesDividendClass> loggerDividend = app.Services.GetRequiredService<ILogger<SharesDividendClass>>();
ILogger<SharesPurchasedClass> loggerPurchased = app.Services.GetRequiredService<ILogger<SharesPurchasedClass>>();
ILogger<SharesSoldClass> loggerSold = app.Services.GetRequiredService<ILogger<SharesSoldClass>>();
ILogger<SharesPurchasedFundsClass> LoggerPurchasedFunds = app.Services.GetRequiredService<ILogger<SharesPurchasedFundsClass>>();
ILogger<SharesSoldFundsClass> loggerSoldFunds = app.Services.GetRequiredService<ILogger<SharesSoldFundsClass>>();
ILogger<SharesFeeClass> loggerFee = app.Services.GetRequiredService<ILogger<SharesFeeClass>>();
ILogger<SharesInterestRatesClass> loggerInterestRates = app.Services.GetRequiredService<ILogger<SharesInterestRatesClass>>();
ILogger<SharesOtherImportsClass> loggerOtherImports = app.Services.GetRequiredService<ILogger<SharesOtherImportsClass>>();
ILogger<SharesImportsFileClass> loggerImportsFile = app.Services.GetRequiredService<ILogger<SharesImportsFileClass>>();
ILogger<SharesIndexYearsClass> loggerIndexYears = app.Services.GetRequiredService<ILogger<SharesIndexYearsClass>>();

// Mapper
IMapper mapper = app.Services.GetRequiredService<IMapper>();
MapperConfiguration config = new(cfg =>
{
    cfg.AddProfile<ContactMappingProfileClass>();
    cfg.AddProfile<FarmingMappingProfileClass>();
    cfg.AddProfile<FarmWorkMappingProfileClass>();
    cfg.AddProfile<MZMappingProfileClass>();
    cfg.AddProfile<FrozenMappingProfileClass>();
    cfg.AddProfile<HuntingMappingProfileClass>();
    cfg.AddProfile<ShopingListProfileClass>();
    cfg.AddProfile<TripMappingProfileClass>();
    cfg.AddProfile<SharesMappingProfileClass>();
});
mapper = config.CreateMapper();

// Email
IConfiguration configuration = app.Services.GetRequiredService<IConfiguration>();
IEmailSender emailSender = app.Services.GetRequiredService<IEmailSender>();

async Task<LastEmailSent?> Get(ApplicationDbContext db, int? id)
{
    if (id == null) 
        throw new ArgumentNullException(nameof(id));

    return await db.LastEmailSent.FirstOrDefaultAsync(r => r.Id == id)
           ?? throw new Exception("Datum för mejl-utskick hittades inte i databasen!");
}

// Get ApplicationDbContext from the request services
app.Use(async (context, next) =>
{
    ApplicationDbContext db = context.RequestServices.GetRequiredService<ApplicationDbContext>() 
        ?? throw new InvalidOperationException("Program felmeddelande: Gick inte att koppla till databasen!");

    LastEmailSent? lastEmailSentBirthday = await Get(db, 1);

    if (lastEmailSentBirthday == null)
        db.LastEmailSent.Add(new LastEmailSent { Time = DateTime.UtcNow });
    else
    {
        if ((DateTime.UtcNow - lastEmailSentBirthday.Time).Hours >= 5)
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
        if ((DateTime.UtcNow - lastEmailSentFrozenFood.Time).Hours >= 6)
        {
            // Fix for CS4014: Await the call to GetOutgoingFrosenFood to ensure proper execution order.
            IFrozenFoodClass frozenFoodClass = context.RequestServices.GetRequiredService<IFrozenFoodClass>();
            await frozenFoodClass.GetOutgoingFrosenFood();
            lastEmailSentFrozenFood.Time = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
    }

    await next();
});

app.UseHsts();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exceptionHandlerPathFeature?.Error, "Unhandled exception");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An error occurred.");
    });
});

app.Run();