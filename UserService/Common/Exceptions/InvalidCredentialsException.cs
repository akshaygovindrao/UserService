namespace UserService.Common.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("The email or password is incorrect.")
    {
    }
}
