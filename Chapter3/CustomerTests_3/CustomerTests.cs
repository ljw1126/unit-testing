using unit_testing.Chapter2.Listing1;
using Xunit;

// 예제 3.7 테스트 생성자에서 초기화 코드 추출
namespace unit_testing.Chapter3.CustomerTests_3
{
    public class CustomerTests
    {
        private readonly Store _store;
        private readonly Customer _sut;

        public CustomerTests()
        {
            _store = new Store();
            _store.AddInventory(Product.Shampoo, 10);
            _sut = new Customer();
        }

        [Fact]
        public void Purchase_succeeds_when_enough_inventory()
        {
            bool actual = _sut.Purchase(_store, Product.Shampoo, 5);

            Assert.True(actual);
            Assert.Equal(5, _store.GetInventory(Product.Shampoo));
        }

        [Fact]
        public void Purchase_fails_when_not_enough_inventory()
        {
            bool actual = _sut.Purchase(_store, Product.Shampoo, 15);

            Assert.False(actual);
            Assert.Equal(10, _store.GetInventory(Product.Shampoo));
        }
    }
}