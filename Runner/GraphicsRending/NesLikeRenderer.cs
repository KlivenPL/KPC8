using Abstract;
using Infrastructure.BitArrays;
using Runner._Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Runner.GraphicsRending {
    internal class NesLikeRenderer : IKPC8Renderer {
#pragma warning disable CA1416 // Validate platform compatibility

        private const ushort SpriteSheetAddr = 0x10;
        private const ushort TilemapAddr = 0xF000;
        private const ushort AttribAddr = 0xF3C0;
        private const ushort SpritePaletteAddr = 0xF5A0;
        private const ushort BgPaletteAddr = 0xF5E0;
        private const ushort OemAddr = 0xF660;

        private const ushort SpritesheetColumnOffset = 16;
        private const ushort SpritesheetRowOffset = 0x100;
        private const ushort SpritesheetNextByteOffset = 8;

        private readonly Func<ushort, byte> ram;
        private readonly Func<ushort, byte> rom;

        private readonly OemSprite[] oemSprites = new OemSprite[64];
        private readonly List<OemSprite> visibleSprites = new List<OemSprite>(12);

        internal NesLikeRenderer(IKpcBuild kpc) {
            ram = kpc.Ram.ReadByte;
            rom = kpc.Rom.ReadByte;

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
                    byte ssTileId = ram((ushort)(TilemapAddr + y / 8 * 40 + x / 8));

                    var pixelX = x % 8;

                    var row = ssTileId / 16;
                    var col = ssTileId % 16;

                    var mainOffset = (ushort)(SpriteSheetAddr + SpritesheetColumnOffset * col + row * SpritesheetRowOffset + pixelY);

                    byte bitA = BitArrayHelper.FromByteLE(rom(mainOffset))[pixelX] ? (byte)1 : (byte)0;
                    byte bitB = BitArrayHelper.FromByteLE(rom((ushort)(mainOffset + SpritesheetNextByteOffset)))[pixelX] ? (byte)1 : (byte)0;

                    byte bgColorByte = (byte)((bitA) | (bitB << 1)); // 0, 1, 2 or 3

                    ushort tmTileId = (ushort)(y / 8 * 40 + x / 8);
                    ushort attribId = (ushort)(tmTileId / 2);
                    byte paletteByte = ram((ushort)(AttribAddr + attribId));

                    var attribHalf = tmTileId % 2 == 0;

                    if (attribHalf) {
                        paletteByte = (byte)((paletteByte & 0b11110000) >> 4);
                    } else {
                        paletteByte = (byte)(paletteByte & 0b00001111);
                    }

                    var bgColor = GetColorFromPalette(BgPaletteAddr, bgColorByte, paletteByte);

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

        private Color GetColorFromPalette(ushort paletteAddress, byte colorByte, byte paletteByte) {
            byte b1 = 0;
            byte b2 = 0;

            if (colorByte == 0) {
                b1 = ram((ushort)(paletteAddress + 0));
                b2 = ram((ushort)(paletteAddress + 1));
            } else {
                var baseAddress = paletteAddress + 2 + paletteByte * 8 + (colorByte - 1) * 2;

                b1 = ram((ushort)(baseAddress + 0));
                b2 = ram((ushort)(baseAddress + 1));
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

            byte bitA = BitArrayHelper.FromByteLE(rom(mainOffset))[pixelX] ? (byte)1 : (byte)0;
            byte bitB = BitArrayHelper.FromByteLE(rom((ushort)(mainOffset + SpritesheetNextByteOffset)))[pixelX] ? (byte)1 : (byte)0;

            byte colorByte = (byte)((bitA) | (bitB << 1)); // 0, 1, 2 or 3

            if (colorByte == 0) {
                return false;
            }

            color = GetColorFromPalette(SpritePaletteAddr, colorByte, sprite.CachedPalette);
            return true;
        }
    }

    internal class OemSprite {
        private readonly ushort baseAddress;
        private readonly Func<ushort, byte> ram;

        public OemSprite(ushort oemAddress, ushort spriteId, Func<ushort, byte> ram) {
            baseAddress = (ushort)(oemAddress + spriteId * 4);
            this.ram = ram;
        }

        public byte TileId => ram(baseAddress);
        public byte PosX_A => ram((ushort)(baseAddress + 1));
        public byte PosX_B => ram((ushort)(baseAddress + 2));
        public byte PosY => ram((ushort)(baseAddress + 3));

        public byte CachedTileId { get; private set; }
        public ushort CachedPosX { get; private set; }
        public byte CachedPosY { get; private set; }
        public byte CachedLayer { get; private set; }
        public byte CachedPalette { get; private set; }

        public void CacheData() {
            CachedTileId = TileId;
            CachedPosX = GetPosX();
            CachedPosY = GetPosY();
            CachedLayer = GetLayer();
            CachedPalette = GetPalette();
        }

        public ushort GetPosX() {
            return (ushort)(((PosX_A & 0b00000001) << 8) | PosX_B);
        }

        public byte GetPosY() {
            return PosY;
        }

        public byte GetPalette() {
            return (byte)((PosX_A & 0b11100000) >> 5);
        }

        public byte GetLayer() {
            return (byte)((PosX_A & 0b00011000) >> 3);
        }
    }
}
