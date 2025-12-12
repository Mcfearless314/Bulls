# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files 
COPY ["OrderMicroservice/OrderService.Api/OrderService.Api.csproj", "OrderService.Api/"]
COPY ["OrderMicroservice/OrderService.Core/OrderService.Core.csproj", "OrderService.Core/"]
COPY ["OrderMicroservice/OrderService.Application/OrderService.Application.csproj", "OrderService.Application/"]
COPY ["OrderMicroservice/OrderService.Infrastructure/OrderService.Infrastructure.csproj", "OrderService.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "OrderService.Api/OrderService.Api.csproj"

# Copy all source code
COPY OrderMicroservice/ .

# Build and publish
RUN dotnet build "OrderService.Api/OrderService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "OrderService.Api/OrderService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderService.Api.dll"]