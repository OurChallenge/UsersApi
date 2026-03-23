using System.Text.Json;
using System.Text.RegularExpressions;
using Users.Domain.Enums;
using Users.Domain.Exceptions;

namespace Users.Domain.Entities;

public class User
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public string Email { get; private set; } = default!;
    public string Password { get; private set; } = default!;
    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public List<Game> Library { get; private set; } = [];

    private User()
    {
    }

    public User(string name, string email, string password)
    {
        UpdateName(name);
        UpdateEmail(email);
        ChangePassword(password);
        AssignRole("User");

        Id = Guid.NewGuid();
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Invalid name.");

        Name = name;
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
            throw new DomainException("Invalid email.");

        Email = email;
    }

    public void ChangePassword(string password)
    {
        password ??= string.Empty;

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
            errors.Add("Password cannot be empty.");

        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters long.");

        if (!Regex.IsMatch(password, "[0-9]"))
            errors.Add("Password must contain at least one number.");

        if (!Regex.IsMatch(password, "[a-zA-Z]"))
            errors.Add("Password must contain at least one letter.");

        if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>_\-+=\\/\[\]~]"))
            errors.Add("Password must contain at least one special character.");

        if (errors.Any())
            throw new DomainException(JsonSerializer.Serialize(errors));

        Password = password;
    }

    public void AssignRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new DomainException("Role não pode ser nula ou vazia.", nameof(role));

        if (Enum.TryParse<UserRole>(role.Trim(), true, out var parsedRole))
        {
            Role = parsedRole;
            return;
        }

        throw new DomainException($"Role inválida: '{role}'", nameof(role));
    }

    public void AcquireGame(Game game)
    {
        if (game == null)
            throw new DomainException("Objeto nulo para classe game.");

        if (!Library.Any(g => g.Id == game.Id))
            Library.Add(game);
    }
}
