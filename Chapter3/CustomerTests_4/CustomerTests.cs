using unit_testing.Chapter2.Listing1;
using Xunit;

// 예제 3.8 비공개 팩토리 메서드로 도출한 공통 초기화 코드
namespace unit_testing.Chapter3.CustomerTests_4
{
    public class CustomerTests
    {
     
        [Fact]
        public void Purchase_succeeds_when_enough_inventory()
        {
            Store store = CreateStoreWithInventory(Product.Shampoo, 10);
            Customer sut = CreateCustomer();

            bool actual = sut.Purchase(store, Product.Shampoo, 5);

            Assert.True(actual);
            Assert.Equal(5, store.GetInventory(Product.Shampoo));
        }

        [Fact]
        public void Purchase_fails_when_not_enough_inventory()
        {
            Store store = CreateStoreWithInventory(Product.Shampoo, 10);
            Customer sut = CreateCustomer();

            bool actual = sut.Purchase(store, Product.Shampoo, 15);    

            Assert.False(actual);
            Assert.Equal(10, store.GetInventory(Product.Shampoo));
        }

        private Store CreateStoreWithInventory(Product product, int quantity)
        {
            Store store = new Store();
            store.AddInventory(product, quantity);
            return store;
        }

        private static Customer CreateCustomer()
        {
            return new Customer();
        }
    }
}