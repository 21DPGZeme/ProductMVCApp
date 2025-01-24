using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductMVCApp;

namespace TotalPriceWithVATUnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            // Arrange
            int quantity = 295;
            decimal price = 35.29m;
            decimal expected = 12596.77m;
            Product product = new Product() { Quantity = quantity, Price = price };

            // Act
            decimal actual = Math.Round((product.Quantity * product.Price) * (1 + Configuration.GetValue<decimal>("VAT")), 2);

            // Assert
            Assert.AreEqual(expected, actual, "Incorrect result");
        }
    }
}
