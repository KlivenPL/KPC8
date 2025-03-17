using System;
namespace Runner._Infrastructure.UnsafeExtensions;

public static class UnsafeByteArrayExtensions {
    /// <summary>
    /// Reads a struct of type T from a byte array using a base offset and an index.
    /// </summary>
    /// <typeparam name="T">The unmanaged struct type to read.</typeparam>
    /// <param name="data">The source byte array.</param>
    /// <param name="index">
    /// The order of the struct in the block (not a raw byte offset). For example, if T is 4096 bytes,
    /// an index of 1 reads from baseOffset + 4096.
    /// </param>
    /// <param name="baseOffset">An optional base offset in the array.</param>
    /// <returns>The struct read from the byte array.</returns>
    public static unsafe T ReadStruct<T>(this byte[] data, int baseOffset, int index) where T : unmanaged {
        int size = sizeof(T);
        ushort offset = (ushort)(baseOffset + index * size);

#if DEBUG
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        if (data.Length < offset + size)
            throw new ArgumentException("The byte array is too small.", nameof(data));
#endif

        fixed (byte* p = &data[offset]) {
            return *(T*)p;
        }
    }

    /// <summary>
    /// Writes a struct of type T into a byte array at a specified index (with a base offset).
    /// </summary>
    public static unsafe void WriteStruct<T>(this byte[] data, int baseOffset, int index, T value) where T : unmanaged {
        int size = sizeof(T);
        ushort offset = (ushort)(baseOffset + index * size);

#if DEBUG
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        if (data.Length < offset + size)
            throw new ArgumentException("The byte array is too small.", nameof(data));
#endif

        fixed (byte* p = &data[offset]) {
            *(T*)p = value;
        }
    }
}
