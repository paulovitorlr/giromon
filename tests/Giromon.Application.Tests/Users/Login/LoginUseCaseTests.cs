using Giromon.Application.Abstractions.Persistence;
using Giromon.Application.Abstractions.Security;
using Giromon.Application.Users.Login;
using Giromon.Domain.Entities;

namespace Giromon.Application.Tests.Users.Login;

public class LoginUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var user = User.Create(
            "Paulo",
            "paulo@email.com",
            "hashed:senha123");

        var useCase = new LoginUseCase(
            new FakeUserRepository(user),
            new FakePasswordHasher(),
            new FakeAccessTokenGenerator());

        var result = await useCase.ExecuteAsync(
            new LoginCommand(
                " PAULO@EMAIL.COM ",
                "senha123"));

        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal($"token:{user.Id}", result.AccessToken);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUserDoesNotExist()
    {
        var useCase = new LoginUseCase(
            new FakeUserRepository(),
            new FakePasswordHasher(),
            new FakeAccessTokenGenerator());

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => useCase.ExecuteAsync(
                new LoginCommand(
                    "inexistente@email.com",
                    "senha123")));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenPasswordIsInvalid()
    {
        var user = User.Create(
            "Paulo",
            "paulo@email.com",
            "hashed:senha123");

        var useCase = new LoginUseCase(
            new FakeUserRepository(user),
            new FakePasswordHasher(),
            new FakeAccessTokenGenerator());

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => useCase.ExecuteAsync(
                new LoginCommand(
                    "paulo@email.com",
                    "senha-incorreta")));
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly User? _user;

        public FakeUserRepository(User? user = null)
        {
            _user = user;
        }

        public Task<bool> ExistsByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_user?.Email == email);
        }

        public Task<User?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            var user = _user?.Email == email
                ? _user
                : null;

            return Task.FromResult(user);
        }

        public Task AddAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return $"hashed:{password}";
        }

        public bool Verify(
            string password,
            string passwordHash)
        {
            return passwordHash == $"hashed:{password}";
        }
    }

    private sealed class FakeAccessTokenGenerator
        : IAccessTokenGenerator
    {
        public string Generate(User user)
        {
            return $"token:{user.Id}";
        }
    }
}