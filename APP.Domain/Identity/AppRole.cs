namespace APP.Domain.Identity;

public sealed record AppRole(
    string Name,
    IReadOnlyCollection<string> Permissions);
