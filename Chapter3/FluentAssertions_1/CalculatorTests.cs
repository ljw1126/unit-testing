using System.IO.Pipelines;
using FluentAssertions;
using Xunit;

namespace unit_testing.Chapter3.FluentAssertions_1
{
    public class CalculatorTests
    {
        [Fact]
        public void Sum_of_two_numbers()
        {
            double first = 10;
            double second = 20;
            var sut = new Calculator();

            double actual = sut.Sum(first, second);

            actual.Should().Be(30);
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