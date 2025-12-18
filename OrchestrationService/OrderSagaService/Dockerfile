# Base runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only csproj first to leverage cache
COPY OrchestrationService/OrderSagaService/OrderSagaService.csproj ./OrderSagaService/
RUN dotnet restore "./OrderSagaService/OrderSagaService.csproj"

# Copy all source code
COPY OrchestrationService/OrderSagaService/. ./OrderSagaService/
WORKDIR "./OrderSagaService"

# Build
RUN dotnet build "OrderSagaService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "OrderSagaService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderSagaService.dll"]