namespace unit_testing.Chapter11.PrivateMethods
{
    // 예제 11.1 복잡한 비공개 메서드가 있는 클래스
     public class Order
    {
        private Customer _customer;
        private List<Product> _products;

        public string GenerateDescription()
        {
            return $"Customer name: {_customer.Name}, " +
                $"total number of products: {_products.Count}, " +
                $"total price: {GetPrice()}"; // 복잡한 비즈니스 로직을 공개 메서드에서 사용하고 있다. 테스트 하기 어렵다.
        }

        private decimal GetPrice()
        {
            decimal basePrice = /* Calculate based on _products */ 0;
            decimal discounts = /* Calculate based on _customer */ 0;
            decimal taxes = /* Calculate based on _products */ 0;
            return basePrice - discounts + taxes;
        }
    }

    public class Product
    {
    }

    public class Customer
    {
        public object Name { get; set; }
    }

    // 예제 11.2 복잡한 비공개 메서드 추출
    public class OrderV2
    {
        private Customer _customer;
        private List<Product> _products;

        public string GenerateDescription()
        {
            var calculator = new PriceCalculator();

            return $"Customer name: {_customer.Name}, " +
                $"total number of products: {_products.Count}, " +
                $"total price: {calculator.Calculate(_customer, _products)}";
        }
    }

    public class PriceCalculator
    {
        public decimal Calculate(Customer customer, List<Product> products)
        {
            decimal basePrice = /* Calculate based on products */ 0;
            decimal discounts = /* Calculate based on customer */ 0;
            decimal taxes = /* Calculate based on products */ 0;
            return basePrice - discounts + taxes;
        }
    }

    // 예제 11.3 비공개 생성자가 있는 클래스
    public class Inquiry
    {
        public bool IsApproved { get; private set; }
        public DateTime? TimeApproved { get; private set; }

        private Inquiry(bool isApproved, DateTime? timeApproved)
        {
            if (isApproved && !timeApproved.HasValue)
                throw new Exception();

            IsApproved = isApproved;
            TimeApproved = timeApproved;
        }

        public void Approve(DateTime now)
        {
            if (IsApproved)
                return;

            IsApproved = true;
            TimeApproved = now;
        }
    }
}