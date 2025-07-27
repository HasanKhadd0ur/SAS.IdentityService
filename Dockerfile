# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 7158


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/SAS.IdentityService.API/SAS.IdentityService.API.csproj", "src/SAS.IdentityService.API/"]
COPY ["src/SAS.IdentityService.ApplicationCore/SAS.IdentityService.ApplicationCore.csproj", "src/SAS.IdentityService.ApplicationCore/"]
COPY ["src/SAS.IdentityService.Infrastructure/SAS.IdentityService.Infrastructure.csproj", "src/SAS.IdentityService.Infrastructure/"]
RUN dotnet restore "./src/SAS.IdentityService.API/SAS.IdentityService.API.csproj"
COPY . .
WORKDIR "/src/src/SAS.IdentityService.API"
RUN dotnet build "./SAS.IdentityService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SAS.IdentityService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SAS.IdentityService.API.dll"]