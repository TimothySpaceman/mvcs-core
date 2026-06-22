namespace Core.Identities;

public record UserIdentity(Guid? Id, string Name, string? Email)
{
    public Guid? Id = Id;
    public string Name = Name;
    public string? Email = Email;

    public bool IsAnonymous => Id is null || Id.Value == Guid.Empty;
}