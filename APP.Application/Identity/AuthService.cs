namespace APP.Application.Identity;

public sealed class AuthService(IIdentityStore identityStore) : IAuthService
{
    public async Task<LoginResult> LoginAsync(
        string userName,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await identityStore.FindUserAsync(userName, cancellationToken);

        if (user is null || !StringComparer.Ordinal.Equals(user.Password, password))
        {
            return LoginResult.Failed("Invalid username or password.");
        }

        var roles = await identityStore.GetRolesAsync(cancellationToken);
        var permissions = roles
            .Where(role => user.Roles.Contains(role.Name, StringComparer.OrdinalIgnoreCase))
            .SelectMany(role => role.Permissions)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return LoginResult.Success(new AuthenticatedUser(
            user.UserName,
            user.DisplayName,
            user.Roles,
            permissions));
    }
}
