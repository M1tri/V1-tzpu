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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDBContext>(options => 
    options.UseSqlite("Data Source=baza.db"));

builder.Services.AddHostedService<TimeSchedulerService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IVLRService, VLRService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IRoomsService, RoomsService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/api/auth/google-callback";
});

Console.WriteLine("GOOGLE ID: " + builder.Configuration["Authentication:Google:ClientId"]);

builder.Services.AddAuthorization();
builder.Services.AddHttpClient<IInfrastructureClient, InfrastructureClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7213");
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDevPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:3000") // React dev server
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

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.MapHub<SessionSchedulerNotificationHub>("/schedulerHub");

app.Run();
