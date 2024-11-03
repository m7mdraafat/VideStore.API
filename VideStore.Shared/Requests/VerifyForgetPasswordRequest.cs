using System.ComponentModel.DataAnnotations;

namespace VideStore.Shared.Requests
{
    public class VerifyForgetPasswordRequest
    {
        [Required(ErrorMessage = "Verification code is required.")]
        public string VerificationCode { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;
    }
}