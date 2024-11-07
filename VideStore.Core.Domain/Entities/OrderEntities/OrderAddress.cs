namespace VideStore.Domain.Entities.OrderEntities;

public class OrderAddress
{
    public string FullName { get; set; } = null!;
    public string StreetAddress { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}