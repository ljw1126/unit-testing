// p296 순환 의존성 제거 예
namespace unit_testing.Chapter8.NonCircular
{
    public class CheckOutService
    {
        public void CheckOut(int orderId)
        {
            var service = new ReportGenerationService();
            Report report = service.GenerateReport(orderId);

            /* other work */
        }
    }

    public class ReportGenerationService
    {
        public Report GenerateReport(
            int orderId
        )
        {
            // do something and return Report type result
            return null;
        }
    }

    public class Report
    {

    }
}