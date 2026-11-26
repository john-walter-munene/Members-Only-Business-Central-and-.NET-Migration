using Microsoft.AspNetCore.Identity;

namespace MembersOnly.Web.Models;

public class ApplicationUser : IdentityUser
{
    // Store the full name, as in the original app.
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // Link to the Business Central Club Member record.
    public string? ClubMemberNo { get; set; }
}