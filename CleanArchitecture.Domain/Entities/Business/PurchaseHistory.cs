using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Domain.Entities.Business
{
    public class PurchaseHistory : BaseEntity
    {
        public Guid PurchaseHistoryId { get; set; }
        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string StripePriceId { get; set; }
        public string PlanName { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Interval { get; set; }
        public string PlanPurchaseType { get; set; }
    }
}
