using Xunit;

namespace ResourceApiTests
{
    public class ResourceApiUnitTests
    {
        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(-4, -6, -10)]
        [InlineData(-2, 2, 0)]
        public void ManySums(int a, int b, int c)
        {
            Assert.Equal(c, Sum(a, b));
        }


        private int Sum(int a, int b)
        {
            return 2 * a + (b - a);
        }

        [Fact]
        public void SingleSum()
        {
            Assert.Equal(4, 2 + 2);
        }
    }
}