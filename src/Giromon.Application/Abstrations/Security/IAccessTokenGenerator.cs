using Giromon.Domain.Entities;

namespace Giromon.Application.Abstractions.Security;

public interface IAccessTokenGenerator
{
    string Generate(User user);
}