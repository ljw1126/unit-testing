namespace unit_testing.Chapter11.LeakingKnowledge
{
    // 예제 11.5 알고리즘 구현 유출
    public static class Calculator
    {
        public static int Add(int value1, int value2)
        {
            return value1 + value2;
        }
    }

    public class CalculatorTests
    {
        [Fact]
        public void Adding_two_numbers()
        {
            int value1 = 1;
            int value2 = 3;
            int expected = value1 + value2; // 💩 비즈니스 로직 노출

            int actual = Calculator.Add(value1, value2);

            Assert.Equal(expected, actual);
        }
    }

    // 예제 11.6 같은 테스트의 매개변수화 버전
    public class CalculatorTests2
    {
        [Theory]
        [InlineData(1, 3)]
        [InlineData(11, 33)]
        [InlineData(100, 500)]
        public void Adding_two_numbers(int value1, int value2)
        {
            int expected = value1 + value2; // 💩 비즈니스 로직 노출

            int actual = Calculator.Add(value1, value2);

            Assert.Equal(expected, actual);
        }
    }

    // 예제 11.7 도메인 지식이 없는 테스트 
    public class CalculatorTests4
    {
        // ✅ 계산하지 말고 하드코딩으로 결과값으로 비교한다
        [Theory]
        [InlineData(1, 3, 4)]
        [InlineData(11, 33, 44)]
        [InlineData(100, 500, 600)]
        public void Adding_two_numbers(int value1, int value2, int expected)
        {
            int actual = Calculator.Add(value1, value2);
            Assert.Equal(expected, actual);
        }
    }
}