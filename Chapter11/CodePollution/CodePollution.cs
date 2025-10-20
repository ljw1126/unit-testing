namespace unit_testing.Chapter11.CodePollution
{
    // 예제 11.8 bool 트리거가 있는 로거
    public class Logger
    {
        private readonly bool _isTestEnvironment;

        public Logger(bool isTestEnvironment)
        {
            _isTestEnvironment = isTestEnvironment;
        }

        public void Log(string text)
        {
            if (_isTestEnvironment)
                return;

            /* Log the text */
        }
    }

    public class Controller
    {
        public void SomeMethod(Logger logger)
        {
            logger.Log("SomeMethod is called");
        }
    }

    // 예제 11.9 불 스위치를 사용한 테스트
    public class Tests
    {
        [Fact]
        public void Some_test()
        {
            var logger = new Logger(true);
            var sut = new Controller();

            sut.SomeMethod(logger);

            /* assert */
        }
    }

    // 예제 11.10 스위치가 없는 버전
    public interface ILogger
    {
        void Log(string text);
    }

    public class Logger2 : ILogger
    {
        public void Log(string text)
        {
            /* Log the text */
        }
    }

    // ✅ 가짜 객체를 주입받아 사용
    public class FakeLogger : ILogger
    {
        public void Log(string text)
        {
            /* Do nothing */
        }
    }

    public class Controller2
    {
        public void SomeMethod(ILogger logger)
        {
            logger.Log("SomeMethod is called");
        }
    }
}