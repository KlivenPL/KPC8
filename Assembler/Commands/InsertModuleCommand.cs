/*using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts.Labels;
using Assembler.Readers;
using Assembler.Tokens;
using Infrastructure.BitArrays;
using System;
using System.Collections;
using System.IO;

namespace Assembler.Commands {
    internal class InsertModuleCommand : CommandBase {
        public override CommandType Type => CommandType.InsertModule;

        protected override string[] AcceptedRegions => new[] { LabelsContext.ConstRegion };

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<IdentifierToken, StringToken>(reader, out var ptrNameToken, out var filePathToken);

            var newPtrAddressToken = new NumberToken(romBuilder.NextAddress, ptrNameToken.CodePosition, ptrNameToken.LineNumber, ptrNameToken.FilePath);

            *//*if (!labelsContext.TryInsertRegionedToken(ptrNameToken.Value, newPtrAddressToken, out var errorMessage)) {
                throw ParserException.Create(errorMessage, reader.Current);
            }

            var loadedBytes = LoadBinFromFile(filePathToken);
            for (int i = 0; i < loadedBytes.Length; i++) {
                romBuilder.AddByte(loadedBytes[i]);
            }*//*
        }

        *//*private BitArray[] LoadBinFromFile(StringToken filePathToken) {

            var fileInfo = new FileInfo(Path.GetFullPath(filePathToken.Value));

            if (!fileInfo.Exists) {
                throw ParserException.Create($"File does not exist:{Environment.NewLine}\"{fileInfo.FullName}\"", filePathToken);
            }

            var bas = new BitArray[fileInfo.Length];
            using var stream = fileInfo.OpenRead();
            using var binaryReader = new BinaryReader(stream);

            for (int i = 0; i < size; i++) {
                var @byte = binaryReader.ReadByte();
                bas[i] = BitArrayHelper.FromByteLE(@byte);
            }

            return bas;
        }*//*
    }
}
*/