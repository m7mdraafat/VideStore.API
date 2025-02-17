using Microsoft.EntityFrameworkCore;

namespace VideStore.Domain.Entities.OrderEntities
{

    [Owned]
    public class Address
    {
        public string AddressName { get; private set; }
        public string AddressLine { get; private set; }
        public string City { get; private set; }
        public string Governorate { get; private set; }
        public string? ZipCode { get; private set; }

        private Address() { }

        public Address(string addressName, string addressLine, string city, string governorate, string? zipCode)
        {
            AddressName = addressName;
            AddressLine = addressLine;
            City = city;
            Governorate = governorate;
            ZipCode = zipCode;
        }
    }
}
