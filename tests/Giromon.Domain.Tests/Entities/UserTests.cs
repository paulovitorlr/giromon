using Giromon.Domain.Entities;

namespace Giromon.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Create_ShouldCreateUserWithNormalizedData()
    {
        var user = User.Create(
            "  Paulo Vitor  ",
            "  PAULO@EMAIL.COM  ",
            "password-hash");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("Paulo Vitor", user.Name);
        Assert.Equal("paulo@email.com", user.Email);
        Assert.Equal("password-hash", user.PasswordHash);
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrow_WhenNameIsInvalid(string name)
    {
        var action = () => User.Create(
            name,
            "paulo@email.com",
            "password-hash");

        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrow_WhenEmailIsInvalid(string email)
    {
        var action = () => User.Create(
            "Paulo Vitor",
            email,
            "password-hash");

        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrow_WhenPasswordHashIsInvalid(
        string passwordHash)
    {
        var action = () => User.Create(
            "Paulo Vitor",
            "paulo@email.com",
            passwordHash);

        Assert.Throws<ArgumentException>(action);
    }
}