using Microsoft.AspNetCore.Authorization;

namespace APP.WEB.Authorization;

public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
