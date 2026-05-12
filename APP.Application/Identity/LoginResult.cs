namespace APP.Application.Identity;

public sealed record LoginResult(
    bool Succeeded,
    string? ErrorMessage,
    AuthenticatedUser? User)
{
    public static LoginResult Failed(string errorMessage)
    {
        return new LoginResult(false, errorMessage, null);
    }

    public static LoginResult Success(AuthenticatedUser user)
    {
        return new LoginResult(true, null, user);
    }
}

public sealed record AuthenticatedUser(
    string UserName,
    string DisplayName,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);
