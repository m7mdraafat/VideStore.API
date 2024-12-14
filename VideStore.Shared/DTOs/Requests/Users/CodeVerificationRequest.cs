using System.ComponentModel.DataAnnotations;
namespace VideStore.Shared.DTOs.Requests.Users

{
    public class CodeVerificationRequest
    {
        [Required(ErrorMessage = "Verification code is required.")]
        public string VerificationCode { get; set; } = null!;
    }
}