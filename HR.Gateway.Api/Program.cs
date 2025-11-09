using System.Text;
using HR.Gateway.Application.Abstractions.MFiles;
using HR.Gateway.Application.Abstractions.Security;
using HR.Gateway.Infrastructure;
using HR.Gateway.Infrastructure.Auth;
using HR.Gateway.Infrastructure.MFiles;
using HR.Gateway.Infrastructure.MFiles.Common;
using HR.Gateway.Infrastructure.MFiles.Tokens;
using HR.Gateway.Infrastructure.Security;          // <-- JwtOptions, JwtTokenFactory
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 0) Infra (DB + Identity)
builder.Services.AddInfrastructure(builder.Configuration);

// 1) M-Files Options (singura sursă)
builder.Services.Configure<MFilesOptions>(builder.Configuration.GetSection("MFiles"));

// 2) Typed HttpClients
builder.Services.AddHttpClient<IMFilesTokenProvider, MFilesTokenProvider>((sp, http) =>
{
    var opts = sp.GetRequiredService<IOptions<MFilesOptions>>().Value;
    var env  = opts.Environments[opts.ActiveEnvironment];
    var baseUrl = env.BaseUrl;
    if (!baseUrl.EndsWith("/")) baseUrl += "/";
    http.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient<IMFilesClient, MFilesClient>((sp, http) =>
{
    var opts = sp.GetRequiredService<IOptions<MFilesOptions>>().Value;
    var env  = opts.Environments[opts.ActiveEnvironment];
    var baseUrl = env.BaseUrl;
    if (!baseUrl.EndsWith("/")) baseUrl += "/";
    http.BaseAddress = new Uri(baseUrl);
});

// 3) AD / LDAP
builder.Services.AddScoped<IAdAuthService, LdapAdAuthService>();

// 4) CORS + Health
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("default", p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});
builder.Services.AddHealthChecks();

// 5) JWT (config + token factory + auth middleware)
//    >>> appsettings: "Jwt": { "Issuer": "...", "Audience": "...", "Key": "...", "ExpiryHours": 8 }
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<ITokenFactory, JwtTokenFactory>();

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey     = jwtSection["Key"]     ?? throw new InvalidOperationException("Missing Jwt:Key");
var jwtIssuer  = jwtSection["Issuer"]  ?? "HR.Gateway";
var jwtAudience= jwtSection["Audience"]?? "HR.Gateway.Clients";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime         = true,
            ClockSkew                = TimeSpan.FromMinutes(2),
            ValidIssuer              = jwtIssuer,
            ValidAudience            = jwtAudience,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// 6) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HR Gateway API", Version = "v1" });
    c.CustomSchemaIds(t => t.FullName?.Replace('+', '.'));
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce tokenul JWT în formatul: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

var enableHttpsRedirect = builder.Configuration.GetValue<bool>("EnableHttpsRedirect", false);
var enableSwagger       = builder.Configuration.GetValue<bool>("EnableSwagger", app.Environment.IsDevelopment());

// ===== Swagger =====
if (enableSwagger)
{
    app.UseDeveloperExceptionPage();   // opțional în prod, dar util la tine acum
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ===== Pipeline =====
if (enableHttpsRedirect)
    app.UseHttpsRedirection();

app.UseCors("default");
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

// ===== UN SINGUR ROOT =====
var root = enableSwagger
    ? app.MapGet("/", () => Results.Redirect("/swagger"))
    : app.MapGet("/", () => Results.Text("HR Gateway API is running"));
root.ExcludeFromDescription();

app.Run();

