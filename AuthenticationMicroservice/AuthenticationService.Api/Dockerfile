FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["AuthenticationMicroservice/AuthenticationService.Api/AuthenticationService.Api.csproj", "AuthenticationMicroservice/AuthenticationService.Api/"]
RUN dotnet restore "AuthenticationMicroservice/AuthenticationService.Api/AuthenticationService.Api.csproj"
COPY . .
WORKDIR "/src/AuthenticationMicroservice/AuthenticationService.Api"
RUN dotnet build "./AuthenticationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthenticationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthenticationService.Api.dll"]
