using System.ComponentModel.DataAnnotations;
using MembersOnly.Web.Authorization;

namespace MembersOnly.Web.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "First name must be 2 to 20 characters.")]
    [AllowedNameCharacters]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Last name must be 2 to 20 characters.")]
    [AllowedNameCharacters]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    // [Compare] is a built-in attribute that checks the two fields match.
    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}