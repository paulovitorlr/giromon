using Giromon.Application.Abstractions.Persistence;
using Giromon.Application.Abstractions.Security;
using Giromon.Domain.Entities;

namespace Giromon.Application.Users.Register;

public sealed class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
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

        await _userRepository.AddAsync(user, cancellationToken);

        return new RegisterUserResult(
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt);
    }
}