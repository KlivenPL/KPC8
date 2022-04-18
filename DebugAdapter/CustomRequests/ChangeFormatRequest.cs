using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using Runner._Infrastructure;

namespace DebugAdapter.CustomRequests {
    internal class ChangeFormatRequestArguments : DebugRequestArguments {
        public DebugValueFormat Format { get; set; }
    }

    internal class ChangeFormatRequest : DebugRequest<ChangeFormatRequestArguments> {
        public ChangeFormatRequest(string command) : base(command) {

        }

        public ChangeFormatRequest() : this(nameof(ChangeFormatRequest)) {

        }
    }
}
