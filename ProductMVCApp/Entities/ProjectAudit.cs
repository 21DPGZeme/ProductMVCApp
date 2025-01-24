using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProductMVCApp.Entities
{
    public enum ProductProperty
    {
        Product,
        Title,
        Quantity,
        Price
    }

    [Table("ProjectAudit")]
    public class ProjectAudit
    {
        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }

        public int? ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Required]
        public ProductProperty Property { get; set; }

        [Required]
        public string Value { get; set; }

        [Required]
        public DateTime TimeChanged { get; set; }
    }
}
