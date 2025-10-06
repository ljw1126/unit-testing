namespace unit_testing.Chapter7.Refactored2
{
    // 이전과 동일
    public class User
    {
        public int UserId { get; private set; }
        public string Email { get; private set; }
        public UserType Type { get; private set; }

        public User(int userId, string email, UserType type)
        {
            UserId = userId;
            Email = email;
            Type = type;
        }

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
            User user = UserFactory.Create(data); // ✅ 팩토리가 도메인 생성 책임 가지도록 리팩터링

            object[] companyData = _database.GetCompany();
            string companyDomainName = (string)companyData[0];
            int numberOfEmployees = (int)companyData[1];

            int newNumberOfEmployees = user.ChangeEmail(newEmail, companyDomainName, numberOfEmployees);

            _database.SaveCompany(newNumberOfEmployees);
            _database.SaveUser(user);
            _messageBus.SendEmailChangeMessage(userId, newEmail);
        }
    }

    /*
        p242
        ORM을 사용하지 않거나, 사용할 수 없으면 도메인 모델에 원시 데이터베이스 데이터로 도메인 클래스를 인스턴스화 하는 팩토리 클래스를 작성하라
        이 팩토리 클래스는 별도 클래스가 될 수도 있고, 더 간단한 경우 기존 도메인 클래스의 정적 팩터리 메서드가 될 수도 있다.
    */
    public class UserFactory
    {
        public static User Create(object[] data)
        {
            Precondition.Requires(data.Length >= 3);

            int id = (int)data[0];
            string email = (string)data[1];
            UserType type = (UserType)data[2];

            return new User(id, email, type);
        }
    }

    public static class Precondition
    {
        // NOTE: 기본값 할당, optional
        public static void Requires(bool precondition, string? message = null)
        {
            if (!precondition) throw new Exception(message);
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

    public class MessageBus
    {
        private IBus _bus;

        public void SendEmailChangeMessage(int userId, string newEmail)
        {
            _bus.Send($"Subject: USER; Type: EMAIL CHANGED; Id: {userId}; NewEmail: {newEmail}");
        }
    }
}