using System.ComponentModel.DataAnnotations;

namespace VideStore.Shared.Requests.Users
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Old password is required.")]
        public string? OldPassword { get; set; } = null;

        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Confirmation password is required.")]
        [Compare("NewPassword", ErrorMessage = "The confirmation password does not match the new password.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}