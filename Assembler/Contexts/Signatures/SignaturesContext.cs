using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Assembler.Contexts.Signatures {
    internal static class SignaturesContext {
#if DEBUG

        private readonly static List<KpcSignature> signatures = new List<KpcSignature>();

        public static void AddSignature(KpcSignature signature) {
            signatures.Add(signature);
        }

        public static void AddSignatures(IEnumerable<KpcSignature> signatures) {
            SignaturesContext.signatures.AddRange(signatures);
        }

        public static string DumpToJson() {
            var json = JsonConvert.SerializeObject(signatures, jsonSerializerSettings);
            return json;
        }

        private static readonly JsonSerializerSettings jsonSerializerSettings = new() {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.None,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() },
        };

#endif
    }
}
