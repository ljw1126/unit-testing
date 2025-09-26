namespace unit_testing.Chapter3.Listing1
{
    public class CalculatorTests
    {
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
    }
}