using System.ComponentModel.DataAnnotations;

namespace VideStore.Shared.Requests
{
    public record RegisterRequest(
        [Required(ErrorMessage = "First name is required.")]
        string FirstName,

        [Required(ErrorMessage = "Last name is required.")]
        string LastName,

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        string Email,

        [Required(ErrorMessage = "Phone number is required.")]
        string PhoneNumber,

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        string Password
    );
}