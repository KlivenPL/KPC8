using Assembler;
using Assembler._Infrastructure;
using Assembler.DebugData;
using Infrastructure.BitArrays;
using Player.Contexts;
using System.Collections;

namespace Player.Loaders {
    internal class ProgramLoader {
        private readonly ProgramContext programContext;

        public ProgramLoader(ProgramContext programContext) {
            this.programContext = programContext;
        }

        public bool TryGetCompiledProgram(out BitArray[] program, out string compileErrors) {
            program = null;
            compileErrors = null;

            if (!programContext.IsSourceFileSelected && !programContext.IsRomFileSelected) {
                if (!programContext.TryLoadSourceOrRomFile()) {
                    return false;
                }
            }

            if (programContext.IsRomFileSelected) {
                program = LoadRomFromRomFile();
                return true;
            }

            return TryCompileSourceFile(out program, out _, out compileErrors);
        }

        public bool TryGetCompiledProgramWithDebugSymbols(out string sourceFilePath, out BitArray[] program, out IEnumerable<IDebugSymbol> debugSymbols, out string compileErrors) {
            program = null;
            debugSymbols = null;
            compileErrors = null;
            sourceFilePath = null;

            if (!programContext.IsSourceFileSelected) {
                if (!programContext.TryLoadSourceFile()) {
                    return false;
                }
            }

            sourceFilePath = programContext.SourceFile.FullName;

            return TryCompileSourceFile(out program, out debugSymbols, out compileErrors);
        }

        private bool TryCompileSourceFile(out BitArray[] program, out IEnumerable<IDebugSymbol> debugSymbols, out string compileErrors) {
            program = null;
            debugSymbols = null;
            compileErrors = null;

            if (string.IsNullOrWhiteSpace(programContext.SourceFile?.FullName)) {
                return false;
            }

            try {
                program = Compiler.CompileFromFile(programContext.SourceFile.FullName, out debugSymbols);
                return true;
            } catch (Exception ex) {
                compileErrors = ex.Message;
                if (ex is not ParserException && ex is not TokenizerException) {
                    compileErrors += $"{Environment.NewLine}{ex.StackTrace}";
                }
                return false;
            }
        }

        private BitArray[] LoadRomFromRomFile() {
            BitArray[] bas = new BitArray[ushort.MaxValue + 1];
            using var stream = programContext.RomFile.OpenRead();
            using var binaryReader = new BinaryReader(stream);

            for (int i = 0; i < ushort.MaxValue + 1; i++) {
                var @byte = binaryReader.ReadByte();
                bas[i] = BitArrayHelper.FromByteLE(@byte);
            }

            return bas;
        }
    }
}
