using Infrastructure.BitArrays;
using Microsoft.Extensions.DependencyInjection;
using Player.BitArrayViewer;
using Player.Gui.Renderers;
using System.Collections;

namespace Player {
    public partial class KPC8Player : Form {
        private readonly IServiceProvider provider;
        public KPC8Player(IServiceProvider provider) {
            InitializeComponent();
            this.provider = provider;
            mnuToolBar.Renderer = new CustomToolStripRenderer();

            new Thread(() => {
                Random random = new Random();
                Bitmap bitmap = new Bitmap(250, 120);
                //pictureBox1.Image = bitmap;
                while (true) {

                    lock (bitmap) {

                        for (int y = 0; y < bitmap.Height; y++) {
                            for (int x = 0; x < bitmap.Width; x++) {
                                bitmap.SetPixel(x, y, Color.FromArgb(random.Next(1, 255), random.Next(1, 255), random.Next(1, 255)));
                            }
                        }
                        pictureBox1.Image?.Dispose();
                        pictureBox1.Image = (Bitmap)bitmap.Clone();
                    }
                    Thread.Sleep(1000 / 120);
                }
            }).Start();
        }

        private void loadRomBtn_Click(object sender, EventArgs e) {
            var bavForm = provider.GetRequiredService<BitArrayViewerForm>();

            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Filter = "KPC8 ROM|*.kpcrom|All files|*.*";
            if (theDialog.ShowDialog() == DialogResult.OK) {
                var testData = LoadRomFromBinaryFile(theDialog.FileName);
                bavForm.Initialize(new BitArrayViewer.BitArrayViewerFormParameters(
                    title: "Test title",
                    bitArraySource: () => testData,
                    Action
                    ));
                bavForm.Show();
            }

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


        private static BitArray[] LoadRomFromBinaryFile(string path) {
            var fileInfo = new FileInfo(path);
            BitArray[] bas = new BitArray[ushort.MaxValue + 1];
            using var stream = fileInfo.OpenRead();
            using var binaryReader = new BinaryReader(stream);

            for (int i = 0; i < ushort.MaxValue + 1; i++) {
                var @byte = binaryReader.ReadByte();
                bas[i] = BitArrayHelper.FromByteLE(@byte);
            }

            return bas;
        }

        Image testImg;
        System.Windows.Forms.Timer timer;
        private void pictureBox1_Click(object sender, EventArgs e) {

        }

        private void Timer_Tick(object? sender, EventArgs e) {
            throw new NotImplementedException();
        }
    }
}