using Moq;
using Xunit;

namespace unit_testing.Chapter5.Listing1
{
    // NOTE : Controller == Application Service
    public class ControllerTests
    {
        [Fact]
        public void Sending_a_greetings_email()
        {
            var emailGatewayMock = new Mock<IEmailGateway>();
            var sut = new Controller(emailGatewayMock.Object);

            sut.GreetUser("tester@email.com");

            emailGatewayMock.Verify(
                x => x.SendGreetingsEmail("tester@email.com"),
                Times.Once
            );
        }
    }

    public class Controller
    {
        private readonly IEmailGateway _emailGateway;
        private readonly IDatabase _database;

        public Controller(IEmailGateway emailGateway)
        {
            _emailGateway = emailGateway;
        }

        public Controller(IDatabase database)
        {
            _database = database;
        }

        public void GreetUser(string userEmail)
        {
            _emailGateway.SendGreetingsEmail(userEmail);
        }

        public Report CreateReport()
        {
            int numberOfUsers = _database.GetNumberOfUsers();
            return new Report(numberOfUsers);
        }
    }

    public class Report(int numberOfUsers)
    {
        public int NumberOfusers { get; } = numberOfUsers;
    }
    
    public interface IDatabase
        {
            int GetNumberOfUsers();
        }

        public interface IEmailGateway
        {
            void SendGreetingsEmail(string userEmail);
        }
}