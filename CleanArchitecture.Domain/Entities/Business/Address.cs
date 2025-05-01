using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Domain.Entities.Business
{
    public class Address : BaseEntity
    {
        [Key]
        public Guid AddressId { get; set; }
        [Required]
        public string AddressName { get; set; } // e.g., Munich appartment etc
        public string CompleteAddress { get; set; }
        public string? Description { get; set; }
        public AddressTypeEnum AddressType { get; set; }
        public StatusEnum Status { get; set; }
        public ICollection<AddressExpenses> addressExpenses = new List<AddressExpenses>();
    }
}
