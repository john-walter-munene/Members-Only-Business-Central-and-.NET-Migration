namespace MembersOnly.Web.Services;

public interface IBusinessCentralClient
{
    // Create a Pending member in Business Central, and return its Member No.
    Task<string> RegisterMemberAsync(string identityUserId, string email, string firstName, string lastName);
}