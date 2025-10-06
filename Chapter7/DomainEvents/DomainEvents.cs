using System.Dynamic;
using FluentAssertions;

namespace unit_testing.Chapter7.DomainEvents
{
    // 예제 7.12 이메일이 변경될 때 이벤트를 추가하는 User
    public class User
    {
        public int UserId { get; private set; }
        public string Email { get; private set; }
        public UserType Type { get; private set; }
        public bool IsEmailConfirmed { get; private set; }
        public List<EmailChangedEvent> EmailChangedEvents { get; private set; }

        public User(int userId, string email, UserType type, bool isEmailConfirmed)
        {
            UserId = userId;
            Email = email;
            Type = type;
            IsEmailConfirmed = isEmailConfirmed;
            EmailChangedEvents = new List<EmailChangedEvent>(); // ✅
        }

        public string CanChangeEmail()
        {
            if (IsEmailConfirmed)
                return "Can't change email after it's confirmed";

            return null;
        }

        public void ChangeEmail(string newEmail, Company company)
        {
            Precondition.Requires(CanChangeEmail() == null);

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
            EmailChangedEvents.Add(new EmailChangedEvent(UserId, newEmail)); // ✅
        }
    }

    // 예제 7.13 도메인 이벤트를 처리하는 컨트롤러
    public class UserController
    {
        private readonly Database _database = new Database();
        private readonly MessageBus _messageBus = new MessageBus();

        public string ChangeEmail(int userId, string newEmail)
        {
            object[] data = _database.GetUserById(userId);
            User user = UserFactory.Create(data);

            string error = user.CanChangeEmail();
            if (error != null)
                return error;

            object[] companyData = _database.GetCompany();
            Company company = CompanyFactory.Create(companyData);

            user.ChangeEmail(newEmail, company);

            _database.SaveCompany(company);
            _database.SaveUser(user);
            // ✅ 애플리케이션이 프로세스 외부 의존성을 도메인 모델에 넘김.
            // 이때 도메인 이벤트는 이미 일어난 일이기 때문에 과거 시제로 명명하고, 불변이다
            foreach (EmailChangedEvent ev in user.EmailChangedEvents)
            {
                _messageBus.SendEmailChangeMessage(ev.UserId, ev.NewEmail);
            }

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

    // ✅ 컬렉션에서도 사용할거라 오버라이딩 재정의
    public class EmailChangedEvent
    {
        public int UserId { get; }
        public string NewEmail { get; }

        public EmailChangedEvent(int userId, string newEmail)
        {
            UserId = userId;
            NewEmail = newEmail;
        }

        protected bool Equals(EmailChangedEvent other)
        {
            return UserId == other.UserId && string.Equals(NewEmail, other.NewEmail);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((EmailChangedEvent)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (UserId * 397) ^ (NewEmail != null ? NewEmail.GetHashCode() : 0);
            }
        }
    }

    // 예제 7.14 도메인 이벤트 생성 테스트
    public class Tests
    {
        [Fact]
        public void Changing_email_from_corporate_to_non_corporate()
        {
            var company = new Company("mycorp.com", 1);
            var sut = new User(1, "user@mycorp.com", UserType.Employee, false);

            sut.ChangeEmail("new@gmail.com", company);

            company.NumberOfEmployees.Should().Be(0);
            sut.Email.Should().Be("new@gmail.com");
            sut.Type.Should().Be(UserType.Customer);
            sut.EmailChangedEvents.Should().Equal(
                new EmailChangedEvent(1, "new@gmail.com")
            );
        }
    }
}