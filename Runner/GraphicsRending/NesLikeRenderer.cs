using Infrastructure.BitArrays;
using Runner._Infrastructure;
using Runner.Build;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Runner.GraphicsRending {
    internal class NesLikeRenderer : IKPC8Renderer {
#pragma warning disable CA1416 // Validate platform compatibility

        private const ushort SpriteSheetAddr = 0x10;
        private const ushort TilemapAddr = 0xF000;
        private const ushort AttribAddr = 0xF3C0;
        private const ushort PaletteAddr = 0xF3FC;
        private const ushort OemAddr = 0xF43D;

        private const ushort SpritesheetColumnOffset = 16;
        private const ushort SpritesheetRowOffset = 0x100;
        private const ushort SpritesheetNextByteOffset = 8;

        private readonly KPC8.Modules.Memory mem;
        private readonly Func<ushort, BitArray> ram;
        private readonly Func<ushort, BitArray> rom;

        private readonly OemSprite[] oemSprites = new OemSprite[64];
        private readonly List<OemSprite> visibleSprites = new List<OemSprite>(12);

        internal NesLikeRenderer(KPC8Build kpc) {
            this.mem = kpc.ModulePanel.Memory;
            ram = kpc.ModulePanel.Memory.GetRamAt;
            rom = kpc.ModulePanel.Memory.GetRomAt;

            for (ushort i = 0; i < 64; i++) {
                oemSprites[i] = new OemSprite(OemAddr, i, ram);
            }
        }

        public bool TryRender(out Bitmap frame) {
            frame = new Bitmap(320, 192);

            for (int i = 0; i < 64; i++) {
                if (oemSprites[i].GetLayer() != 0) {
                    oemSprites[i].CacheData();
                    visibleSprites.Add(oemSprites[i]);
                }
            }

            visibleSprites.Sort((x, y) => x.CachedLayer - y.CachedLayer);

            for (int y = 0; y < 24 * 8; y++) {
                int pixelY = y % 8;

                for (int x = 0; x < 40 * 8; x++) {
                    byte tileId = ram((ushort)(TilemapAddr + y / 8 * 40 + x / 8)).ToByteLE();

                    var pixelX = x % 8;

                    var row = tileId / 16;
                    var col = tileId % 16;

                    var mainOffset = (ushort)(SpriteSheetAddr + SpritesheetColumnOffset * col + row * SpritesheetRowOffset + pixelY);

                    byte bitA = rom(mainOffset)[pixelX] ? (byte)1 : (byte)0;
                    byte bitB = rom((ushort)(mainOffset + SpritesheetNextByteOffset))[pixelX] ? (byte)1 : (byte)0;

                    byte bgColorByte = (byte)((bitA) | (bitB << 1)); // 0, 1, 2 or 3

                    byte attribId = (byte)(y / 32 * 10 + x / 32);
                    byte paletteByte = ram((ushort)(AttribAddr + attribId)).ToByteLE();

                    var bgColor = GetColorFromPalette(bgColorByte, paletteByte);

                    Color colorToDisplay = bgColor;

                    for (int i = 0; i < visibleSprites.Count; i++) {
                        var currentSprite = visibleSprites[i];
                        if (GetSpritePixelIfVisible(currentSprite, x, y, out var spriteColor)) {
                            if (currentSprite.CachedLayer == 1) {
                                if (bgColorByte == 0) {
                                    colorToDisplay = spriteColor;
                                }
                            } else {
                                colorToDisplay = spriteColor;
                            }
                        }
                    }

                    frame.SetPixel(x, y, colorToDisplay);
                }
            }

            visibleSprites.Clear();
            return true;
        }

        private Color GetColorFromPalette(byte colorByte, byte paletteByte) {
            byte b1 = 0;
            byte b2 = 0;

            if (colorByte == 0) {
                b1 = ram(PaletteAddr + 0).ToByteLE();
                b2 = ram(PaletteAddr + 1).ToByteLE();
            } else {
                var baseAddress = PaletteAddr + 2 + paletteByte * 8 + (colorByte - 1) * 2;

                b1 = ram((ushort)(baseAddress + 0)).ToByteLE();
                b2 = ram((ushort)(baseAddress + 1)).ToByteLE();
            }

            // format: XRRRRRGG|GGGBBBBB
            //            b1       b2
            int r = (b1 & 0b01111100) >> 2;
            int g = ((b1 & 0b00000011) << 3) + ((b2 & 0b11100000) >> 5);
            int b = b2 & 0b00011111;

            // * 8 - 15 bit color (max 31) to 32 bit color (max 255)
            return Color.FromArgb(r << 3, g << 3, b << 3);
        }

        public bool GetSpritePixelIfVisible(OemSprite sprite, int x, int y, out Color color) {
            color = Color.Black;

            if (x - sprite.CachedPosX < 0 || x - sprite.CachedPosX >= 8 || y - sprite.CachedPosY < 0 || y - sprite.CachedPosY >= 8) {
                return false;
            }

            byte tileId = sprite.CachedTileId;

            var pixelX = x - sprite.CachedPosX;

            var row = tileId / 16;
            var col = tileId % 16;

            var mainOffset = (ushort)(SpriteSheetAddr + SpritesheetColumnOffset * col + row * SpritesheetRowOffset + y - sprite.CachedPosY);

            byte bitA = rom(mainOffset)[pixelX] ? (byte)1 : (byte)0;
            byte bitB = rom((ushort)(mainOffset + SpritesheetNextByteOffset))[pixelX] ? (byte)1 : (byte)0;

            byte colorByte = (byte)((bitA) | (bitB << 1)); // 0, 1, 2 or 3

            if (colorByte == 0) {
                return false;
            }

            color = GetColorFromPalette(colorByte, sprite.CachedPalette);
            return true;
        }
    }

    internal class OemSprite {
        public OemSprite(ushort oemAddress, ushort spriteId, Func<ushort, BitArray> ram) {
            ushort baseAddress = (ushort)(oemAddress + spriteId * 4);

            TileId = ram(baseAddress);
            PosX_A = ram((ushort)(baseAddress + 1));
            PosX_B = ram((ushort)(baseAddress + 2));
            PosY = ram((ushort)(baseAddress + 3));
        }

        public BitArray TileId { get; }
        public BitArray PosX_A { get; }
        public BitArray PosX_B { get; }
        public BitArray PosY { get; }

        public byte CachedTileId { get; private set; }
        public ushort CachedPosX { get; private set; }
        public byte CachedPosY { get; private set; }
        public byte CachedLayer { get; private set; }
        public byte CachedPalette { get; private set; }

        public void CacheData() {
            CachedTileId = TileId.ToByteLE();
            CachedPosX = GetPosX();
            CachedPosY = GetPosY();
            CachedLayer = GetLayer();
            CachedPalette = GetPalette();
        }

        public ushort GetPosX() {
            return (ushort)(((PosX_A.ToByteLE() & 0b00000001) << 8) | PosX_B.ToByteLE());
        }

        public byte GetPosY() {
            return PosY.ToByteLE();
        }

        public byte GetPalette() {
            return (byte)((PosX_A.ToByteLE() & 0b11100000) >> 5);
        }

        public byte GetLayer() {
            return (byte)((PosX_A.ToByteLE() & 0b00011000) >> 3);
        }
    }
}
