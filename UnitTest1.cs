namespace unit_testing;

public class UnitTest1
{
    [Fact(DisplayName = "덧셈")]
    public void Test1()
    {
        Assert.Equal(4, Sum(2, 2));
    }

    [Fact(DisplayName = "문자열 비교")]
    public void Test2()
    {
        Assert.Equal("123", "123");
    }

    private int Sum(int a, int b)
    {
        return a + b;
    }
}
