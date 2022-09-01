using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts.Labels;
using Assembler.Contexts.Regions;
using Assembler.Readers;
using Assembler.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assembler.Commands {
    internal class InsertModuleCommand : CommandBase {
        public override CommandType Type => CommandType.InsertModule;

        protected override CommandAllowedIn AcceptedRegions => CommandAllowedIn.ConstRegion;

        protected override void PreParseInner(TokenReader reader, IRegion region) {
            ParseParameters<StringToken>(reader, out var filePathToken);

            var constRegion = (ConstRegion)region;
            constRegion.InsertModule(TokenizeFile(filePathToken));
        }

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<StringToken>(reader, out var _);
        }

        private List<IToken> TokenizeFile(StringToken filePathToken) {

            var fileInfo = new FileInfo(Path.GetFullPath(filePathToken.Value));

            if (!fileInfo.Exists) {
                throw ParserException.Create($"Could not insert module: file does not exist:{Environment.NewLine}\"{fileInfo.FullName}\"", filePathToken);
            }

            string input;

            try {
                using var stream = fileInfo.OpenRead();
                using var reader = new StreamReader(stream);
                input = reader.ReadToEnd();
            } catch (Exception ex) {
                throw ParserException.Create($"Could not insert module:{Environment.NewLine}\"{ex.Message}\"", filePathToken);

            }

            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var codeReader = new CodeReader(ms, fileInfo.FullName);
            return new Tokenizer().Tokenize(codeReader).ToList();
        }
    }
}
