# 🚀 HR.Gateway - Ghid Deployment IIS (cu SignalR)

Acest ghid te ajută să faci upgrade de la versiunea veche (doar Concedii Odihna) la versiunea nouă cu:
- ✅ Concedii Odihna, Fără Plată, Evenimente
- ✅ SignalR pentru notificări real-time
- ✅ Retry automat pentru M-Files session expired
- ✅ Logging îmbunătățit

---

## 📋 Pre-requisite pe Server IIS

### 1. **.NET 9 Hosting Bundle** (OBLIGATORIU)
```powershell
# Verifică versiunea instalată
dotnet --list-runtimes

# Dacă nu ai .NET 9, descarcă și instalează:
# https://dotnet.microsoft.com/download/dotnet/9.0
# Alege: ASP.NET Core Runtime 9.0.x - Windows Hosting Bundle
```

**După instalare, RESTART IIS:**
```cmd
net stop was /y
net start w3svc
```

### 2. **WebSocket Protocol** (OBLIGATORIU pentru SignalR)

**Server Manager:**
1. Add Roles and Features
2. Server Roles → Web Server (IIS) → Application Development
3. ✅ Bifează **WebSocket Protocol**
4. Install
5. **RESTART IIS** după instalare

**Verificare în PowerShell:**
```powershell
Get-WindowsFeature Web-WebSockets
# Dacă Install State = Installed → OK
```

---

## 🔧 Partea 1: Pregătire Publish Local

### 1.1. Rulează scriptul de publish

```powershell
cd C:\Users\ViorelBalea\source\repos\viorelbalea\HR.Gateway
.\publish-gateway.ps1
```

Acesta va crea folder-ul: `.\publish\gateway\` cu toate fișierele necesare.

### 1.2. Verifică și editează `appsettings.Production.json`

Deschide: `.\publish\gateway\appsettings.Production.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.AspNetCore.SignalR": "Information",
      "Microsoft.AspNetCore.Http.Connections": "Information"
    }
  },
  "AllowedHosts": "*",

  "EnableHttpsRedirect": true,
  "EnableSwagger": false,

  "ConnectionStrings": {
    "DefaultConnection": "Server=SQL_SERVER;Database=HRGatewayDb;Integrated Security=true;TrustServerCertificate=true;"
  },

  "Jwt": {
    "Key": "SCHIMBA_CU_CHEIE_SECRETA_MINM_32_CARACTERE_PROD",
    "Issuer": "HR.Gateway",
    "Audience": "HR.Gateway.Clients",
    "ExpiryHours": 8
  },

  "MFiles": {
    "ActiveEnvironment": "Production",
    "Environments": {
      "Production": {
        "BaseUrl": "http://mfiles-server-prod/",
        "VaultGuid": "{GUID-VAULT-PRODUCTION}",
        "Username": "mfiles_user",
        "Password": "mfiles_password"
      }
    }
  },

  "LDAP": {
    "Server": "ldap://dc.domeniu.ro",
    "BaseDN": "DC=domeniu,DC=ro",
    "BindDN": "CN=service_account,OU=ServiceAccounts,DC=domeniu,DC=ro",
    "BindPassword": "password_service_account"
  }
}
```

**IMPORTANT:** Înlocuiește valorile cu cele corecte pentru Production!

### 1.3. Verifică `web.config`

Ar trebui să existe deja în `.\publish\gateway\web.config`. Verifică că are:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet"
                  arguments=".\HR.Gateway.Api.dll"
                  stdoutLogEnabled="true"
                  stdoutLogFile=".\logs\stdout"
                  hostingModel="InProcess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
        </environmentVariables>
      </aspNetCore>

      <!-- CRITICAL pentru SignalR -->
      <webSocket enabled="true" />

      <!-- Timeout mai mare pentru SignalR long-polling -->
      <httpProtocol>
        <customHeaders>
          <add name="Access-Control-Allow-Origin" value="*" />
          <add name="Access-Control-Allow-Headers" value="Content-Type, Authorization" />
          <add name="Access-Control-Allow-Methods" value="GET, POST, PUT, DELETE, OPTIONS" />
        </customHeaders>
      </httpProtocol>
    </system.webServer>
  </location>
</configuration>
```

---

## 🚀 Partea 2: Deployment pe Server IIS

### 2.1. Backup versiunea veche

```powershell
# Pe server
cd C:\inetpub\wwwroot\HR.Gateway
mkdir ..\HR.Gateway.backup-$(Get-Date -Format 'yyyyMMdd-HHmmss')
Copy-Item -Path .\* -Destination ..\HR.Gateway.backup-$(Get-Date -Format 'yyyyMMdd-HHmmss') -Recurse
```

### 2.2. Stop Application Pool

```powershell
# În IIS Manager sau PowerShell
Stop-WebAppPool -Name "HR.Gateway"

# SAU în cmd
%windir%\system32\inetsrv\appcmd stop apppool /apppool.name:"HR.Gateway"
```

**Așteaptă 5-10 secunde** pentru ca toate conexiunile să se închidă.

### 2.3. Copiază fișierele noi

**Opțiune A: Manual**
1. Deschide `\\server\c$\inetpub\wwwroot\HR.Gateway`
2. Șterge toate fișierele (EXCEPT `appsettings.Production.json` dacă e deja configurat)
3. Copiază tot din `.\publish\gateway\` pe server

**Opțiune B: PowerShell (dacă ai acces remote)**
```powershell
$serverPath = "\\server\c$\inetpub\wwwroot\HR.Gateway"
$localPath = ".\publish\gateway\*"

# Șterge fișiere vechi (păstrează appsettings.Production.json)
Get-ChildItem -Path $serverPath -Exclude "appsettings.Production.json" | Remove-Item -Recurse -Force

# Copiază noi
Copy-Item -Path $localPath -Destination $serverPath -Recurse -Force
```

### 2.4. Verifică permisiuni folder

Asigură-te că `IIS_IUSRS` și `IIS AppPool\HR.Gateway` au permisiuni:
- **Read & Execute** pe tot folder-ul
- **Write** pe folder-ul `logs\` (pentru logging)

```powershell
icacls "C:\inetpub\wwwroot\HR.Gateway" /grant "IIS_IUSRS:(OI)(CI)RX"
icacls "C:\inetpub\wwwroot\HR.Gateway\logs" /grant "IIS_IUSRS:(OI)(CI)F"
```

### 2.5. Configurare Application Pool pentru SignalR

**IIS Manager:**
1. Application Pools → HR.Gateway → Advanced Settings
2. Setări importante:
   - **.NET CLR Version**: `No Managed Code` (pentru .NET 9)
   - **Managed Pipeline Mode**: `Integrated`
   - **Start Mode**: `AlwaysRunning` (recomandat)
   - **Idle Time-out (minutes)**: `0` sau `20` (pentru SignalR long-lived connections)
   - **Ping Enabled**: `False` (evită timeout-uri SignalR)
   - **Regular Time Interval (minutes)**: `0` (dezactivează recycle automat)

### 2.6. Configurare Site pentru WebSocket

**IIS Manager:**
1. Sites → HR.Gateway → Configuration Editor
2. Section: `system.webServer/webSocket`
3. Setări:
   - `enabled`: `True`
   - `receiveBufferLimit`: `4194304` (4MB - pentru mesaje mari SignalR)
   - `pingInterval`: `00:00:10` (ping la 10 secunde)

### 2.7. Start Application Pool

```powershell
Start-WebAppPool -Name "HR.Gateway"

# SAU în cmd
%windir%\system32\inetsrv\appcmd start apppool /apppool.name:"HR.Gateway"
```

---

## ✅ Partea 3: Testare Deployment

### 3.1. Health Check
```powershell
Invoke-WebRequest -Uri "https://gateway.domeniu.ro/health"
# Expected: Status 200, Body: "Healthy"
```

### 3.2. Verificare SignalR Endpoint
```powershell
Invoke-WebRequest -Uri "https://gateway.domeniu.ro/hubs/cereri/negotiate" -Method POST
# Expected: Status 200, JSON cu connectionId și availableTransports
```

### 3.3. Test Autentificare
```powershell
$body = @{
    Email = "user@domeniu.ro"
    Parola = "password"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "https://gateway.domeniu.ro/api/auth/login" `
    -Method POST `
    -Body $body `
    -ContentType "application/json"

Write-Host "Token: $($response.token)"
```

### 3.4. Verificare Logs

Verifică `C:\inetpub\wwwroot\HR.Gateway\logs\` pentru erori:

```powershell
Get-Content "C:\inetpub\wwwroot\HR.Gateway\logs\stdout_*.log" -Tail 50
```

---

## 🔥 Troubleshooting

### Problema: "HTTP Error 500.31 - Failed to load ASP.NET Core runtime"
**Soluție:**
1. Verifică că .NET 9 Hosting Bundle e instalat
2. Restart IIS: `net stop was /y && net start w3svc`

### Problema: SignalR connection failed
**Soluții:**
1. Verifică că WebSocket Protocol e instalat pe server
2. Verifică `web.config` are `<webSocket enabled="true" />`
3. Verifică Application Pool Idle Timeout nu e prea mic
4. Verifică firewall permite WebSocket (port 443/80)

### Problema: "Access to the path is denied"
**Soluție:**
```powershell
icacls "C:\inetpub\wwwroot\HR.Gateway" /grant "IIS AppPool\HR.Gateway:(OI)(CI)F"
```

### Problema: JWT Authentication failed
**Soluție:**
- Verifică `appsettings.Production.json` → `Jwt:Key` e același în Gateway și Desktop App

### Problema: M-Files connection failed
**Soluție:**
- Verifică `appsettings.Production.json` → `MFiles` settings
- Test conexiune: `curl http://mfiles-server-prod/`

---

## 📝 Checklist Final

- [ ] .NET 9 Hosting Bundle instalat pe server
- [ ] WebSocket Protocol instalat pe server
- [ ] IIS restart după instalare dependencies
- [ ] `appsettings.Production.json` configurat corect
- [ ] `web.config` are `<webSocket enabled="true" />`
- [ ] Application Pool cu `.NET CLR Version = No Managed Code`
- [ ] Application Pool `Idle Time-out = 0` sau `20`
- [ ] Permisiuni `IIS_IUSRS` pe folder Gateway
- [ ] Backup versiune veche făcut
- [ ] Health check `200 OK`
- [ ] SignalR negotiate `200 OK`
- [ ] Test login funcționează
- [ ] Logs nu au erori critice

---

## 🎯 Update Desktop App

După ce Gateway-ul e deployed, update-ează Desktop App cu noul URL:

**appsettings.json:**
```json
{
  "Gateway": {
    "BaseUrl": "https://gateway.domeniu.ro/",
    "SignalRHubUrl": "https://gateway.domeniu.ro/hubs/cereri"
  }
}
```

Rebuild Desktop App și reinstall la utilizatori.

---

**✅ GATA! Aplicația ar trebui să funcționeze cu toate feature-urile noi!**

**Pentru suport:** Verifică logs în `C:\inetpub\wwwroot\HR.Gateway\logs\`
