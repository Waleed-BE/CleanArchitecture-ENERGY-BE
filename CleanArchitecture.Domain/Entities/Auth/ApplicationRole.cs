using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Domain.Entities.Auth
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
