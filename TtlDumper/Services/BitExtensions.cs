using System;
using System.Collections;
using System.Globalization;

namespace TtlDumper.Services
{
    /// <summary>
    /// Extension methods to help manipulate bits and bytes.
    /// </summary>
    public static class BitExtensions
    {
        /// <summary>
        /// Convert an 8 bit long bit array into a byte
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static byte ToByte(this BitArray bits)
        {
            if (bits.Count > 8)
            {
                throw new ArgumentException("The bit array must be 8 bits or less in length", nameof(bits));
            }
            
            var bytes = new byte[1];

            bits.CopyTo(bytes, 0);

            return bytes[0];
        }

        /// <summary>
        /// Convert an 8 bit long bit array into a byte
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this BitArray bits)
        {
            var numberOfBytes = (int) Math.Ceiling(bits.Length / 8.0);

            var bytes = new byte[numberOfBytes];

            bits.CopyTo(bytes, 0);

            return bytes;
        }

        public static ushort ToUInt16(this BitArray bits)
        {
            if (bits.Length > 16)
            {
                throw new ArgumentException("16 bits max", nameof(bits));
            }
            
            var array = new ushort[1];
            
            bits.CopyTo(array, 0);

            return array[0];

        }

        /// <summary>
        /// Returns whether the bit at the specified position is set.
        /// https://stackoverflow.com/a/16533966
        /// </summary>
        /// <typeparam name="T">Any integer type.</typeparam>
        /// <param name="t">The value to check.</param>
        /// <param name="pos">The position of the bit to check, 0 refers to the least significant bit.</param>
        /// <returns>true if the specified bit is on, otherwise false.</returns>
        public static bool IsBitSet<T>(this T t, int pos) where T : struct, IConvertible
        {
            var value = t.ToInt64(CultureInfo.CurrentCulture);
            return (value & (1 << pos)) != 0;
        }
    }
}