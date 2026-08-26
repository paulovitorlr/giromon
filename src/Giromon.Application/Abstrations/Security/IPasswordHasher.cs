namespace Giromon.Application.Abstractions.Security;

public interface IPasswordHasher
{
    string Hash(string password);
}