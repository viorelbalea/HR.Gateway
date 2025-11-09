# -------- build stage --------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiem doar csproj-urile pentru restore rapid (cache-friendly)
COPY HR.Gateway.sln ./
COPY HR.Gateway.Api/HR.Gateway.Api.csproj HR.Gateway.Api/
COPY HR.Gateway.Application/HR.Gateway.Application.csproj HR.Gateway.Application/
COPY HR.Gateway.Infrastructure/HR.Gateway.Infrastructure.csproj HR.Gateway.Infrastructure/
COPY HR.Gateway.Migrator/HR.Gateway.Migrator.csproj HR.Gateway.Migrator/

RUN dotnet restore HR.Gateway.Api/HR.Gateway.Api.csproj

# Apoi copiem restul surselor și publicăm API-ul
COPY . .
RUN dotnet publish HR.Gateway.Api/HR.Gateway.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# -------- runtime stage --------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HR.Gateway.Api.dll"]
