using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProductMVCApp.Entities
{
    public enum ProductProperty
    {
        Title,
        Quantity,
        Price
    }

    [Table("ProjectAudit")]
    public class ProjectAudit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
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
