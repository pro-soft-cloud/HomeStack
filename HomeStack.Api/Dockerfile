ARG CACHEBUSTER=0

# Chiseled: kein Shell, kein Paketmanager, minimaler Footprint & Angriffsfläche
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled-extra AS base
WORKDIR /app
EXPOSE 8080
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HomeStack.Api/HomeStack.Api.csproj", "HomeStack.Api/"]
RUN dotnet restore "./HomeStack.Api/HomeStack.Api.csproj"
COPY . .
WORKDIR "/src/HomeStack.Api"
#RUN dotnet build "./HomeStack.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore
RUN dotnet build "./HomeStack.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HomeStack.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish \
    --no-restore \
    /p:UseAppHost=false \
    /p:DebugType=none

FROM base AS final

WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_gcServer=0 \
    DOTNET_gcConcurrent=true \
    DOTNET_GCHeapHardLimitMB=256 \
    DOTNET_EnableDiagnostics=0

COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .

HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD ["dotnet", "HomeStack.Api.dll", "--healthcheck"]

ENTRYPOINT ["dotnet", "HomeStack.Api.dll"]