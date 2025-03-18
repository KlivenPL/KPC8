using Abstract;
using Abstract.Components;
using OpenTK.Graphics.OpenGL;
using Runner._Infrastructure;
using Runner._Infrastructure.UnsafeExtensions;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

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

        // OpenGL texture info.
        private int textureId = -1;
        private int textureWidth;
        private int textureHeight;

        // Double-buffered pixel data.
        private byte[] frontBuffer;
        private byte[] backBuffer;
        private readonly object bufferLock = new object();

        // Waiting mechanism for reinitialization.
        private readonly ManualResetEventSlim reinitEvent = new ManualResetEventSlim(false);

        // Flag set by background thread when the camera size has changed.
        private volatile bool textureReinitRequired;

        // Timing for animation.
        private DateTime _lastUpdate = DateTime.Now;
        private double time;

        internal Kpc8Renderer(IKpcBuild kpc) {
            _rawRom = ((IRawMemory)kpc.Rom).RawBytes;
            _rawRam = ((IRawMemory)kpc.Ram).RawBytes;
            frontBuffer = Array.Empty<byte>();
            backBuffer = Array.Empty<byte>();
        }

        float cameraOffsetTemp = 0;

        /// <summary>
        /// Computes a frame (pixel-by-pixel) into the provided target buffer.
        /// This method runs on a background thread and does not call any OpenGL functions.
        /// It waits if the camera size has changed until the main thread reinitializes.
        /// </summary>
        private unsafe void ComputeFrame(ref byte[] targetBuffer) {
            // Read GPU registers.
            var grs = _rawRam.ReadStruct<GpuRegisters>(GrsAddr, 0);
            int cameraSizeX = grs.CameraSizeX;
            int cameraSizeY = grs.CameraSizeY;

            // If the current camera size differs from our texture dimensions, notify and wait.
            if (cameraSizeX != textureWidth || cameraSizeY != textureHeight || cameraSizeX == 0 || cameraSizeY == 0) {
                textureReinitRequired = true;
                reinitEvent.Wait(); // Wait until the main thread reinitializes.
                // After waiting, re-read registers.
                grs = _rawRam.ReadStruct<GpuRegisters>(GrsAddr, 0);
                cameraSizeX = grs.CameraSizeX;
                cameraSizeY = grs.CameraSizeY;
            }

            double deltaTime = (DateTime.Now - _lastUpdate).TotalMilliseconds / 1000.0;
            double freq = 0.05;
            time += deltaTime;

            float scale = 512f;
            double sinT = Math.Sin(2 * Math.PI * freq * cameraOffsetTemp);
            double cosT = Math.Cos(2 * Math.PI * freq * cameraOffsetTemp);
            short cameraScrollX = (short)(sinT * scale);
            short cameraScrollY = (short)(cosT * scale);

            int tilemapSizeX = grs.TilemapSizeX;
            int tilemapSizeY = grs.TilemapSizeY;

            var ss = _rawRom.ReadStruct<Spritesheet>(SsAddr, 0);
            var tm = _rawRam.ReadStruct<Tilemaps>(TmAddr, 0);
            var tss = _rawRam.ReadStruct<TileSsSelectors>(TssAddr, 0);
            var tpa = _rawRam.ReadStruct<TilePaletteAssignments>(TpaAddr, 0);
            var plt = _rawRam.ReadStruct<Palettes>(PltAddr, 0);

            // Compute each pixel's color.
            for (int cY = 0; cY < cameraSizeY; cY++) {
                int tmPixelY = Mod(cY + cameraScrollY, tilemapSizeY * 8);
                int tmY = tmPixelY / 8;
                for (int cX = 0; cX < cameraSizeX; cX++) {
                    int tmPixelX = Mod(cX + cameraScrollX, tilemapSizeX * 8);
                    int tmX = tmPixelX / 8;

                    byte spriteId = tm.GetSpriteId(tmX, tmY, tilemapSizeX);
                    byte ssId = tss.GetSpritesheetId(tmX, tmY);
                    byte colorId = ss.GetPixelColorId(ssId, spriteId, tmPixelX % 8, tmPixelY % 8);

                    Color color;
                    if (colorId == 0) {
                        color = ColorBytesToColor(grs.bgColorB1, grs.bgColorB2);
                    } else {
                        byte paletteId = tpa.GetTilePaletteId(tmX, tmY);
                        color = plt.GetColor(colorId, paletteId);
                    }

                    int index = (cY * cameraSizeX + cX) * 4;
                    targetBuffer[index + 0] = color.R;
                    targetBuffer[index + 1] = color.G;
                    targetBuffer[index + 2] = color.B;
                    targetBuffer[index + 3] = 255;
                }
            }
            _lastUpdate = DateTime.Now;
            cameraOffsetTemp += 0.005f;
        }

        /// <summary>
        /// This method is run on a background thread and computes the pixel data into backBuffer.
        /// It then swaps the buffers for use by the UI thread.
        /// </summary>
        public void BackgroundRenderLoop(CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                ComputeFrame(ref backBuffer);

                lock (bufferLock) {
                    var temp = frontBuffer;
                    frontBuffer = backBuffer;
                    backBuffer = temp;
                }
            }
        }

        /// <summary>
        /// Must be called on the UI thread.
        /// Updates the texture from the frontBuffer.
        /// </summary>
        public void UpdateTextureFromFrontBuffer() {
            lock (bufferLock) {
                GL.BindTexture(TextureTarget.Texture2D, textureId);
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, textureWidth, textureHeight,
                    PixelFormat.Rgba, PixelType.UnsignedByte, frontBuffer);
            }
        }

        /// <summary>
        /// Renders the textured quad to fill the display.
        /// Must be called on the UI thread (e.g. in GLControl.Paint).
        /// </summary>
        public void RenderQuad(int displayWidth, int displayHeight) {
            float imageAspect = (float)textureWidth / textureHeight;
            int newWidth, newHeight;
            // Compute a centered rectangle that preserves the aspect ratio.
            if ((float)displayWidth / displayHeight > imageAspect) {
                // Parent is wider than our image: use full height.
                newWidth = (int)(displayHeight * imageAspect);
                newHeight = displayHeight;
            } else {
                // Parent is taller: use full width.
                newWidth = displayWidth;
                newHeight = (int)(displayWidth / imageAspect);
            }
            int offsetX = (displayWidth - newWidth) / 2;
            int offsetY = (displayHeight - newHeight) / 2;

            // Set the viewport to the entire control.
            GL.Viewport(0, 0, displayWidth, displayHeight);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Set up an orthographic projection for the control.
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, displayWidth, 0, displayHeight, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0.0, 1.0); GL.Vertex2(offsetX, offsetY);
            GL.TexCoord2(1.0, 1.0); GL.Vertex2(offsetX + newWidth, offsetY);
            GL.TexCoord2(1.0, 0.0); GL.Vertex2(offsetX + newWidth, offsetY + newHeight);
            GL.TexCoord2(0.0, 0.0); GL.Vertex2(offsetX, offsetY + newHeight);
            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }

        /// <summary>
        /// Call this on the UI thread once at startup—or whenever the camera size changes—to reinitialize the texture and buffers.
        /// This method MUST be called on the main (UI) thread.
        /// </summary>
        public void SetupTexture() {
            // Read GPU registers to obtain camera dimensions.
            var grs = _rawRam.ReadStruct<GpuRegisters>(GrsAddr, 0);
            int cameraSizeX = grs.CameraSizeX;
            int cameraSizeY = grs.CameraSizeY;
            if (cameraSizeX == 0 || cameraSizeY == 0) {
                return;
            }

            // Reinitialize only if the size has changed.
            if (cameraSizeX != textureWidth || cameraSizeY != textureHeight) {
                reinitEvent.Reset(); // Pause background computations.
                textureWidth = cameraSizeX;
                textureHeight = cameraSizeY;
                int bufferSize = textureWidth * textureHeight * 4;
                frontBuffer = new byte[bufferSize];
                backBuffer = new byte[bufferSize];

                if (textureId < 0) {
                    textureId = GL.GenTexture();
                }
                GL.BindTexture(TextureTarget.Texture2D, textureId);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                // Allocate texture memory.
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    textureWidth, textureHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
                textureReinitRequired = false; // Clear the flag.
                reinitEvent.Set(); // Resume background computations.
            }
        }

        /// <summary>
        /// Returns true if the background thread has determined that the texture size (from GPU registers)
        /// does not match the current texture dimensions. This indicates that the texture must be reinitialized
        /// by the main (UI) thread.
        /// </summary>
        public bool IsTextureReinitRequired() {
            return textureReinitRequired || frontBuffer.Length == 0 || backBuffer.Length == 0;
        }

        /// <summary>
        /// Clears the GLControl’s frame (used when stopping rendering).
        /// Must be called on the UI thread.
        /// </summary>
        public void ClearFrame(int displayWidth, int displayHeight) {
            GL.Viewport(0, 0, displayWidth, displayHeight);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

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
