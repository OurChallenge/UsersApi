using FIAP_Cloud_Games.Domain.Exceptions;

namespace FIAP_Cloud_Games.Domain.Entities
{
    /// <summary>
    /// Represents a game.
    /// </summary>
    public class Game
    {
        // Properties
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Genre { get; private set; }
        public DateTime ReleaseDate { get; private set; }
        public DateTime AddedAtDate { get; private set; }
        public decimal Price { get; private set; }
        public decimal Discount { get; private set; }
        public decimal SalePrice => Price - (Price * Discount);

        // Constructor required by EF Core (reflection)
        private Game() { }

        // Constructor used when creating new instances
        public Game(string title, string description, string genre, decimal price, DateTime releaseDate, decimal discount = 0)
        {
            UpdateTitle(title);
            UpdateDescription(description);
            UpdateGenre(genre);
            UpdatePrice(price);
            SetDiscount(discount);
            SetReleaseDate(releaseDate);

            Id = Guid.NewGuid();
            AddedAtDate = DateTime.UtcNow;
        }

        // Guard Methods
        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Invalid title.");
            Title = title;
        }

        public void UpdateDescription(string description)
        {
            Description = description ?? string.Empty;
        }

        public void UpdateGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre))
                throw new DomainException("Invalid genre.");
            Genre = genre;
        }

        public void UpdatePrice(decimal price)
        {
            if (price < 0)
                throw new DomainException("Price cannot be negative.");
            Price = price;
        }

        public void SetDiscount(decimal discount)
        {
            if (discount < 0 || discount > 1)
                throw new DomainException("Discount must be between 0 and 1.");
            Discount = discount;
        }
        public void SetReleaseDate(DateTime releaseDate)
        {
            if (releaseDate == default)
                throw new DomainException("Release date is required.");
            ReleaseDate = releaseDate;
        }
    }
}
