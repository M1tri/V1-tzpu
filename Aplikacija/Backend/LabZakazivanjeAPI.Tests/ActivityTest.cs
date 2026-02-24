using Xunit;
using Microsoft.EntityFrameworkCore;
using LabZakazivanjeAPI.Services;
using LabZakazivanjeAPI.Services.Interfaces;
using LabZakazivanjeAPI.Models;
using Moq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Threading.Tasks;
using LabZakazivanjeAPI.Models.DTOs;

namespace Test;

public class ActivityTest
{
    private AppDBContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDBContext(options);
    }

    private void CommonSetup(AppDBContext context, out ActivityClass ac, out Activity a)
    {
        ActivityClass activityClass = new ActivityClass
        {
            Naziv = "AOR-LAB-VEZBE"
        };
        context.ActivityClasses.Add(activityClass);
        context.SaveChanges();

        Activity activity = new Activity
        {
            Name = "AOR-LV1",
            Tip = activityClass,
            VLRIDS = new List<string> { "racunar" }
        };
        context.Activities.Add(activity);
        context.SaveChanges();

        ac = activityClass;
        a = activity;
    }

    [Fact]
    public async Task TestGetActivities()
    {
        var context = GetDbContext();
        var service = new ActivityService(context);

        CommonSetup(context, out ActivityClass ac1, out Activity a1);
        CommonSetup(context, out ActivityClass ac2, out Activity a2);

        var result = await service.GetActivites();

        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.Count());
    }

    [Fact]
    public async Task TestGetActivityWhenExists()
    {
        var context = GetDbContext();
        var service = new ActivityService(context);

        CommonSetup(context, out ActivityClass ac, out Activity a);

        var result = await service.GetActivity(a.Id);

        Assert.True(result.Success);
        Assert.Equal(a.Id, result.Data!.Id);
        Assert.Equal(a.Name, result.Data.Naziv);
        Assert.Equal(ac.Naziv, result.Data.Tip);
    }

    [Fact]
    public async Task TestGetActivityWhenNotExists()
    {
        var context = GetDbContext();
        var service = new ActivityService(context);

        var result = await service.GetActivity(100);

        Assert.False(result.Success);
        Assert.Equal("Ne postoji activity", result.ErrorMessage);
    }

    [Fact]
    public async Task TestAddActivity()
    {
        var context = GetDbContext();
        var service = new ActivityService(context);

        CommonSetup(context, out ActivityClass ac, out Activity a);

        CreateActivityDTO activity = new CreateActivityDTO
        {
            Naziv = "Nova aktivnost",
            ActivityClassId = ac.Id,
            VLRIDs = new List<string> { "vlr1", "vlr2" }
        };

        var result = await service.AddActivity(activity);

        Assert.True(result.Success);
        Assert.Equal(activity.Naziv, result.Data!.Naziv);
        Assert.Equal(ac.Naziv, result.Data.Tip);
        Assert.Equal(activity.VLRIDs, result.Data.VLRIDs);
    }

    [Fact]
    public async Task TestAddActivityWhenInvalidActivityClass()
    {
        var context = GetDbContext();
        var service = new ActivityService(context);

        CreateActivityDTO activity = new CreateActivityDTO
        {
            Naziv = "Nova aktivnost",
            ActivityClassId = 1000,
            VLRIDs = new List<string> { "vlr1", "vlr2" }
        };

        var result = await service.AddActivity(activity);

        Assert.False(result.Success);
        Assert.Equal("Ne postoji ta klasa!", result.ErrorMessage);
    }
}