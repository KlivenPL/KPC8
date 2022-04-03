using Infrastructure.BitArrays;
using System.Text;

namespace Player.BitArrayViewer {
    public partial class BitArrayViewerForm : Form {
        private BitArrayViewerFormParameters parameters;
        private string[] cachedValues;

        public BitArrayViewerForm() {
            InitializeComponent();
        }

        public void Initialize(BitArrayViewerFormParameters parameters) {
            this.parameters = parameters;
            this.Text = parameters.Title;
            parameters.SourceChangedEvent += OnDataSourceChanged;
            cachedValues = new string[parameters.GetBitArraySource().Length];

            CacheValues();
            InitializeMemoryView();
        }

        private void InitializeMemoryView() {
            for (int i = 0x0; i <= 0xf; i++) {
                memoryView.Columns.Add(i.ToString("X"), i.ToString("X"));
            }
            memoryView.Columns.Add("ASCII", "ASCII");

            memoryView.VirtualMode = true;
            memoryView.ReadOnly = true;
            memoryView.RowCount = parameters.GetBitArraySource().Length / 16;
            memoryView.ColumnCount = 18;
            memoryView.CellValueNeeded += MemoryView_CellValueNeeded;

            memoryView.AllowUserToResizeColumns = false;
            memoryView.AllowUserToResizeRows = false;
            memoryView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader;
            memoryView.AutoResizeColumns();
            memoryView.Columns[17].MinimumWidth = 150;
        }

        private void MemoryView_CellValueNeeded(object? sender, DataGridViewCellValueEventArgs e) {
            if (e.ColumnIndex == 0) {
                e.Value = e.RowIndex.ToString("X4");
                return;
            }

            if (e.ColumnIndex == 17) {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < 16; i++) {
                    sb.Append(Encoding.ASCII.GetString(new[] { byte.Parse(cachedValues[e.RowIndex * 16 + i] ?? "0") }));
                }

                e.Value = sb.ToString();
                return;
            }

            e.Value = cachedValues[e.RowIndex * 16 + e.ColumnIndex - 1];
        }

        private void OnDataSourceChanged() {
            CacheValues();
            memoryView.Refresh();
        }

        private void CacheValues() {
            var ba = parameters.GetBitArraySource();

            for (int i = 0; i < ba.Length; i++) {
                cachedValues[i] = BitArrayHelper.ToByteLE(ba[i]).ToString();
            }
        }
    }
}
