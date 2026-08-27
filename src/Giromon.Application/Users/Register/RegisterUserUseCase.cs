using Giromon.Application.Abstractions.Persistence;
using Giromon.Application.Abstractions.Security;
using Giromon.Domain.Entities;

namespace Giromon.Application.Users.Register;

public sealed class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserUseCase(
        IUserRepository userRepository,
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserResult> ExecuteAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Password))
        {
            throw new ArgumentException(
                "A senha do usuário é obrigatória.",
                nameof(command.Password));
        }

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        var emailAlreadyExists =
            await _userRepository.ExistsByEmailAsync(
                normalizedEmail,
                cancellationToken);

        if (emailAlreadyExists)
        {
            throw new EmailAlreadyInUseException();
        }

        var passwordHash = _passwordHasher.Hash(command.Password);

        var user = User.Create(
            command.Name,
            normalizedEmail,
            passwordHash);

        var wallet = Wallet.Create(user.Id);

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _walletRepository.AddAsync(
            wallet,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterUserResult(
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt);
    }
}