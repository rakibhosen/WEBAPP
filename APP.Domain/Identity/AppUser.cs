namespace APP.Domain.Identity;

public sealed record AppUser(
    string UserName,
    string DisplayName,
    string Password,
    IReadOnlyCollection<string> Roles);
