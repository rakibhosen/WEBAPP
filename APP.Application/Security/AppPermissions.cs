namespace APP.Application.Security;

public static class AppPermissions
{
    public const string DashboardView = "Dashboard.View";
    public const string PrivacyView = "Privacy.View";
    public const string ReportsView = "Reports.View";
    public const string UsersManage = "Users.Manage";

    public static readonly IReadOnlyCollection<string> All =
    [
        DashboardView,
        PrivacyView,
        ReportsView,
        UsersManage
    ];
}
