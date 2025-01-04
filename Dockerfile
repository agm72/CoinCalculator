# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Install Node.js (necessary for React build)
RUN apt-get update && apt-get install -y \
    curl \
    && curl -fsSL https://deb.nodesource.com/setup_20.x | sudo bash - \
    && apt-get install -y nodejs \
    && apt-get clean

# Copy necessary files for both the React and .NET projects
COPY ["coinscalculator.client/nuget.config", "coinscalculator.client/"]
COPY ["CoinsCalculator.Server/CoinsCalculator.Server.csproj", "CoinsCalculator.Server/"]
COPY ["coinscalculator.client/coinscalculator.client.esproj", "coinscalculator.client/"]

# Restore .NET dependencies
RUN dotnet restore "./CoinsCalculator.Server/CoinsCalculator.Server.csproj"

# Copy the rest of the application code
COPY . .

# Build the .NET application
WORKDIR "/src/CoinsCalculator.Server"
RUN dotnet build "./CoinsCalculator.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CoinsCalculator.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoinsCalculator.Server.dll"]
