namespace unit_testing.Chapter3.Listing1
{
    public class CalculatorTests
    {
        // 예제 3.4 ~ 3.5
        [Fact]
        public void Sum_of_two_number()
        {
            // Arrange
            double first = 10;
            double second = 20;
            var sut = new Calculator();

            // Act
            double actual = sut.Sum(first, second);

            // Assert
            Assert.Equal(30, actual);
        }
    }

    public class Calculator
    {
        public double Sum(double first, double second)
        {
            return first + second;
        }

        public void CleanUp()
        {
        }
    }

    public class CalculatorTest2 : IDisposable
    {
        private readonly Calculator sut;

        // 클래스 내 각 테스트 이전에 호출
        public CalculatorTest2()
        {
            sut = new Calculator();
        }

        // 클래스 내 각 테스트 이후에 호출
        public void Dispose()
        {
            sut.CleanUp();
        }

        [Fact]
        public void Sum_of_two_number()
        {
            // Arrange
            double first = 10;
            double second = 20;
            var sut = new Calculator();

            // Act
            double actual = sut.Sum(first, second);

            // Assert
            Assert.Equal(30, actual);
        }
    }
}