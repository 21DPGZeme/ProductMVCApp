using System.Net.NetworkInformation;
using ProductMVCApp.Entities;

namespace TotalPriceVATCalcTest
{
    public class UnitTest
    {
        [Fact]
        public void Test1()
        {
            // Arrange
            int quantity = 295;
            decimal price = 35.29m;
            decimal VAT = 0.21m;
            decimal expected = 12596.77m;
            Product product = new Product() { Quantity = quantity, Price = price };

            // Act
            decimal actual = Math.Round((product.Quantity * product.Price) * (1 + VAT), 2);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}