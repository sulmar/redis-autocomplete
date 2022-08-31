using Xunit;

namespace RedisAutoComplete.IntegrationTests
{
    public class SharedRedisCompletionServiceUnitTests : IClassFixture<RedisDatabaseFixture>
    {
        private readonly RedisDatabaseFixture fixture;
        public SharedRedisCompletionServiceUnitTests(RedisDatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Get_Prefix_ShouldReturnsAutocompletedWords()
        {
            // Arrange
            string prefix = "abb";

            // Act
            var result = fixture.completionService.Get(prefix);

            // Assert
            Assert.All(result, item => Assert.Contains("abb", item));
        }

        [Theory]
        [InlineData("a")]
        [InlineData("ab")]        
        [InlineData("abb")]
        public void Get_Prefix_ShouldReturnsAutocompletedWords2(string prefix)
        {
            // Arrange

            // Act
            var result = fixture.completionService.Get(prefix);

            // Assets
            Assert.All(result, item => Assert.StartsWith(prefix, item));
        }

        [Theory]
        [InlineData("zo")]        
        public void Get_Prefix_ShouldReturnsAutocompletedWords3(string prefix)
        {
            // Arrange

            // Act
            var result = fixture.completionService.Get(prefix);

            // Assets
            var expected = new string[]
            {
                "zoe",
                "zola",
                "zonda",
                "zondra",
                "zonnya",
                "zora",
                "zorah",
                "zorana",
                "zorina",
                "zorine",
            };
           
        }
    }
}
