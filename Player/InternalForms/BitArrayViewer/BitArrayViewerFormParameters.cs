using System.Collections;

namespace Player.InternalForms.BitArrayViewer {
    public class BitArrayViewerFormParameters {

#nullable disable
        public BitArrayViewerFormParameters(string title, Func<BitArray[]> bitArraySource, Action sourceChangedEvent) {
            Title = title;
            GetBitArraySource = bitArraySource;
            SourceChangedEvent += sourceChangedEvent;
        }

        public string Title { get; }
        public Func<BitArray[]> GetBitArraySource { get; }
        public event Action SourceChangedEvent;
    }
}
