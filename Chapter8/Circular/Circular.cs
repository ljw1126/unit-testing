
// 8.5.3 순환 의존성 제거하기에 언급된 예.
namespace unit_testing.Chapter8.Circular
{
    public class CheckOutService
    {
        public void CheckOut(int orderId)
        {
            var service = new ReportGenerationService();
            service.GenerateReport(orderId, this);

            /* other work */
        }
    }

    public class ReportGenerationService
    {
        public void GenerateReport(
            int orderId,
            CheckOutService checkOutService
        )
        {
            /* calls checkOutService when generation is completed */
        }
    }
}