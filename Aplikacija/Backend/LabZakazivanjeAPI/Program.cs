using LabZakazivanjeAPI.Clients;
using LabZakazivanjeAPI.Clients.Interfaces;
using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Services;
using LabZakazivanjeAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddHttpClient<IInfrastructureClient, InfrastructureClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7213");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
