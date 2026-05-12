namespace APP.WEB.Models;

public sealed record UserAccessViewModel(
    IReadOnlyCollection<UserAccessUserViewModel> Users,
    int ActiveOperatorCount,
    int PrivilegedRoleCount,
    string AccessPolicy);

public sealed record UserAccessUserViewModel(
    string DisplayName,
    string UserName,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);
