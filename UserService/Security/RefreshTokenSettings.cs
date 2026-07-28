namespace UserService.Security;

public class RefreshTokenSettings
{
    public const string SectionName = "RefreshToken";

    public int ExpiryDays { get; set; } = 30;
}
