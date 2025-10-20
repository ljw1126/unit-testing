namespace unit_testing.Chapter11.PrivateState
{
    // 예제 11.4 비공개 상태가 있는 클래스
    public class Customer
    {
        // 상태를 public으로 공개하면 캡슐화가 깨지고, 내부 세부사항에 결합도가 높아짐 
        // 상태 패턴 마냥 행위에 따라 상태를 가진 객체가 변하면 되지 않나??
        private CustomerStatus _status = CustomerStatus.Regular;

        public void Promote()
        {
            _status = CustomerStatus.Preferred;
        }

        public decimal GetDiscount()
        {
            return _status == CustomerStatus.Preferred ? 0.05m : 0m;
        }
    }

    public enum CustomerStatus
    {
        Regular,
        Preferred
    }
}