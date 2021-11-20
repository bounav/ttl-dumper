using System;
using System.Collections;
using System.Linq;
using System.Text;
using TtlDumper.Services;

namespace TtlDumper
{
    public static class LogHelper
    {
        /// <summary>
        /// Formats a debug line
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        public static void Log(BitArray address, byte value)
        {
            var addressBytes = address.ToBytes();

            var builder = new StringBuilder();

            builder.Append("0x")
                   .Append(addressBytes.Sum(x => (int)x).ToString("X").PadLeft(5, '0'))
                   .Append(" ");

            // Start 'rendering' most significant byte first
            for (var i = addressBytes.Length - 1; i >= 0; i--)
            {
                var addressByte = addressBytes[i];
                builder.Append(Convert.ToString(addressByte, 2).PadLeft(8, '0'));

                if (i > 0)
                {
                    builder.Append(" ");
                }
            }

            builder.Append(": ").Append(Convert.ToString(value, 2).PadLeft(8, '0'));

            builder.Append(" ").Append(Convert.ToChar(value));

            Console.WriteLine(builder.ToString());
        }
    }
}
