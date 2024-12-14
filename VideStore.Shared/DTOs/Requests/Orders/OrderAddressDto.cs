namespace VideStore.Shared.DTOs.Requests.Orders;

public class OrderAddressDto
{
    public string FullName { get; set; } = null!;
    public string StreetAddress { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Governorate { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}