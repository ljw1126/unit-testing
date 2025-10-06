namespace unit_testing.Chapter7.CanExecute
{
    // 예제 7.9 ~ 10 사용자의 이메일을 변경할지 여부를 결정하는 컨트롤러
    public class User
    {
        public int UserId { get; private set; }
        public string Email { get; private set; }
        public UserType Type { get; private set; }
        public bool IsEmailConfirmed { get; private set; }

        public User(int userId, string email, UserType type, bool isEmailConfirmed)
        {
            UserId = userId;
            Email = email;
            Type = type;
            IsEmailConfirmed = isEmailConfirmed; // ✅
        }

        // ✅
        public string CanChangeEmail()
        {
            if (IsEmailConfirmed)
                return "Can't change email after it's confirmed";

            return null;
        }

        public void ChangeEmail(string newEmail, Company company)
        {
            Precondition.Requires(CanChangeEmail() == null); // ✅

            if (Email == newEmail) return;

            UserType newType = company.IsEmailCorporate(newEmail)
                ? UserType.Employee
                : UserType.Customer;

            if (Type != newType)
            {
                int delta = newType == UserType.Employee ? 1 : -1;
                company.ChangeNumberOfEmployees(delta);
            }

            Email = newEmail;
            Type = newType;
        }
    }

    public class UserController
    {
        private readonly Database _database = new Database();
        private readonly MessageBus _messageBus = new MessageBus();

        public string ChangeEmail(int userId, string newEmail)
        {
            // 1. 데이터 준비
            object[] data = _database.GetUserById(userId);
            User user = UserFactory.Create(data);

            // 2. 의사결정 (User에서 의사결정을 옮김, 이메일 변경 진행 여부를 컨트롤러에서 수행)
            string error = user.CanChangeEmail(); // ✅ 메서드 반환 타입도 변경
            if (error != null)
                return error;

            // 결정에 따라 실행하기
            object[] companyData = _database.GetCompany();
            Company company = CompanyFactory.Create(companyData);

            // 변경시 해얄 할일을 User에서 수행
            user.ChangeEmail(newEmail, company);

            _database.SaveCompany(company);
            _database.SaveUser(user);
            _messageBus.SendEmailChangeMessage(userId, newEmail);

            return "OK";
        }
    }

    public class CompanyFactory
    {
        public static Company Create(object[] data)
        {
            Precondition.Requires(data.Length >= 2);

            string domainName = (string)data[0];
            int numberOfEmployees = (int)data[1];

            return new Company(domainName, numberOfEmployees);
        }
    }

    public class Company
    {
        public string DomainName { get; private set; }
        public int NumberOfEmployees { get; private set; }

        public Company(string domainName, int numberOfEmployees)
        {
            DomainName = domainName;
            NumberOfEmployees = numberOfEmployees;
        }

        public void ChangeNumberOfEmployees(int delta)
        {
            Precondition.Requires(NumberOfEmployees + delta >= 0);

            NumberOfEmployees += delta;
        }

        public bool IsEmailCorporate(string email)
        {
            string emailDomain = email.Split('@')[1];
            return emailDomain == DomainName;
        }
    }

    public class UserFactory
    {
        public static User Create(object[] data)
        {
            Precondition.Requires(data.Length >= 3);

            int id = (int)data[0];
            string email = (string)data[1];
            UserType type = (UserType)data[2];

            // TODO. 변경 여부에 대한 bool 플래그를 여기서 셋팅, (예제는 그냥 null 반환)
            // 가입시 default로 false가 할당된 경우가 생각되네 => 정책 풀이. confirm 받은 후에는 이메일을 변경할 수 없다.
            return new User(id, email, type, false);
        }
    }

    public static class Precondition
    {
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

        // refactoring
        public void SaveCompany(Company company)
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