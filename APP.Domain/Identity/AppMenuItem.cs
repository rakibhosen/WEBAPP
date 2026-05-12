namespace APP.Domain.Identity;

public sealed record AppMenuItem(
    string Key,
    string Text,
    string? Controller,
    string? Action,
    string? Permission,
    int SortOrder,
    string? Icon = null,
    string? ParentKey = null);
