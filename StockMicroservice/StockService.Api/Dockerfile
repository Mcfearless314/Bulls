# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files 
COPY ["StockMicroservice/StockService.Api/StockService.Api.csproj", "StockService.Api/"]
COPY ["StockMicroservice/StockService.Core/StockService.Core.csproj", "StockService.Core/"]
COPY ["StockMicroservice/StockService.Application/StockService.Application.csproj", "StockService.Application/"]
COPY ["StockMicroservice/StockService.Infrastructure/StockService.Infrastructure.csproj", "StockService.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "StockService.Api/StockService.Api.csproj"

# Copy all source code
COPY StockMicroservice/ .

# Build and publish
RUN dotnet build "StockService.Api/StockService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "StockService.Api/StockService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StockService.Api.dll"]