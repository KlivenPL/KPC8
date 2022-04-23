using Infrastructure.BitArrays;
using Runner._Infrastructure;
using Runner.Build;
using System;
using System.Collections;
using System.Drawing;

namespace Runner.GraphicsRending {
    internal class NesLikeRenderer : IKPC8Renderer {
#pragma warning disable CA1416 // Validate platform compatibility

        private const ushort SpriteSheetAddr = 0x10;
        private const ushort TileMapAddr = 0xF000;

        private const ushort SpritesheetColumnOffset = 16;
        private const ushort SpritesheetRowOffset = 0x100;
        private const ushort SpritesheetNextByteOffset = 8;

        private readonly KPC8.Modules.Memory mem;
        private readonly Func<ushort, BitArray> ram;
        private readonly Func<ushort, BitArray> rom;

        internal NesLikeRenderer(KPC8Build kpc) {
            this.mem = kpc.ModulePanel.Memory;
            ram = kpc.ModulePanel.Memory.GetRamAt;
            rom = kpc.ModulePanel.Memory.GetRomAt;
        }

        public bool TryRender(out Bitmap frame) {
            frame = new Bitmap(320, 192);

            for (int y = 0; y < 24 * 8; y++) {
                int pixelY = y % 8;

                for (int x = 0; x < 40 * 8; x++) {
                    var color = Color.Black;

                    byte tileId = ram((ushort)(TileMapAddr + y / 8 * 40 + x / 8)).ToByteLE();

                    var pixelX = x % 8;

                    var row = tileId / 16;
                    var col = tileId % 16;

                    var mainOffset = (ushort)(SpriteSheetAddr + SpritesheetColumnOffset * col + row * SpritesheetRowOffset + pixelY);

                    var bitA = rom(mainOffset)[pixelX];
                    var bitB = rom((ushort)(mainOffset + SpritesheetNextByteOffset))[pixelX];

                    if (bitA && bitB) {
                        color = Color.White;
                    } else if (bitA && !bitB) {
                        color = Color.Red;
                    } else if (!bitA && bitB) {
                        color = Color.Blue;
                    }

                    frame.SetPixel(x, y, color);
                }
            }

            return true;
        }
    }
}
