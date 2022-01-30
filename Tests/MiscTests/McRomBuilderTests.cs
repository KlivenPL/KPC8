using KPC8.Microcode;
using KPC8.RomProgrammers.Microcode;
using System.Linq;
using Xunit;

namespace Tests.MiscTests {
    public class McRomBuilderTests : TestBase {

        private const int ProceduralInstructionsCount = 56;
        private const int ConditionalInstructionsCount = 8;
        private const int WastedInstructions = 8;

        private int TotalInstructions => ProceduralInstructionsCount + ConditionalInstructionsCount + WastedInstructions;
        private int TotalLength => (ProceduralInstructionsCount + WastedInstructions) * 16 * 40 + ConditionalInstructionsCount * 8 * 16 * 40;

        [Fact]
        public void BuildMcRomBuilder() {
            var mcRomBuilder = new McRomBuilder(64);
            var instruction = new McProceduralInstruction(nameof(McInstructionType.Nop), NopInstruction.Nop().ToArray(), (ushort)McInstructionType.Nop);

            mcRomBuilder.AddInstructions(new McProceduralInstruction[] { instruction });
            mcRomBuilder.SetDefaultInstruction(GetDefaultInstruction());
            var output = mcRomBuilder.Build();

            Assert.NotNull(output);

            for (int i = 0; i < 64; i++) {
                Assert.NotNull(mcRomBuilder.GetInstructions[i]);
            }
        }

        [Fact]
        public void FindAllProceduralInstructions() {
            var builder = new McRomBuilder(ProceduralInstructionsCount)
                .SetDefaultInstruction(GetDefaultInstruction())
                .FindAndAddAllProceduralInstructions();

            var romData = builder.Build();
            Assert.Equal(56 * 16 * 40, romData.Sum(r => r.Length));
        }

        [Fact]
        public void FindAllConditionalInstructions() {
            var builder = new McRomBuilder(TotalInstructions)
                .SetDefaultInstruction(GetDefaultInstruction())
                .FindAndAddAllConditionalInstructions();

            var romData = builder.Build();
            Assert.Equal(TotalLength, romData.Sum(r => r.Length));
        }

        [Fact]
        public void FindAllInstructions() {
            var builder = new McRomBuilder(TotalInstructions)
                .SetDefaultInstruction(GetDefaultInstruction())
                .FindAndAddAllProceduralInstructions()
                .FindAndAddAllConditionalInstructions();

            var romData = builder.Build();
            Assert.Equal(TotalLength, romData.Sum(r => r.Length));
        }

        private McInstruction GetDefaultInstruction() {
            return new McProceduralInstruction("NOP", NopInstruction.Nop().ToArray(), 0x0);
        }
    }
}
