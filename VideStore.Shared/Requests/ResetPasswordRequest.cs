using System.ComponentModel.DataAnnotations;

namespace VideStore.Shared.Requests
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Confirmation password is required.")]
        [Compare("NewPassword", ErrorMessage = "The confirmation password does not match the new password.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}