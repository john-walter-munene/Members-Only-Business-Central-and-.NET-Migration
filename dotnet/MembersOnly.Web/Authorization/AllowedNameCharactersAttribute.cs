using System.ComponentModel.DataAnnotations;

namespace MembersOnly.Web.Authorization;

// Allow letters, spaces, and hyphens only.
public class AllowedNameCharactersAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        // Let [Required] handle empty values.
        if (value is not string text || string.IsNullOrWhiteSpace(text))
        {
            return ValidationResult.Success;
        }

        // Check each character against the allowed set.
        foreach (char c in text)
        {
            if (!char.IsLetter(c) && c != ' ' && c != '-')
            {
                return new ValidationResult(ErrorMessage ?? "Name can only contain letters, spaces, and hyphens.");
            }
        }

        return ValidationResult.Success;
    }
}