using Giromon.Application.Abstractions.Persistence;
using Giromon.Application.Abstractions.Security;

namespace Giromon.Application.Users.Login;

public sealed class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccessTokenGenerator _accessTokenGenerator;

    public LoginUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAccessTokenGenerator accessTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _accessTokenGenerator = accessTokenGenerator;
    }

    public async Task<LoginResult> ExecuteAsync(
        LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Email) ||
            string.IsNullOrWhiteSpace(command.Password))
        {
            throw new InvalidCredentialsException();
        }

        var normalizedEmail =
            command.Email.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (user is null ||
            !_passwordHasher.Verify(
                command.Password,
                user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        var accessToken = _accessTokenGenerator.Generate(user);

        return new LoginResult(
            user.Id,
            user.Name,
            user.Email,
            accessToken);
    }
}