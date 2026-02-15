namespace LabZakazivanjeAPI.Models;

using Microsoft.EntityFrameworkCore;

public class AppDBContext : DbContext
{
    public DbSet<Room> Rooms {get; set;}
    public DbSet<Activity> Activities {get; set;}
    public DbSet<Session> Sessions {get; set;}
    public DbSet<ActiveVLR> ActiveVLRs {get; set;}

    public AppDBContext(DbContextOptions<AppDBContext> options)
        : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Session>()
            .Property(r => r.Stanje)
            .HasConversion<string>();

        modelBuilder.Entity<ActiveVLR>()
            .Property(r => r.Status)
            .HasConversion<string>();
    }
}