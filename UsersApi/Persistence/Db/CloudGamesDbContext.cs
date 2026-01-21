using FIAP_Cloud_Games.Domain.Entities;
using FIAP_Cloud_Games.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FIAP_Cloud_Games.Infrastructure.Persistence.Data
{
    public class CloudGamesDbContext : DbContext
    {
        public CloudGamesDbContext(DbContextOptions<CloudGamesDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Game> Games => Set<Game>();
        public DbSet<UserGame> UserGames => Set<UserGame>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            // User
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

            // Game
            mb.Entity<Game>(e =>
            {
                e.ToTable("Games");
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).IsRequired().HasMaxLength(200);
                e.Property(x => x.Description).HasMaxLength(2000);
                e.Property(x => x.Genre).IsRequired().HasMaxLength(100);
                e.Property(x => x.Price).HasColumnType("decimal(18,2)");
                e.Property(x => x.Discount).HasColumnType("decimal(5,4)"); // 0..1
                e.Property(x => x.ReleaseDate).HasColumnType("date");
                e.Property(x => x.AddedAtDate).HasColumnType("datetime2");
                // SalePrice é calculado => não mapeia coluna
                e.Ignore(x => x.SalePrice);
            });

            // N:N explícito via UserGame
            mb.Entity<UserGame>(e =>
            {
                e.ToTable("UserGames");
                e.HasKey(x => new { x.UserId, x.GameId });

                e.Property(x => x.AddedAt).HasColumnType("datetime2");

                e.HasOne(x => x.User)
                    .WithMany() // não precisamos expor a coleção de junção no User
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Game)
                    .WithMany() // idem
                    .HasForeignKey(x => x.GameId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // (opcional) índices úteis
            mb.Entity<User>().HasIndex(u => u.Email).IsUnique();
            mb.Entity<Game>().HasIndex(g => g.Title);
        }
    }
}
