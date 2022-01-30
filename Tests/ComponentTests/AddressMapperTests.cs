using _Infrastructure.BitArrays;
using Components.SignalMappers;
using Components.Signals;
using Infrastructure.BitArrays;
using System;
using System.Collections;
using System.Linq;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class AddressMapperTests : TestBase {

        [Fact]
        public void AddressMatch_IsMatch() {
            ushort mappedAddress = 0x2137;
            var mappedAddressVal = BitArrayHelper.FromUShortLE(mappedAddress);
            var zero16 = new BitArray(16);

            using var addressMapper = CreateAddressMapper(builder => builder.Add(mappedAddress), out Signal[] addressInput, out Signal outputEnable, out Signal isMatch);

            addressInput.Write(zero16);

            // nothing
            Enable(outputEnable);
            MakeTickAndWait();
            Assert.False(isMatch);

            addressInput.Write(mappedAddressVal);

            // match
            Enable(outputEnable);
            MakeTickAndWait();
            Assert.True(isMatch);

            // match
            Enable(outputEnable);
            MakeTickAndWait();
            Assert.True(isMatch);

            addressInput.Write(zero16);

            // nothing
            Enable(outputEnable);
            MakeTickAndWait();
            Assert.False(isMatch);
        }

        private AddressMapper CreateAddressMapper(Func<MappedAddressBuilder, MappedAddressBuilder> mappedAddressBuilder, out Signal[] addressInput, out Signal outputEnable, out Signal isMatch) {
            var mapper = AddressMapper.Create(16, mappedAddressBuilder);
            addressInput = mapper.CreateSignalAndPlugIn(x => x.AddressInput).ToArray();

            outputEnable = mapper.CreateSignalAndPlugInPort(r => r.OutputEnable);
            isMatch = mapper.IsMatch;

            return mapper;
        }
    }
}
