# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files 
COPY ["PaymentMicroservice/PaymentService.Api/PaymentService.Api.csproj", "PaymentService.Api/"]
COPY ["PaymentMicroservice/PaymentService.Core/PaymentService.Core.csproj", "PaymentService.Core/"]
COPY ["PaymentMicroservice/PaymentService.Application/PaymentService.Application.csproj", "PaymentService.Application/"]
COPY ["PaymentMicroservice/PaymentService.Infrastructure/PaymentService.Infrastructure.csproj", "PaymentService.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "PaymentService.Api/PaymentService.Api.csproj"

# Copy all source code
COPY PaymentMicroservice/ .

# Build and publish
RUN dotnet build "PaymentService.Api/PaymentService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "PaymentService.Api/PaymentService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.Api.dll"]