using Giromon.Application.Abstractions.Persistence;
using Giromon.Application.Abstractions.Security;
using Giromon.Application.Users.Register;
using Giromon.Domain.Entities;

namespace Giromon.Application.Tests.Users.Register;

public class RegisterUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldRegisterUser()
    {
        var repository = new FakeUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var useCase = new RegisterUserUseCase(
            repository,
            passwordHasher);

        var command = new RegisterUserCommand(
            "Paulo",
            "PAULO@EMAIL.COM",
            "senha123");

        var result = await useCase.ExecuteAsync(command);

        var savedUser = Assert.Single(repository.Users);

        Assert.Equal(savedUser.Id, result.Id);
        Assert.Equal("Paulo", result.Name);
        Assert.Equal("paulo@email.com", result.Email);
        Assert.Equal("hashed:senha123", savedUser.PasswordHash);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        var repository = new FakeUserRepository();
        var passwordHasher = new FakePasswordHasher();

        repository.Users.Add(
            User.Create(
                "Usuário existente",
                "paulo@email.com",
                "existing-hash"));

        var useCase = new RegisterUserUseCase(
            repository,
            passwordHasher);

        var command = new RegisterUserCommand(
            "Paulo",
            "PAULO@EMAIL.COM",
            "senha123");

        await Assert.ThrowsAsync<EmailAlreadyInUseException>(
            () => useCase.ExecuteAsync(command));

        Assert.Single(repository.Users);
        Assert.Equal(0, passwordHasher.HashCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenPasswordIsEmpty()
    {
        var repository = new FakeUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var useCase = new RegisterUserUseCase(
            repository,
            passwordHasher);

        var command = new RegisterUserCommand(
            "Paulo",
            "paulo@email.com",
            " ");

        await Assert.ThrowsAsync<ArgumentException>(
            () => useCase.ExecuteAsync(command));

        Assert.Empty(repository.Users);
        Assert.Equal(0, passwordHasher.HashCallCount);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = [];

        public Task<bool> ExistsByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            var exists = Users.Any(
                user => user.Email.Equals(
                    email,
                    StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(exists);
        }

        public Task AddAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            Users.Add(user);

            return Task.CompletedTask;
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public int HashCallCount { get; private set; }

        public string Hash(string password)
        {
            HashCallCount++;

            return $"hashed:{password}";
        }
    }
}