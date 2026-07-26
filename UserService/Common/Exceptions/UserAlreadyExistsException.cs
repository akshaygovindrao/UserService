namespace UserService.Common.Exceptions;

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string email)
        : base($"A user with email '{email}' already exists.")
    {
    }
}
