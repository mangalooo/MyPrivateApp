# Använd .NET 10 SDK för att bygga båda projekten
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["MyPrivateApp/MyPrivateApp.csproj", "MyPrivateApp/"]
RUN dotnet restore "MyPrivateApp/MyPrivateApp.csproj"

COPY . .
WORKDIR "/src/MyPrivateApp"
RUN dotnet publish "MyPrivateApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "MyPrivateApp.dll"]