using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace VideStore.Domain.Entities.OrderEntities;


[Owned]
public class OrderAddress
{
    [Required]
    public string FullName { get; set; } = null!;
    [Required]
    public string StreetAddress { get; set; } = null!;
    [Required]
    public string City { get; set; } = null!;
    [Required]
    public string Governorate { get; set; } = null!;
    [Required]
    public string PostalCode { get; set; } = null!;
    [Required, Phone]
    public string PhoneNumber { get; set; } = null!;
}
