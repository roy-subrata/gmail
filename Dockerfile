# ── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY GmailApp.csproj .
RUN dotnet restore GmailApp.csproj

COPY . .
RUN dotnet publish GmailApp.csproj -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# curl is needed for the HEALTHCHECK below
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app/publish .

# Directory for the SQLite database (mount a volume here to persist data)
# Owned by the non-root 'app' user that the aspnet base image provides
RUN mkdir -p /data && chown app:app /data

USER app

EXPOSE 8080

# aspnet:10.0 already sets ASPNETCORE_HTTP_PORTS=8080; point DB at the volume
ENV ConnectionStrings__DefaultConnection="Data Source=/data/gmail.db"

HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "GmailApp.dll"]
