
using unit_testing.Chapter2.Listing1;

namespace unit_testing.Chapter6.Listing1
{
    // 예제 6.1 출력 기반 테스트
    public class CustomerControllerTests
    {
        [Fact]
        public void Discount_of_two_products()
        {
            var product1 = new Product("Hand wash");
            var product2 = new Product("Shampoo");
            var sut = new PriceEngine();

            decimal discount = sut.CalculateDiscount(
                product1, product2);

            Assert.Equal(0.02m, discount);
        }
    }

    public class PriceEngine
    {
        public decimal CalculateDiscount(params Product[] products)
        {
            decimal discount = products.Length * 0.01m;
            return Math.Min(discount, 0.2m); // TODO. 표기법 무엇 ?
        }
    }

    public class Product
    {
        private string _name;

        public Product(string name)
        {
            _name = name;
        }
    }
}