using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Domain.Enums;

namespace Users.Infrastructure.Persistence.Db;

public class CloudGamesDbContext : DbContext
{
    public CloudGamesDbContext(DbContextOptions<CloudGamesDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<UserGame> UserGames => Set<UserGame>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Role).HasConversion(
                x => x.ToString(),
                x => Enum.Parse<UserRole>(x));
            e.Property(x => x.Email).IsRequired().HasMaxLength(320);
            e.Property(x => x.Password).IsRequired().HasMaxLength(200);
            e.Property(x => x.CreatedDate).HasColumnType("datetime2");
        });

        mb.Entity<Game>(e =>
        {
            e.ToTable("Games");
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).HasMaxLength(2000);
            e.Property(x => x.Genre).IsRequired().HasMaxLength(100);
            e.Property(x => x.Price).HasColumnType("decimal(18,2)");
            e.Property(x => x.Discount).HasColumnType("decimal(5,4)");
            e.Property(x => x.ReleaseDate).HasColumnType("date");
            e.Property(x => x.AddedAtDate).HasColumnType("datetime2");
            e.Ignore(x => x.SalePrice);
        });

        mb.Entity<UserGame>(e =>
        {
            e.ToTable("UserGames");
            e.HasKey(x => new { x.UserId, x.GameId });
            e.Property(x => x.AddedAt).HasColumnType("datetime2");

            e.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Game)
                .WithMany()
                .HasForeignKey(x => x.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<User>().HasIndex(u => u.Email).IsUnique();
        mb.Entity<Game>().HasIndex(g => g.Title);
    }
}
