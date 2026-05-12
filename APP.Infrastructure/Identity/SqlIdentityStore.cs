using APP.Application.Identity;
using APP.Domain.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace APP.Infrastructure.Identity;

public sealed class SqlIdentityStore(IConfiguration configuration) : IIdentityStore
{
    private readonly string connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

    public async Task<AppUser?> FindUserAsync(string userName, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        const string userSql = """
            SELECT UserName, DisplayName, Password
            FROM dbo.AppUsers
            WHERE UserName = @UserName AND IsActive = 1;
            """;

        await using var userCommand = new SqlCommand(userSql, connection);
        userCommand.Parameters.AddWithValue("@UserName", userName);

        await using var reader = await userCommand.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var appUser = new
        {
            UserName = reader.GetString(0),
            DisplayName = reader.GetString(1),
            Password = reader.GetString(2)
        };

        await reader.CloseAsync();

        const string roleSql = """
            SELECT r.Name
            FROM dbo.AppUserRoles ur
            INNER JOIN dbo.AppRoles r ON r.Id = ur.RoleId
            INNER JOIN dbo.AppUsers u ON u.Id = ur.UserId
            WHERE u.UserName = @UserName
            ORDER BY r.Name;
            """;

        await using var roleCommand = new SqlCommand(roleSql, connection);
        roleCommand.Parameters.AddWithValue("@UserName", appUser.UserName);

        var roles = new List<string>();
        await using var roleReader = await roleCommand.ExecuteReaderAsync(cancellationToken);
        while (await roleReader.ReadAsync(cancellationToken))
        {
            roles.Add(roleReader.GetString(0));
        }

        return new AppUser(appUser.UserName, appUser.DisplayName, appUser.Password, roles);
    }

    public async Task<IReadOnlyCollection<AppUser>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            SELECT u.UserName, u.DisplayName, u.Password, r.Name AS RoleName
            FROM dbo.AppUsers u
            LEFT JOIN dbo.AppUserRoles ur ON ur.UserId = u.Id
            LEFT JOIN dbo.AppRoles r ON r.Id = ur.RoleId
            WHERE u.IsActive = 1
            ORDER BY u.DisplayName, r.Name;
            """;

        var users = new Dictionary<string, (string DisplayName, string Password, List<string> Roles)>(StringComparer.OrdinalIgnoreCase);
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var userName = reader.GetString(0);
            if (!users.TryGetValue(userName, out var user))
            {
                user = (reader.GetString(1), reader.GetString(2), []);
                users[userName] = user;
            }

            if (!reader.IsDBNull(3))
            {
                user.Roles.Add(reader.GetString(3));
            }
        }

        return users
            .Select(item => new AppUser(item.Key, item.Value.DisplayName, item.Value.Password, item.Value.Roles))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<AppRole>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            SELECT r.Name, p.Name AS PermissionName
            FROM dbo.AppRoles r
            LEFT JOIN dbo.AppRolePermissions rp ON rp.RoleId = r.Id
            LEFT JOIN dbo.AppPermissions p ON p.Id = rp.PermissionId
            ORDER BY r.Name, p.Name;
            """;

        var rolePermissions = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var roleName = reader.GetString(0);
            if (!rolePermissions.TryGetValue(roleName, out var permissions))
            {
                permissions = [];
                rolePermissions[roleName] = permissions;
            }

            if (!reader.IsDBNull(1))
            {
                permissions.Add(reader.GetString(1));
            }
        }

        return rolePermissions
            .Select(item => new AppRole(item.Key, item.Value.Distinct(StringComparer.OrdinalIgnoreCase).ToArray()))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<AppMenuItem>> GetMenuItemsAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            SELECT [Key], [Text], Controller, [Action], PermissionName, SortOrder, Icon, ParentKey
            FROM dbo.AppMenuItems
            WHERE IsActive = 1
            ORDER BY COALESCE(ParentKey, ''), SortOrder, [Text];
            """;

        var menuItems = new List<AppMenuItem>();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            menuItems.Add(new AppMenuItem(
                reader.GetString(0),
                reader.GetString(1),
                reader.IsDBNull(2) ? null : reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.GetInt32(5),
                reader.IsDBNull(6) ? null : reader.GetString(6),
                reader.IsDBNull(7) ? null : reader.GetString(7)));
        }

        return menuItems;
    }
}
