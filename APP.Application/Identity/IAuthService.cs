namespace APP.Application.Identity;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(string userName, string password, CancellationToken cancellationToken = default);
}
