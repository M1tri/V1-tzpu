using LabZakazivanjeAPI.Clients;
using LabZakazivanjeAPI.Clients.Interfaces;
using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Services;
using LabZakazivanjeAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using LabZakazivanjeAPI.Notifications;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDBContext>(options => 
    options.UseSqlite("Data Source=baza.db"));

builder.Services.AddHostedService<TimeSchedulerService>();
builder.Services.AddHostedService<DashboardStubService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IVLRService, VLRService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IRoomsService, RoomsService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? ""; 
    options.CallbackPath = "/signin-google";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Kljuc"] ?? ""))
    };
});

builder.Services.AddAuthorization(options =>
{
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
        CookieAuthenticationDefaults.AuthenticationScheme,
        JwtBearerDefaults.AuthenticationScheme);
    defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
});;

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified; 
});

builder.Services.AddAuthorization();
builder.Services.AddHttpClient<IInfrastructureClient, InfrastructureClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7213");
});

builder.Services.AddHttpClient<IDashboardClient, DashboardClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7213");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDevPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("ReactDevPolicy");

app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.MapHub<SessionSchedulerNotificationHub>("/schedulerHub");

app.Run();

public partial class Program { }