using Assembler.DebugData;
using Assembler.Readers;
using Infrastructure.BitArrays;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assembler {
    internal class Program {

        private static readonly JsonSerializerSettings jsonSerializerSettings = new() {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.All
        };

        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.Error.WriteLine("Input file not given");
                return;
            }

            var outputFilePath = $"{Path.GetFileNameWithoutExtension(args[0])}.kpcrom";
            var outputDebugSymbolsFilePath = $"{Path.GetFileNameWithoutExtension(args[0])}.kpcdbg";

            if (args.Length == 2) {
                outputFilePath = args[1];
            }

            if (args.Length == 3) {
                outputDebugSymbolsFilePath = args[2];
            }

            if (!File.Exists(args[0])) {
                Console.Error.WriteLine($"File {args[0]} does not exist");
                return;
            }

            try {
                Process(args[0], outputFilePath, outputDebugSymbolsFilePath, out IEnumerable<IDebugSymbol> debugSymbols);
            } catch (Exception ex) {
                Console.Error.WriteLine(ex.Message);
            }
        }

        private static void Process(string inputFilePath, string outputFilePath, string outputDebugSymbolsFilePath, out IEnumerable<IDebugSymbol> debugSymbols) {
            var src = LoadSourceFile(inputFilePath);
            var compiled = Compile(inputFilePath, src, out debugSymbols);
            SaveToBinaryFile(compiled, outputFilePath);
            SaveDebugSymbolsToJson(debugSymbols, outputDebugSymbolsFilePath);
        }

        private static string LoadSourceFile(string path) {
            var fileInfo = new FileInfo(path);
            using var stream = fileInfo.OpenRead();
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static BitArray[] Compile(string path, string input, out IEnumerable<IDebugSymbol> debugSymbols) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var codeReader = new CodeReader(ms, path);
            var tokens = new Tokenizer().Tokenize(codeReader).ToList();
            var tokenReader = new TokenReader(tokens);
            var parser = new Parser();
            return parser.Parse(tokenReader, out debugSymbols);
        }

        private static void SaveToBinaryFile(BitArray[] bas, string path) {
            var fileInfo = new FileInfo(path);

            if (fileInfo.Exists) {
                fileInfo.Delete();
            }

            using var stream = fileInfo.OpenWrite();
            using var binaryWriter = new BinaryWriter(stream);

            for (int i = 0; i < bas.Length; i++) {
                var @byte = bas[i].ToByteLE();
                binaryWriter.Write(@byte);
            }
        }

        private static void SaveDebugSymbolsToJson(IEnumerable<IDebugSymbol> symbols, string path) {
            var json = JsonConvert.SerializeObject(symbols, jsonSerializerSettings);
            var fileInfo = new FileInfo(path);

            if (fileInfo.Exists) {
                fileInfo.Delete();
            }

            using var stream = fileInfo.OpenWrite();
            using var writer = new StreamWriter(stream);
            writer.Write(json);
        }
    }
}
