using Giromon.Application.Abstractions.Persistence;
using Giromon.Application.Abstractions.Security;
using Giromon.Application.Users.Register;
using Giromon.Domain.Entities;

namespace Giromon.Application.Tests.Users.Register;

public class RegisterUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldRegisterUserAndCreateWallet()
    {
        var userRepository = new FakeUserRepository();
        var walletRepository = new FakeWalletRepository();
        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();

        var useCase = new RegisterUserUseCase(
            userRepository,
            walletRepository,
            unitOfWork,
            passwordHasher);

        var command = new RegisterUserCommand(
            "Paulo",
            "PAULO@EMAIL.COM",
            "senha123");

        var result = await useCase.ExecuteAsync(command);

        var savedUser = Assert.Single(userRepository.Users);
        var savedWallet = Assert.Single(walletRepository.Wallets);

        Assert.Equal(savedUser.Id, result.Id);
        Assert.Equal("Paulo", result.Name);
        Assert.Equal("paulo@email.com", result.Email);
        Assert.Equal("hashed:senha123", savedUser.PasswordHash);

        Assert.Equal(savedUser.Id, savedWallet.UserId);
        Assert.Equal(0m, savedWallet.Balance);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        var userRepository = new FakeUserRepository();
        var walletRepository = new FakeWalletRepository();
        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();

        userRepository.Users.Add(
            User.Create(
                "Usuário existente",
                "paulo@email.com",
                "existing-hash"));

        var useCase = new RegisterUserUseCase(
            userRepository,
            walletRepository,
            unitOfWork,
            passwordHasher);

        var command = new RegisterUserCommand(
            "Paulo",
            "PAULO@EMAIL.COM",
            "senha123");

        await Assert.ThrowsAsync<EmailAlreadyInUseException>(
            () => useCase.ExecuteAsync(command));

        Assert.Single(userRepository.Users);
        Assert.Empty(walletRepository.Wallets);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
        Assert.Equal(0, passwordHasher.HashCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenPasswordIsEmpty()
    {
        var userRepository = new FakeUserRepository();
        var walletRepository = new FakeWalletRepository();
        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();

        var useCase = new RegisterUserUseCase(
            userRepository,
            walletRepository,
            unitOfWork,
            passwordHasher);

        var command = new RegisterUserCommand(
            "Paulo",
            "paulo@email.com",
            " ");

        await Assert.ThrowsAsync<ArgumentException>(
            () => useCase.ExecuteAsync(command));

        Assert.Empty(userRepository.Users);
        Assert.Empty(walletRepository.Wallets);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
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

        public Task<User?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            var user = Users.SingleOrDefault(
                user => user.Email.Equals(
                    email,
                    StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(user);
        }

        public Task AddAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            Users.Add(user);

            return Task.CompletedTask;
        }
    }

    private sealed class FakeWalletRepository : IWalletRepository
    {
        public List<Wallet> Wallets { get; } = [];

        public Task<Wallet?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var wallet = Wallets.SingleOrDefault(
                wallet => wallet.UserId == userId);

            return Task.FromResult(wallet);
        }

        public Task AddAsync(
            Wallet wallet,
            CancellationToken cancellationToken = default)
        {
            Wallets.Add(wallet);

            return Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;

            return Task.FromResult(1);
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

        public bool Verify(
            string password,
            string passwordHash)
        {
            return passwordHash == $"hashed:{password}";
        }
    }
}