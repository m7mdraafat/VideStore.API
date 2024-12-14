
namespace VideStore.Shared.DTOs.Responses.Users
{
    public class UserAddressDto
    {
        public string AddressName { get; set; } = null!;
        public string AddressLine { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Governorate { get; set; } = null!;
        public string? PostalCode { get; set; }

    }
}
