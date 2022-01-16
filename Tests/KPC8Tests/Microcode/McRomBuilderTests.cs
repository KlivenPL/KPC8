using KPC8.Microcode;
using KPC8.RomProgrammers.Microcode;
using System.Linq;
using Xunit;

namespace Tests.KPC8Tests.Microcode {
    public class McRomBuilderTests : TestBase {

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
            var builder = new McRomBuilder(64)
                .SetDefaultInstruction(GetDefaultInstruction())
                .FindAndAddAllProceduralInstructions();

            var romData = builder.Build();
            Assert.Equal(64 * 16 * 40, romData.Sum(r => r.Length));
        }

        private McInstruction GetDefaultInstruction() {
            return new McProceduralInstruction("NOP", NopInstruction.Nop().ToArray(), 0x0);
        }
    }
}
