using Assembler.Readers;
using Infrastructure.BitArrays;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace Assembler {
    class Program {
        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.Error.WriteLine("Input file not given");
                return;
            }

            var outputFilePath = $"{Path.GetFileNameWithoutExtension(args[0])}.kpcrom";

            if (args.Length == 2) {
                outputFilePath = args[1];
            }

            if (!File.Exists(args[0])) {
                Console.Error.WriteLine($"File {args[0]} does not exist");
                return;
            }

            try {
                Process(args[0], outputFilePath);
            } catch (Exception ex) {
                Console.Error.WriteLine(ex.Message);
            }
        }

        private static void Process(string inputFilePath, string outputFilePath) {
            var src = LoadSourceFile(inputFilePath);
            var compiled = Compile(src);
            SaveToBinaryFile(compiled, outputFilePath);
        }

        private static string LoadSourceFile(string path) {
            var fileInfo = new FileInfo(path);
            using var stream = fileInfo.OpenRead();
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static BitArray[] Compile(string input) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var codeReader = new CodeReader(ms);
            var tokens = new Tokenizer().Tokenize(codeReader).ToList();
            var tokenReader = new TokenReader(tokens);
            var parser = new Parser();
            return parser.Parse(tokenReader);
        }

        private static void SaveToBinaryFile(BitArray[] bas, string path) {
            var fileInfo = new FileInfo(path);
            using var stream = fileInfo.OpenWrite();
            using var binaryWriter = new BinaryWriter(stream);

            for (int i = 0; i < bas.Length; i++) {
                var @byte = bas[i].ToByteLE();
                binaryWriter.Write(@byte);
            }
        }
    }
}
