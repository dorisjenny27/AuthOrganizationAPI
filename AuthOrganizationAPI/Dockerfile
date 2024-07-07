# Use the appropriate base images for ASP.NET Core
# Update to the latest versions if necessary
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK image for building and restoring packages
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AuthOrganizationAPI/AuthOrganizationAPI.csproj", "AuthOrganizationAPI/"]
RUN dotnet restore "AuthOrganizationAPI/AuthOrganizationAPI.csproj"
COPY . .
WORKDIR "/src/AuthOrganizationAPI"
RUN dotnet build "AuthOrganizationAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "AuthOrganizationAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage/image for running the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthOrganizationAPI.dll"]