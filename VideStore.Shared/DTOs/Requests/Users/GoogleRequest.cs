using System.ComponentModel.DataAnnotations;

namespace VideStore.Shared.DTOs.Requests.Users

{
    public record GoogleRequest(
        [Required(ErrorMessage = "Google token is required.")]
        string IdToken
        );
}
