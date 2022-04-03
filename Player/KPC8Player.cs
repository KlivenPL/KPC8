using Infrastructure.BitArrays;
using Microsoft.Extensions.DependencyInjection;
using Player.BitArrayViewer;
using System.Collections;

namespace Player {
    public partial class KPC8Player : Form {
        private readonly IServiceProvider provider;

        public KPC8Player(IServiceProvider provider) {
            InitializeComponent();
            this.provider = provider;
        }

        private void loadRomBtn_Click(object sender, EventArgs e) {
            var bavForm = provider.GetRequiredService<BitArrayViewerForm>();
            var testData = TestBitArraySource().ToArray();
            bavForm.Initialize(new BitArrayViewer.BitArrayViewerFormParameters(
                title: "Test title",
                bitArraySource: () => testData,
                Action
                ));
            bavForm.Show();

        }
        private static Action Action { get; set; }
        static IEnumerable<BitArray> TestBitArraySource() {
            Random random = new Random();

            yield return BitArrayHelper.FromByteLE((byte)'a');
            yield return BitArrayHelper.FromByteLE((byte)'b');
            yield return BitArrayHelper.FromByteLE((byte)'c');
            yield return BitArrayHelper.FromByteLE((byte)'d');
            yield return BitArrayHelper.FromByteLE((byte)'e');
            yield return BitArrayHelper.FromByteLE((byte)'f');
            yield return BitArrayHelper.FromByteLE((byte)'g');
            yield return BitArrayHelper.FromByteLE((byte)'h');

            for (int i = 0; i < ushort.MaxValue - 8; i++) {
                yield return new BitArray(new bool[] {
                    random.Next(0, 2) == 1 ? true : false,
                    random.Next(0, 2) == 1 ? true : false,
                    random.Next(0, 2) == 1 ? true : false,
                    random.Next(0, 2) == 1 ? true : false,
                    random.Next(0, 2) == 1 ? true : false,
                    random.Next(0, 2) == 1 ? true : false,
                    random.Next(0, 2) == 1 ? true : false,
                    random.Next(0, 2) == 1 ? true : false,
                });
            }
        }
    }
}