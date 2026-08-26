namespace Giromon.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private User()
    {
    }

    private User(
        Guid id,
        string name,
        string email,
        string passwordHash,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public static User Create(
        string name,
        string email,
        string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "O nome do usuário é obrigatório.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "O e-mail do usuário é obrigatório.",
                nameof(email));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException(
                "O hash da senha é obrigatório.",
                nameof(passwordHash));
        }

        return new User(
            Guid.NewGuid(),
            name.Trim(),
            email.Trim().ToLowerInvariant(),
            passwordHash,
            DateTime.UtcNow);
    }
}