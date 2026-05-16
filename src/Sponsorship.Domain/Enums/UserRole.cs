namespace Sponsorship.Domain.Enums;

/// <summary>Role name constants that mirror the Identity roles seeded at startup.</summary>
public static class Roles
{
    public const string Requestor    = nameof(Requestor);
    public const string Manager      = nameof(Manager);
    public const string FinanceAdmin = nameof(FinanceAdmin);
    public const string SystemAdmin  = nameof(SystemAdmin);
}
