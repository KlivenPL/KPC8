using Assembler.DebugData;
using Assembler.Readers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assembler {
    public static class Compiler {
        public static BitArray[] CompileFromFile(string path, out IEnumerable<IDebugSymbol> debugSymbols) {
            var src = LoadSourceFile(path);
            return Compile(src, out debugSymbols);
        }

        private static string LoadSourceFile(string path) {
            var fileInfo = new FileInfo(path);
            using var stream = fileInfo.OpenRead();
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static BitArray[] Compile(string input, out IEnumerable<IDebugSymbol> debugSymbols) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var codeReader = new CodeReader(ms);
            var tokens = new Tokenizer().Tokenize(codeReader).ToList();
            var tokenReader = new TokenReader(tokens);
            var parser = new Parser();
            return parser.Parse(tokenReader, out debugSymbols);
        }
    }
}
