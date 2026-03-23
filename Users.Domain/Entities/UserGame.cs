namespace Users.Domain.Entities;

public class UserGame
{
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = default!;
    public Game Game { get; set; } = default!;
}
