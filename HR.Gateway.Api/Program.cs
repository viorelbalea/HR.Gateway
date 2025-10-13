using System.Text;
using HR.Gateway.Application.Abstractions.MFiles;
using HR.Gateway.Application.Abstractions.Security;
using HR.Gateway.Infrastructure;
using HR.Gateway.Infrastructure.Auth;
using HR.Gateway.Infrastructure.MFiles;
using HR.Gateway.Infrastructure.MFiles.Clients;
using HR.Gateway.Infrastructure.MFiles.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 0) Infra (DB + Identity)
builder.Services.AddInfrastructure(builder.Configuration);

// 1) M-Files Options (singura sursă)
builder.Services.Configure<MFilesOptions>(builder.Configuration.GetSection("MFiles"));

// 2) Typed HttpClients (fără IHttpClientFactory in clase)
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

// 3) AD / LDAP (dacă îl folosești)
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

// 5) JWT
var jwtKey = builder.Configuration["JWT:Key"] ?? throw new InvalidOperationException("Missing JWT:Key");
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "HR.Gateway";
var jwtAudience = builder.Configuration["JWT:Audience"] ?? "HR.Gateway.Clients";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// 6) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HR Gateway API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Lipește DOAR tokenul (fără 'Bearer ')."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// log de sanity check din Options
var mfilesOpts = app.Services.GetRequiredService<IOptions<MFilesOptions>>().Value;
var activeEnv  = mfilesOpts.ActiveEnvironment;
var envCfg     = mfilesOpts.Environments[activeEnv];
app.Logger.LogInformation("ENV={Env} | MFiles.BaseUrl={Url} | Vault={Vault}",
    app.Environment.EnvironmentName, envCfg.BaseUrl, envCfg.Credentials?.VaultGuid);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("default");
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
