using Assembler.Readers;
using Assembler.Tokens;
using System.IO;
using System.Text;
using Xunit;

namespace Tests.AssemblerTests.Tokens {
    public class RegionTokenTests {

        [Theory]
        [InlineData("*l", "l")]
        [InlineData("*l1", "l1")]
        [InlineData("*region1", "region1")]
        [InlineData("*region_1", "region_1")]
        [InlineData("*region 1", "region")]
        [InlineData("*region:", "region")]
        [InlineData("*region.", "region")]
        [InlineData("*@const", "@const")]
        public void TryAccept_InputAccepted(string input, string expectedValue) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new RegionToken();
            var result = token.TryAccept(reader);

            Assert.True(result);
            Assert.Equal(expectedValue, token.Value);
            Assert.Equal(expectedValue.Length, token.Value.Length);
        }

        [Theory]
        [InlineData("*")]
        [InlineData("region*")]
        [InlineData("* region")]
        [InlineData("\"*region\"")]
        [InlineData(".region1")]
        [InlineData(":region1")]
        public void TryAccept_InputNotAccepted(string input) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new RegionToken();
            var result = token.TryAccept(reader);

            Assert.False(result);
            Assert.Null(token.Value);
        }

        private static CodeReader CreateReader(MemoryStream ms) {
            var reader = new CodeReader(ms);
            reader.Read();
            return reader;
        }
    }
}
