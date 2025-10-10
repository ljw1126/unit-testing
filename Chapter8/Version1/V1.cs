using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.Data.SqlClient;
using Dapper;

namespace unit_testing.Chapter8.Version1
{
    // 7장 예제 재활용
    public class User
    {
        public int UserId { get; set; }
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
            EmailChangedEvents = new List<EmailChangedEvent>();
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
            EmailChangedEvents.Add(new EmailChangedEvent(UserId, newEmail));
        }
    }

    public class UserController
    {
        private readonly Database _database;
        private readonly IMessageBus _messageBus;

        public UserController(Database database, IMessageBus messageBus)
        {
            _database = database;
            _messageBus = messageBus;
        }

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

            foreach (EmailChangedEvent ev in user.EmailChangedEvents)
            {
                _messageBus.SendEmailChangedMessage(ev.UserId, ev.NewEmail);
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
            bool isEmailConfirmed = (bool)data[3];

            return new User(id, email, type, isEmailConfirmed);
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

    // ✅ 통합 테스트를 위해 구현
    public class Database
    {
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public object[] GetUserById(int userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // NODE: 컨벤션이.. 
                string query = "SELECT * FROM [dbo].[User] WHERE UserID = @UserID";
                dynamic data = connection.QuerySingle(query, new { UserID = userId });

                return new object[]
                {
                    data.UserID,
                    data.Email,
                    data.Type,
                    data.IsEmailConfirmed
                };
            }
        }

        public void SaveUser(User user)
        {
            // 해당 문법을 SQL Batch라고 부름
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // NOTE: 마지막 SELECT는 반환 받을 값을 뜻함
                string updateQuery = @"
                    UPDATE [dbo].[User]
                    SET Email = @Email, Type = @Type, IsEmailConfirmed = @IsEmailConfirmed
                    WHERE UserID = @UserID
                    SELECT @UserID";

                string insertQuery = @"
                    INSERT [dbo].[User] (Email, Type, IsEmailConfirmed)
                    VALUES (@Email, @Type, @IsEmailConfirmed)
                    SELECT CAST(SCOPE_IDENTITY() as int)";

                string query = user.UserId == 0 ? insertQuery : updateQuery;
                int userId = connection.Query<int>(query, new
                {
                    user.Email,
                    user.UserId,
                    user.IsEmailConfirmed,
                    Type = (int)user.Type
                })
                .Single();

                user.UserId = userId;
            }
        }

        public object[] GetCompany()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM dbo.Company";
                dynamic data = connection.QuerySingle(query);

                if (data == null) return [];

                return new object[]
                {
                    data.DomainName,
                    data.NumberOfEmployees
                };
            }
        }

        // NOTE: 비즈니스 로직이 전체 Company를 조회하도록 되어있는데, 오직 1건만 있다고 가정하는듯 하다.
        public object[] GetCompany(string domainName)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //string query = "SELECT * FROM dbo.Company";
                string query = @"SELECT * FROM [dbo].[Company] WHERE DomainName = @DomainName";
                dynamic data = connection.QuerySingle(query, new { DomainName = domainName });

                if (data == null) return [];

                return new object[]
                {
                    data.DomainName,
                    data.NumberOfEmployees
                };
            }
        }

        public void SaveCompany(Company company)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // string query = @"
                //     UPDATE dbo.Company 
                //     SET DomainName = @DomainName, NumberOfEmployees = @NumberOfEmployees";

                // connection.Execute(query, new
                // {
                //     company.DomainName,
                //     company.NumberOfEmployees
                // });

                // 1. 해당 DomainName을 가진 행을 업데이트 시도
                string updateQuery = @"
                    UPDATE [dbo].[Company] 
                    SET NumberOfEmployees = @NumberOfEmployees
                    WHERE DomainName = @DomainName";

                int rowsAffected = connection.Execute(updateQuery, company);

                // 2. 업데이트된 행이 없다면 (즉, 해당 DomainName이 없다면) INSERT 실행
                if (rowsAffected == 0)
                {
                    string insertQuery = @"
                        INSERT [dbo].[Company] (DomainName, NumberOfEmployees)
                        VALUES (@DomainName, @NumberOfEmployees)";

                    connection.Execute(insertQuery, company);
                }
            }
        }
    }

    internal interface IBus
    {
        void Send(string message);
    }

    public interface IMessageBus
    {
        void SendEmailChangedMessage(int userId, string newEmail);
    }

    public class MessageBus : IMessageBus
    {
        private IBus _bus;

        public void SendEmailChangedMessage(int userId, string newEmail)
        {
            _bus.Send($"Subject: USER; Type: EMAIL CHANGED; Id: {userId}; NewEmail: {newEmail}");
        }
    }

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

    // 예제 8.2 통합 테스트
    public class IntegrationTests : IDisposable
    {
        private const string ConnectionString = "Server=localhost,1433;Database=TestDB;User Id=sa;Password=root1234!;TrustServerCertificate=True";

        [Fact]
        public void Changing_email_from_corporate_to_non_corporate()
        {
            // Arrange
            var db = new Database(ConnectionString);
            User user = CreateUser("user@mycorp.com", UserType.Employee, db);
            CreateCompany("mycorp.com", 1, db); // Company no pk

            var messageBusMock = new Mock<IMessageBus>();
            var sut = new UserController(db, messageBusMock.Object);

            // Act
            string result = sut.ChangeEmail(user.UserId, "new@gmail.com"); // user@mycorp.com -> new@gmail.com 퇴사자인듯

            // Assert
            Assert.Equal("OK", result);

            object[] userData = db.GetUserById(user.UserId);
            User userFromDb = UserFactory.Create(userData); // user 검증
            Assert.Equal("new@gmail.com", userFromDb.Email);
            Assert.Equal(UserType.Customer, userFromDb.Type);

            object[] companyData = db.GetCompany();
            Company companyFromDb = CompanyFactory.Create(companyData); // company 검증
            Assert.Equal(0, companyFromDb.NumberOfEmployees);

            messageBusMock.Verify(
                x => x.SendEmailChangedMessage(user.UserId, "new@gmail.com"),
                Times.Once
            );
        }

        private User CreateUser(string email, UserType type, Database database)
        {
            var user = new User(0, email, type, false);
            database.SaveUser(user);
            return user;
        }

        private Company CreateCompany(string domainName, int numberOfEmployees, Database database)
        {
            var company = new Company(domainName, numberOfEmployees);
            database.SaveCompany(company);
            return company;
        }

        public void Dispose()
        {
            // 각 테스트가 끝난 후 모든 데이터를 정리 (테스트 격리)
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                // User와 Company 테이블의 모든 데이터 삭제
                connection.Execute("TRUNCATE TABLE [dbo].[User];");
                connection.Execute("TRUNCATE TABLE [dbo].[Company];");

                // NOTE: Identity(User.UserID)를 0부터 다시 시작하려면 TRUNCATE를 사용할 수 있습니다.
                // connection.Execute("TRUNCATE TABLE [dbo].[User];");
            }
        }
    }
}