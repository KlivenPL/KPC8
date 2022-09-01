using System.Collections.Generic;

namespace DebugAdapter.Configuration {
    public class DapAdapterConfiguration {
        // public string SourceFilePath { get; init; }

        public IEnumerable<string> SourceFilePaths { get; init; }
    }
}
