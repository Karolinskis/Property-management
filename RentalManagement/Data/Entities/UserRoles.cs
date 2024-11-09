namespace RentalManagement.Entities;

public class UserRoles
{
    public const string Tenant = nameof(Tenant);
    public const string Owner = nameof(Owner);
    public const string Admin = nameof(Admin);

    public static readonly IReadOnlyCollection<string> All = new[] { Tenant, Owner, Admin };
}
