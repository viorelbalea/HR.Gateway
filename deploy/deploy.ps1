
Param(
  [ValidateSet('simple','traefik')]
  [string]$Mode = 'simple'
)

Push-Location $PSScriptRoot

if (!(Test-Path .env)) {
  Copy-Item .env.template .env
  Write-Host "Created .env from template. Please edit secrets, then re-run." -ForegroundColor Yellow
  Pop-Location
  exit 1
}

if ($Mode -eq 'simple') {
  docker compose -f docker-compose.simple.yml up -d --build
} else {
  docker compose -f docker-compose.traefik.yml up -d --build
}

Pop-Location
