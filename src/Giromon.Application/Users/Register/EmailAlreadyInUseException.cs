namespace Giromon.Application.Users.Register;

public sealed class EmailAlreadyInUseException : Exception
{
	public EmailAlreadyInUseException()
		: base("Já existe um usuário cadastrado com esse e-mail.")
	{
	}
}