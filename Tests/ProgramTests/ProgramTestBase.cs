using Assembler;
using Assembler.Readers;
using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tests.ProgramTests {
    public abstract class ProgramTestBase : TestBase {
        private static string[] embeddedResourceNames;

        static ProgramTestBase() {
            embeddedResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        }

        protected CsPanel CompileAndBuildPcModules(string embeddedFileName, out ModulePanel modules) {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = embeddedResourceNames.First(x => x.EndsWith(embeddedFileName));

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream, Encoding.ASCII);
            var rom = Compile(reader.ReadToEnd());
            return BuildPcModules(rom, out modules);
        }

        protected void TickUntilNop(ModulePanel module) {
            var none = ControlSignalType.None.ToBitArray();
            while (true) {
                MakeTickAndWait();
                if (BitArrayHelper.EqualTo(none, module.ControlBus.PeakAll())) {
                    return;
                }
            }
        }

        protected void TickOneInstruction(ModulePanel module) {
            while (true) {
                MakeTickAndWait();
                if (ControlSignalTypeExtensions.FromBitArray(module.ControlBus.PeakAll()).HasFlag(ControlSignalType.Ic_clr))
                    return;
            }
        }

        private CsPanel BuildPcModules(BitArray[] romData, out ModulePanel modules) {
            var cp = new CpuBuilder(_testClock)
               .WithControlModule(null, true)
               .WithMemoryModule(romData, null)
               .WithRegistersModule()
               .WithAluModule()
               .BuildWithModulesAccess(out modules);

            MakeOnlyLoops();

            return cp;
        }

        private static BitArray[] Compile(string input) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var codeReader = new CodeReader(ms);
            var tokens = new Tokenizer().Tokenize(codeReader).ToList();
            var tokenReader = new TokenReader(tokens);
            var parser = new Parser();
            return parser.Parse(tokenReader);
        }
    }
}
