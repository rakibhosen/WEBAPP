namespace APP.Application.Identity;

public sealed record MenuItemDto(
    string Key,
    string Text,
    string? Controller,
    string? Action,
    string Url,
    string? Permission,
    string? Icon,
    IReadOnlyCollection<MenuItemDto> Children);
