
namespace unit_testing.Chapter7.Refactored1
{
    // 예제 7.2 애플리케이션 서비스 버전1
    // 협력 객체가 필요 없어짐 ➡️ 도메인 모델 
    public class User
    {
        public int UserId { get; private set; }
        public string Email { get; private set; }
        public UserType Type { get; private set; }

        // ✅ 생성자 추가
        public User(int userId, string email, UserType type)
        {
            UserId = userId;
            Email = email;
            Type = type;
        }

        // Database, MessageBus에 직접 의존성이 사라짐 
        // numberOfEmployees를 반환
        public int ChangeEmail(string newEmail, string companyDomainName, int numberOfEmployees)
        {
            if (Email == newEmail)
            {
                return numberOfEmployees;
            }

            string emailDomain = newEmail.Split('@')[1];
            bool isEmailCorporate = (emailDomain == companyDomainName);
            UserType newType = (isEmailCorporate ? UserType.Employee : UserType.Customer);

            if (Type != newType)
            {
                int delta = (newType == UserType.Employee ? 1 : -1);
                int newNumber = numberOfEmployees + delta;
                numberOfEmployees = newNumber;
            }

            Email = newEmail;
            Type = newType;

            return numberOfEmployees;
        }
    }

    public class UserController
    {
        private readonly Database _database = new Database();
        private readonly MessageBus _messageBus = new MessageBus();

        public void ChangeEmail(int userId, string newEmail)
        {
            object[] data = _database.GetUserById(userId);
            string email = (string)data[1];
            UserType type = (UserType)data[2];
            //💩 복잡한 로직에 속함. 애플리케이션 서비스의 역할은 복잡도나 도메인 유의성이 로직이 아니라 오케스트레이션만 해당한다.
            var user = new User(userId, email, type);

            object[] companyData = _database.GetCompany();
            string companyDomainName = (string)companyData[0];
            int numberOfEmployees = (int)companyData[1];

            // 💩 업데이트된 직원수를 반환하는데 이상. 회사 직원 수는 특정 사용자와 관련이 없다. 이 책임은 다른곳에 있어야 한다
            int newNumberOfEmployees = user.ChangeEmail(newEmail, companyDomainName, numberOfEmployees);

            _database.SaveCompany(newNumberOfEmployees);
            _database.SaveUser(user);
            // 💩 컨트롤러는 새로운 이메일이 전과 다른지 여부와 관계없이 무조건 데이터를 수정해서 저장하고 메시지 버스에 알림을 보낸다
            _messageBus.SendEmailChangeMessage(userId, newEmail);
        }
    }

    public enum UserType
    {
        Customer = 1,
        Employee = 2
    }

    // static 키워드 제거
    public class Database
    {
        public object[] GetUserById(int userId)
        {
            return null;
        }

        public User GetUserByEmail(string email)
        {
            return null;
        }

        public void SaveUser(User user)
        {

        }

        public object[] GetCompany()
        {
            return null;
        }

        public void SaveCompany(int newNumber)
        {

        }
    }

    internal interface IBus
    {
        void Send(string message);
    }

    // static 키워드 제거
    public class MessageBus
    {
        private IBus _bus;

        public void SendEmailChangeMessage(int userId, string newEmail)
        {
            // TODO
            _bus.Send($"Subject: USER; Type: EMAIL CHANGED; Id: {userId}; NewEmail: {newEmail}");
        }
    }
}