using Microsoft.AspNetCore.Identity;
using ProductMVCApp.Entities;

namespace ProductMVCApp.Models
{
    public class ProjectAuditModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public IdentityUser User { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public ProductProperty Property { get; set; }

        public string Value { get; set; }

        public DateTime TimeChanged { get; set; }
    }
}
