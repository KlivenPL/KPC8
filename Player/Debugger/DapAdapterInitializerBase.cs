using Assembler;
using Assembler.DebugData;
using Runner.Configuration;
using System.Collections;

namespace Player.Debugger {
    internal abstract class DapAdapterInitializerBase {


        protected bool TryLoadKPC8Configuration(string configurationFilePath, BitArray[] program, out KPC8Configuration kpc8Configuration, out string kpcConfigErrorMessage) {
            // TODO
            kpc8Configuration = new KPC8Configuration {
                ClockMode = Components.Clocks.ClockMode.Manual,
                ClockPeriodInTicks = 3,
                RomData = program,
            };
            kpcConfigErrorMessage = null;
            return true;
        }

        protected bool TryCompile(string sourceFilePath, out BitArray[] program, out IEnumerable<IDebugSymbol> debugSymbols, out string compileErrorMessage) {
            debugSymbols = null;
            compileErrorMessage = null;
            program = null;

            try {
                program = Compiler.CompileFromFile(sourceFilePath, out debugSymbols);
                return true;
            } catch (Exception ex) {
                compileErrorMessage = ex.Message;
            }

            return false;
        }
    }
}
