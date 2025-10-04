
namespace unit_testing.Chapter6.Listing2
{
    // 예제 6.2 상태 기반 테스트
    public class CustomerControllerTests
    {
        [Fact]
        public void Adding_a_product_to_an_order()
        {
            var product = new Product("Hand wash");
            var sut = new Order();

            sut.AddProduct(product);

            Assert.Equal(1, sut.Products.Count);
            Assert.Equal(product, sut.Products[0]);
        }
    }

    public class Order
    {
        private readonly List<Product> _products = new List<Product>();
        public IReadOnlyList<Product> Products => _products.ToList();

        public void AddProduct(Product product)
        {
            _products.Add(product);
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