using Assembler.DebugData;
using Assembler.Readers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assembler {
    public static class Compiler {
        public static BitArray[] CompileFromFile(string path, out IEnumerable<IDebugSymbol> debugSymbols) {
            var src = LoadSourceFile(path, out var fileDirectory);

            var defaultPath = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(fileDirectory.FullName);

            var program = Compile(src, out debugSymbols);

            Directory.SetCurrentDirectory(defaultPath);
            return program;
        }

        private static string LoadSourceFile(string path, out DirectoryInfo fileDirectory) {
            var fileInfo = new FileInfo(path);

            if (!fileInfo.Exists) {
                throw new Exception($"File {path} does not exist");
            }

            fileDirectory = fileInfo.Directory;
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
