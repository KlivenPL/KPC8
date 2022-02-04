using Assembler.Readers;
using Assembler.Tokens;
using System.IO;
using System.Text;
using Xunit;

namespace Tests.AssemblerTests.Tokens {
    public class LabelTokenTests {

        [Theory]
        [InlineData(":l", "l")]
        [InlineData(":l1", "l1")]
        [InlineData(":label1", "label1")]
        [InlineData(":label_1", "label_1")]
        [InlineData(":label 1", "label")]
        [InlineData(":label:", "label")]
        [InlineData(":label.", "label")]
        public void TryAccept_InputAccepted(string input, string expectedValue) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new LabelToken();
            var result = token.TryAccept(reader);

            Assert.True(result);
            Assert.Equal(expectedValue, token.Value);
            Assert.Equal(expectedValue.Length, token.Value.Length);
        }

        [Theory]
        [InlineData(":")]
        [InlineData("label:")]
        [InlineData(": label")]
        [InlineData("\":label\"")]
        [InlineData(".label1")]
        [InlineData("*label1")]
        public void TryAccept_InputNotAccepted(string input) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new LabelToken();
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
