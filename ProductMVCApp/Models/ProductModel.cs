using System.ComponentModel.DataAnnotations;

namespace ProductMVCApp.Models
{
    public class ProductModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        //[Range(0, double.MaxValue)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public bool IsDeleted { get; set; }
    }
}
