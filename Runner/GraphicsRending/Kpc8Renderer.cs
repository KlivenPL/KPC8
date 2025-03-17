using Abstract;
using Abstract.Components;

//using Infrastructure.BitArrays;
using Runner._Infrastructure;
using Runner._Infrastructure.UnsafeExtensions;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Runner.GraphicsRending {
    internal class Kpc8Renderer : IKPC8Renderer {
#pragma warning disable CA1416 // Validate platform compatibility
        private readonly byte[] _rawRom;
        private readonly byte[] _rawRam;

        private const ushort SsAddr = 0x10;
        private const ushort GrsAddr = 0xCCE0;
        private const ushort TmAddr = 0xCD00;
        private const ushort TssAddr = 0xDD00;
        private const ushort TpaAddr = 0xE100;
        private const ushort PltAddr = 0xF100;
        private const ushort FgsAddr = 0xF700;

        private DateTime _lastUpdate = DateTime.Now;
        private double time;

        internal Kpc8Renderer(IKpcBuild kpc) {
            _rawRom = ((IRawMemory)kpc.Rom).RawBytes;
            _rawRam = ((IRawMemory)kpc.Ram).RawBytes;
        }

        public unsafe bool TryRender(out Bitmap frame) {
            var grs = _rawRam.ReadStruct<GpuRegisters>(GrsAddr, 0);

            var tilemapSizeX = grs.TilemapSizeX;
            var tilemapSizeY = grs.TilemapSizeY;
            var cameraSizeX = grs.CameraSizeX;
            var cameraSizeY = grs.CameraSizeY;
            var cameraScrollX = grs.CameraScrollX;
            var cameraScrollY = grs.CameraScrollY;

            if (cameraSizeX == 0 || cameraSizeY == 0) {
                frame = new Bitmap(1, 1);
                return false;
            }

            double deltaTime = (DateTime.Now - _lastUpdate).TotalMilliseconds / 1000f;
            double freq = 0.25f;
            time += deltaTime;

            var scale = 512f;
            var sinT = (double)Math.Sin(2 * Math.PI * freq * time);
            var cosT = (double)Math.Cos(2 * Math.PI * freq * time);

            cameraScrollX = (short)(sinT * scale);
            cameraScrollY = (short)(cosT * scale);

            var ss = _rawRom.ReadStruct<Spritesheet>(SsAddr, 0);
            var tm = _rawRam.ReadStruct<Tilemaps>(TmAddr, 0);
            var tss = _rawRam.ReadStruct<TileSsSelectors>(TssAddr, 0);
            var tpa = _rawRam.ReadStruct<TilePaletteAssignments>(TpaAddr, 0);
            var plt = _rawRam.ReadStruct<Palettes>(PltAddr, 0);

            frame = new Bitmap(cameraSizeX, cameraSizeY);

            for (int cY = 0; cY < cameraSizeY; cY++) {
                var tmPixelY = Mod(cY + cameraScrollY, tilemapSizeY * 8);
                var tmY = tmPixelY / 8;

                for (int cX = 0; cX < cameraSizeX; cX++) {
                    var tmPixelX = Mod(cX + cameraScrollX, tilemapSizeX * 8);
                    var tmX = tmPixelX / 8;

                    var spriteId = tm.GetSpriteId(tmX, tmY, tilemapSizeX);
                    var ssId = tss.GetSpritesheetId(tmX, tmY);
                    var colorId = ss.GetPixelColorId(ssId, spriteId, tmPixelX % 8, tmPixelY % 8);

                    Color color;
                    if (colorId == 0) {
                        color = ColorBytesToColor(grs.bgColorB1, grs.bgColorB2);
                    } else {
                        var paletteId = tpa.GetTilePaletteId(tmX, tmY);
                        color = plt.GetColor(colorId, paletteId);
                    }

                    frame.SetPixel(cX, cY, color);
                }
            }

            _lastUpdate = DateTime.Now;
            return true;
        }

        //private bool GetSpritePixelIfVisible(OemSprite sprite, int x, int y, out Color color) {
        //    color = Color.Black;

        //    if (x - sprite.CachedPosX < 0 || x - sprite.CachedPosX >= 8 || y - sprite.CachedPosY < 0 || y - sprite.CachedPosY >= 8) {
        //        return false;
        //    }

        //    byte spriteId = sprite.CachedSpriteId;

        //    var pixelX = x - sprite.CachedPosX;

        //    var row = spriteId / 16;
        //    var col = spriteId % 16;

        //    var mainOffset = (ushort)(SpriteSheetAddr + SpritesheetColumnOffset * col + row * SpritesheetRowOffset + y - sprite.CachedPosY);

        //    byte bitA = (byte)(((rom(mainOffset)) >> (7 - pixelX)) & 1);
        //    byte bitB = (byte)(((rom((ushort)(mainOffset + SpritesheetNextByteOffset))) >> (7 - pixelX)) & 1);

        //    byte colorByte = (byte)((bitA) | (bitB << 1)); // 0, 1, 2 or 3

        //    if (colorByte == 0) {
        //        return false;
        //    }

        //    color = GetColorFromPalette(SpritePaletteAddr, colorByte, sprite.CachedPalette);
        //    return true;
        //}

        //private Color GetColorFromPalette(ushort paletteAddress, byte colorByte, byte paletteByte) {
        //    byte b1 = 0;
        //    byte b2 = 0;

        //    if (colorByte == 0) {
        //        b1 = ram((ushort)(paletteAddress + 0));
        //        b2 = ram((ushort)(paletteAddress + 1));
        //    } else {
        //        var baseAddress = paletteAddress + 2 + paletteByte * 8 + (colorByte - 1) * 2;

        //        b1 = ram((ushort)(baseAddress + 0));
        //        b2 = ram((ushort)(baseAddress + 1));
        //    }

        //    // format: XRRRRRGG|GGGBBBBB
        //    //            b1       b2
        //    int r = (b1 & 0b01111100) >> 2;
        //    int g = ((b1 & 0b00000011) << 3) + ((b2 & 0b11100000) >> 5);
        //    int b = b2 & 0b00011111;

        //    // * 8 - 15 bit color (max 31) to 32 bit color (max 255)
        //    return Color.FromArgb(r << 3, g << 3, b << 3);
        //}

        //private class OemSprite {
        //    private readonly ushort baseAddress;
        //    private readonly Func<ushort, byte> ram;

        //    public OemSprite(ushort oemAddress, int spriteId, Func<ushort, byte> ram) {
        //        baseAddress = (ushort)(oemAddress + spriteId * 4);
        //        this.ram = ram;
        //    }

        //    public byte SpriteId => ram(baseAddress);
        //    public byte PosX_A => ram((ushort)(baseAddress + 1));
        //    public byte PosX_B => ram((ushort)(baseAddress + 2));
        //    public byte PosY => ram((ushort)(baseAddress + 3));

        //    public byte CachedSpriteId { get; private set; }
        //    public ushort CachedPosX { get; private set; }
        //    public byte CachedPosY { get; private set; }
        //    public byte CachedLayer { get; private set; }
        //    public byte CachedPalette { get; private set; }

        //    public void CacheData() {
        //        CachedSpriteId = SpriteId;
        //        CachedPosX = GetPosX();
        //        CachedPosY = GetPosY();
        //        CachedLayer = GetLayer();
        //        CachedPalette = GetPalette();
        //    }

        //    public ushort GetPosX() {
        //        return (ushort)(((PosX_A & 0b00000001) << 8) | PosX_B);
        //    }

        //    public byte GetPosY() {
        //        return PosY;
        //    }

        //    public byte GetPalette() {
        //        return (byte)((PosX_A & 0b11100000) >> 5);
        //    }

        //    public byte GetLayer() {
        //        return (byte)((PosX_A & 0b00011000) >> 3);
        //    }
        //}


        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct GpuRegisters {
            public byte bgColorB1;
            public byte bgColorB2;

            private byte tilemapSizeX;
            private byte tilemapSizeY;

            private ushort cameraSizeX;
            private ushort cameraSizeY;

            private short cameraScrollX;
            private short cameraScrollY;

            public readonly byte TilemapSizeX => (byte)(tilemapSizeX & 63);
            public readonly byte TilemapSizeY => (byte)(tilemapSizeY & 63);
            public readonly ushort CameraSizeX => (ushort)(SwapUShort(cameraSizeX) /*& 495u*/);
            public readonly ushort CameraSizeY => (ushort)(SwapUShort(cameraSizeY) /*& 495u*/);
            public readonly short CameraScrollX => SwapShort(cameraScrollX);
            public readonly short CameraScrollY => SwapShort(cameraScrollY);
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct Spritesheet {
            public const int SpriteSize = 16;         // 16 bytes per sprite (8 bytes low, 8 bytes high)
            public const int SpriteWidth = 8;         // 8 pixels per sprite row
            public const int SpriteHeight = 8;        // 8 pixels per sprite column
            public const int SpritesPerRow = 16;      // Pattern table grid: 16 columns x 16 rows = 256 sprites
            public const int SpritesPerColumn = 16;
            public const int SpritesheetSize = SpriteSize * SpritesPerRow * SpritesPerColumn; // 4096 bytes

            // The fixed buffer stores the entire pattern table inline.
            private fixed byte Data[SpritesheetSize];

            /// <summary>
            /// Retrieves the color index (0–3) for the specified sprite and pixel within that sprite.
            /// </summary>
            /// <param name="spriteColumn">Sprite column (0–15) in the pattern table.</param>
            /// <param name="spriteRow">Sprite row (0–15) in the pattern table.</param>
            /// <param name="pixelX">Pixel column (0–7) within the sprite.</param>
            /// <param name="pixelY">Pixel row (0–7) within the sprite.</param>
            /// <returns>The 2-bit color index for that pixel.</returns>
            public byte GetPixelColorId(int spritesheetId, int spriteId, int pixelX, int pixelY) {
                var row = spriteId / 16;
                var col = spriteId % 16;

                var mainOffset = (ushort)(spritesheetId * 4096 + 16 * col + row * 0x100 + pixelY);

                fixed (byte* pData = Data) {
                    byte bitA = (byte)((pData[mainOffset] >> (7 - pixelX)) & 1);
                    byte bitB = (byte)((pData[(ushort)(mainOffset + 8)] >> (7 - pixelX)) & 1);
                    return (byte)((bitA) | (bitB << 1)); // 0, 1, 2 or 3
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct Tilemaps {
            public const ushort PixelSizeX = 512;
            public const ushort PixelSizeY = 512;

            public const byte MaxColumnsX = 64;
            public const byte MaxRowsY = 64;

            private fixed byte Data[MaxColumnsX * MaxRowsY];

            public byte GetSpriteId(int tmX, int tmY, int sizeX) {
#if DEBUG
                if (tmY * MaxColumnsX + tmX >= MaxColumnsX * MaxRowsY) {
                    throw new ArgumentException($"[{nameof(Tilemaps)}] TM size exceeded");
                }
#endif

                fixed (byte* pData = Data) {
                    return pData[(ushort)tmY * sizeX + (ushort)tmX];
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct TileSsSelectors {
            public const byte ColumnsX = 64;
            public const byte RowsY = 64;
            public const byte SelectorsPerColumn = 4;

            private const byte RowSize = ColumnsX / SelectorsPerColumn;

            private fixed byte Data[RowSize * RowsY];

            public byte GetSpritesheetId(int tmX, int tmY) {
                var selectorX = tmX / SelectorsPerColumn;
                int selectorOffset = (selectorX / 4) * 2;

#if DEBUG
                if (tmY * ColumnsX + tmX >= ColumnsX * RowsY) {
                    throw new ArgumentException($"[{nameof(TileSsSelectors)}] TM size exceeded");
                }

                if (selectorX > 15) {
                    throw new ArgumentException($"[{nameof(TileSsSelectors)}] selectorX > 15 ({selectorX})");
                }

                if (selectorOffset % 2 != 0 || selectorOffset > 6) {
                    throw new ArgumentException($"[{nameof(TileSsSelectors)}] invalid selectorOffset: ({selectorOffset})");
                }
#endif

                fixed (byte* pData = Data) {
                    return (byte)((pData[tmY * RowSize + selectorX] >> selectorOffset) & 3);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct TilePaletteAssignments {
            public const byte Columns = 64;
            public const byte Rows = 64;
            private fixed byte Data[Rows * Columns];

            public byte GetTilePaletteId(int tmX, int tmY) {
#if DEBUG
                if (tmY * Rows + tmX >= Columns * Rows) {
                    throw new ArgumentException($"[{nameof(TilePaletteAssignments)}] TM size exceeded {tmX}, {tmY}");
                }
#endif
                fixed (byte* pData = Data) {
                    return pData[tmY * Rows + tmX];
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct Palettes {
            public const byte Columns = 3;
            public const ushort Rows = 256;
            public const byte ColorSize = 2;

            private fixed byte Data[Columns * ColorSize * Rows];

            public Color GetColor(int colorId, int paletteId) {
#if DEBUG
                if (paletteId > 255) {
                    throw new ArgumentException($"[{nameof(Palettes)}] paletteId size exceeded {paletteId}");
                }

                if (colorId > 3 || colorId < 1) {
                    throw new ArgumentException($"[{nameof(Palettes)}] colorId size exceeded {colorId}");
                }
#endif

                var colorIndex = colorId - 1;
                fixed (byte* pData = Data) {
                    var b1 = pData[paletteId * ColorSize + colorIndex * 2];
                    var b2 = pData[paletteId * ColorSize + colorIndex * 2 + 1];

                    return ColorBytesToColor(b1, b2);
                }
            }
        }

        private static Color ColorBytesToColor(byte b1, byte b2) {
            // format: XRRRRRGG|GGGBBBBB
            //            b1       b2
            int r = (b1 & 0b01111100) >> 2;
            int g = ((b1 & 0b00000011) << 3) + ((b2 & 0b11100000) >> 5);
            int b = b2 & 0b00011111;

            // * 8 - 15 bit color (max 31) to 32 bit color (max 255)
            return Color.FromArgb(r << 3, g << 3, b << 3);
        }

        private static ushort SwapUShort(ushort value) {
            return (ushort)((value >> 8) | (value << 8));
        }

        private static short SwapShort(short value) {
            return (short)((value >> 8) | (value << 8));
        }

        private static int Mod(int a, int b) {
            return ((a % b) + b) % b;
        }
    }
}
