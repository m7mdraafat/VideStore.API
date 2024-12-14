using VideStore.Shared.DTOs.Responses.Users;

namespace VideStore.Shared.DTOs.Responses.Users
{
    public record CurrentUserResponse(
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        List<UserAddressDto> UserAddresses);
}
