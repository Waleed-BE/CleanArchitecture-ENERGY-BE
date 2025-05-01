using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.DTOs.Business
{
    public class AddAddressDto
    {
        public Guid AddressId { get; set; }
        public string AddressName { get; set; } // e.g., Munich appartment etc
        public string CompleteAddress { get; set; }
        public string? Description { get; set; }
        public AddressTypeEnum AddressType { get; set; }
        public StatusEnum Status { get; set; }
    }
}
