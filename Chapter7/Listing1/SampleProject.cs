using unit_testing.Chapter3.CustomerTests_4;
using Xunit.Sdk;

namespace unit_testing.Chapter7.Listing1
{
    //예제 7.1 CRM 시스템의 초기 구현
    public class User
    {
        public int UserId { get; private set; }
        public string Email { get; private set; }
        public UserType Type { get; private set; }

        public void ChangeEmail(int userId, string newEmail)
        {
            object[] data = Database.GetUserById(userId);
            UserId = userId;
            Email = (string)data[1];
            Type = (UserType)data[2];

            if (Email == newEmail) return;

            object[] companyData = Database.GetCompany();
            string companyDomainName = (string)companyData[0];
            int numberOfEmployees = (int)companyData[1];

            string emailDomain = newEmail.Split('@')[1];
            bool isEmailCorporate = (emailDomain == companyDomainName);
            UserType newType = (isEmailCorporate ? UserType.Employee : UserType.Customer);

            if (Type != newType)
            {
                int delta = (newType == UserType.Employee ? 1 : -1);
                int newNumber = numberOfEmployees + delta;
                Database.SaveCompany(newNumber);
            }

            Email = newEmail;
            Type = newType;

            Database.SaveUser(this);
            MessageBus.SendEmailChangeMessage(UserId, newEmail);
        }
    }

    public enum UserType
    {
        Customer = 1,
        Employee = 2
    }

    public class Database
    {
        public static object[] GetUserById(int userId)
        {
            return null;
        }

        public static User GetUserByEmail(string email)
        {
            return null;
        }

        public static void SaveUser(User user)
        {

        }

        public static object[] GetCompany()
        {
            return null;
        }

        public static void SaveCompany(int newNumber)
        {

        }
    }

    internal interface IBus
    {
        void Send(string message);
    }

    public class MessageBus
    {
        private static IBus _bus;

        public static void SendEmailChangeMessage(int userId, string newEmail)
        {
            // TODO
            _bus.Send($"Subject: USER; Type: EMAIL CHANGED; Id: {userId}; NewEmail: {newEmail}");
        }
    }
}