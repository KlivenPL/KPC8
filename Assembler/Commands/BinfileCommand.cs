using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts.Labels;
using Assembler.Readers;
using Assembler.Tokens;
using Infrastructure.BitArrays;
using System;
using System.Collections;
using System.IO;

namespace Assembler.Commands {
    internal class BinfileCommand : CommandBase {
        public override CommandType Type => CommandType.Binfile;

        protected override CommandAllowedIn AcceptedRegions => CommandAllowedIn.ConstRegion;

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<IdentifierToken, NumberToken, StringToken>(reader, out var ptrNameToken, out var sizeToken, out var filePathToken);

            var newPtrAddressToken = new NumberToken(romBuilder.NextAddress, ptrNameToken.CodePosition, ptrNameToken.LineNumber, ptrNameToken.FilePath);

            if (!TryInsertToken(ptrNameToken.Value, newPtrAddressToken, out var errorMessage)) {
                throw ParserException.Create(errorMessage, reader.Current);
            }

            var loadedBytes = LoadBinFromFile(filePathToken, sizeToken);
            for (int i = 0; i < loadedBytes.Length; i++) {
                romBuilder.AddByte(loadedBytes[i]);
            }
        }

        private BitArray[] LoadBinFromFile(StringToken filePathToken, NumberToken sizeToken) {

            var fileInfo = new FileInfo(Path.GetFullPath(filePathToken.Value));

            if (!fileInfo.Exists) {
                throw ParserException.Create($"File does not exist:{Environment.NewLine}\"{fileInfo.FullName}\"", filePathToken);
            }

            if (sizeToken.Value > fileInfo.Length) {
                throw ParserException.Create($"Size ({sizeToken.Value}) must be <= than file size ({fileInfo.Length})", sizeToken);
            }

            var size = sizeToken.Value;

            var bas = new BitArray[size];
            using var stream = fileInfo.OpenRead();
            using var binaryReader = new BinaryReader(stream);

            for (int i = 0; i < size; i++) {
                var @byte = binaryReader.ReadByte();
                bas[i] = BitArrayHelper.FromByteLE(@byte);
            }

            return bas;
        }
    }
}
