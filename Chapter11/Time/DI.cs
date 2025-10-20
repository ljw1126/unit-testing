namespace unit_testing.Chapter11.Time
{
    public interface IDateTimeServer
    {
        DateTime Now { get; }
    }

    public class DateTimeServer2 : IDateTimeServer
    {
        public DateTime Now => DateTime.Now;
    }

    public class InquiryController
    {
        private readonly IDateTimeServer _dateTimeServer; // 인터페이스에 의존해야 하는데, 예제는 구현체에 의존 오타

        public InquiryController(IDateTimeServer dateTimeServer)
        {
            _dateTimeServer = dateTimeServer;
        }

        public void ApproveInquiry(int id)
        {
            Inquiry inquiry = GetById(id);
            inquiry.Approve(_dateTimeServer.Now); // 시간을 파라미터, 일반 값으로 주입
            SaveInquiry(inquiry);
        }

        private void SaveInquiry(Inquiry inquiry)
        {
        }

        private Inquiry GetById(int id)
        {
            return null;
        }
    }

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