using System.ComponentModel.DataAnnotations;

namespace VideStore.Shared.Requests
{
    public record GoogleRequest(
        [Required(ErrorMessage = "Google token is required.")]
        string IdToken
        );
}
