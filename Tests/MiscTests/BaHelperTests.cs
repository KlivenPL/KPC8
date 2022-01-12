using Infrastructure.BitArrays;
using Tests._Infrastructure;
using Xunit;

namespace Tests.MiscTests {
    public class BaHelperTests {

        [Theory]
        [InlineData("00000000", 0)]
        [InlineData("00000001", 1)]
        [InlineData("00001111", 15)]
        [InlineData("10000000", 128)]
        [InlineData("10001111", 143)]
        [InlineData("11111111", 255)]
        public void FromToByteLE(string originalBits, byte expectedValue) {
            var originalBa = BitArrayHelper.FromString(originalBits);
            byte value = originalBa.ToByteLE();
            Assert.Equal(value, expectedValue);
            var backAgain = BitArrayHelper.FromByteLE(value);
            BitAssert.Equality(originalBa, backAgain);
        }

        [Theory]
        [InlineData("00000000", 0)]
        [InlineData("00000001", 1)]
        [InlineData("00001111", 15)]
        [InlineData("10000000", -128)]
        [InlineData("10001111", -113)]
        [InlineData("11111111", -1)]
        public void FromToSByteLE(string originalBits, sbyte expectedValue) {
            var originalBa = BitArrayHelper.FromString(originalBits);
            sbyte value = originalBa.ToSByteLE();
            Assert.Equal(value, expectedValue);
            var backAgain = BitArrayHelper.FromSByteLE(value);
            BitAssert.Equality(originalBa, backAgain);
        }

        [Theory]
        [InlineData("00000000 00000000", 0)]
        [InlineData("00000000 00000001", 1)]
        [InlineData("00000000 00001111", 15)]
        [InlineData("01000000 10000000", 16512)]
        [InlineData("11111111 11111111", -1)]
        [InlineData("11111111 11110111", -9)]
        [InlineData("10000000 11110111", -32521)]
        [InlineData("10000000 00000000", -32768)]
        public void FromToShortLE(string originalBits, short expectedValue) {
            var originalBa = BitArrayHelper.FromString(originalBits);
            short value = originalBa.ToShortLE();
            Assert.Equal(value, expectedValue);
            var backAgain = BitArrayHelper.FromShortLE(value);
            BitAssert.Equality(originalBa, backAgain);
        }

        [Theory]
        [InlineData("00000000 00000000", 0)]
        [InlineData("00000000 00000001", 1)]
        [InlineData("00000000 00001111", 15)]
        [InlineData("01000000 10000000", 16512)]
        [InlineData("11111111 11111111", 65535)]
        [InlineData("11111111 11110111", 65527)]
        [InlineData("10000000 11110111", 33015)]
        [InlineData("10000000 00000000", 32768)]
        public void FromToUShortLE(string originalBits, ushort expectedValue) {
            var originalBa = BitArrayHelper.FromString(originalBits);
            ushort value = originalBa.ToUShortLE();
            Assert.Equal(value, expectedValue);
            var backAgain = BitArrayHelper.FromUShortLE(value);
            BitAssert.Equality(originalBa, backAgain);
        }

        [Theory]
        [InlineData("00000000 00000000 00000000 00000000", 0)]
        [InlineData("00000000 00000000 00000000 00000001", 1)]
        [InlineData("00000000 00000000 00000000 00001111", 15)]
        [InlineData("00000000 00000000 01000000 10000000", 16512)]
        [InlineData("01000001 11000001 01000100 10000000", 1103185024)]
        [InlineData("11111111 11111111 11111111 11111111", -1)]
        [InlineData("11111111 11111111 11111111 11110111", -9)]
        [InlineData("11111111 11111111 10000000 11110111", -32521)]
        [InlineData("11111111 11111111 10000000 00000000", -32768)]
        [InlineData("11111111 10110001 10000000 00000000", -5144576)]
        [InlineData("10110111 01110000 10000010 00000001", -1217363455)]
        [InlineData("10000000 00000000 00000000 00000000", -2147483648)]
        public void FromToIntLE(string originalBits, int expectedValue) {
            var originalBa = BitArrayHelper.FromString(originalBits);
            int value = originalBa.ToIntLE();
            Assert.Equal(value, expectedValue);
            var backAgain = BitArrayHelper.FromIntLE(value);
            BitAssert.Equality(originalBa, backAgain);
        }

        [Theory]
        [InlineData("00000000 00000000 00000000 00000000", 0)]
        [InlineData("00000000 00000000 00000000 00000001", 1)]
        [InlineData("00000000 00000000 00000000 00001111", 15)]
        [InlineData("00000000 00000000 01000000 10000000", 16512)]
        [InlineData("01000001 11000001 01000100 10000000", 1103185024)]
        [InlineData("11111111 11111111 11111111 11111111", 4294967295)]
        [InlineData("11111111 11111111 11111111 11110111", 4294967287)]
        [InlineData("11111111 11111111 10000000 11110111", 4294934775)]
        [InlineData("11111111 11111111 10000000 00000000", 4294934528)]
        [InlineData("11111111 10110001 10000000 00000000", 4289822720)]
        [InlineData("10110111 01110000 10000010 00000001", 3077603841)]
        [InlineData("10000000 00000000 00000000 00000000", 2147483648)]
        public void FromToUIntLE(string originalBits, uint expectedValue) {
            var originalBa = BitArrayHelper.FromString(originalBits);
            uint value = originalBa.ToUIntLE();
            Assert.Equal(value, expectedValue);
            var backAgain = BitArrayHelper.FromUIntLE(value);
            BitAssert.Equality(originalBa, backAgain);
        }
    }
}
