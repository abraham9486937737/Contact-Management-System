# Multi-stage build for Contact Management System (ASP.NET Core)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ContactManagementAPI/ContactManagementAPI.csproj ContactManagementAPI/
RUN dotnet restore ContactManagementAPI/ContactManagementAPI.csproj

COPY . .
RUN dotnet publish ContactManagementAPI/ContactManagementAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 7860
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DISABLE_HTTPS_REDIRECTION=true

# Persist data across redeploys (requires HF Space persistent storage enabled)
ENV SQLITE_DB_PATH=/data/ContactManagement.db
ENV UPLOADS_ROOT=/data/uploads

RUN mkdir -p /data /data/uploads /data/uploads/photos /data/uploads/documents

ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=${ASPNETCORE_URLS:-http://0.0.0.0:${PORT:-7860}} dotnet ContactManagementAPI.dll"]