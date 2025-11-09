# --- build ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY HR.Gateway.sln ./
COPY HR.Gateway.Api/HR.Gateway.Api.csproj HR.Gateway.Api/
COPY HR.Gateway.Application/HR.Gateway.Application.csproj HR.Gateway.Application/
COPY HR.Gateway.Infrastructure/HR.Gateway.Infrastructure.csproj HR.Gateway.Infrastructure/
COPY HR.Gateway.Migrator/HR.Gateway.Migrator.csproj HR.Gateway.Migrator/
RUN dotnet restore HR.Gateway.Migrator/HR.Gateway.Migrator.csproj
COPY . .
RUN dotnet publish HR.Gateway.Migrator/HR.Gateway.Migrator.csproj -c Release -o /app/publish /p:UseAppHost=false

# --- runtime (console) ---
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "HR.Gateway.Migrator.dll"]
