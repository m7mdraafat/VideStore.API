using System.Text.Json.Serialization;

namespace VideStore.Shared.DTOs.Responses.Users
{
    public class AppUserResponse
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string? Role { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsVerified { get; set; } = false;
        public string RefreshTokenExpiration { get; set; } = string.Empty;

    }
}
