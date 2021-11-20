using System;
using System.Collections;
using NUnit.Framework;
using TtlDumper.Services;

namespace TtlDumper.Test.Services
{
    [TestFixture]
    public class BitExtensionsTest
    {
        [Test]
        public void TestToByteWhenBitArrayNot8BitsLong()
        {
            var bits = new BitArray(9);

            Assert.Throws<ArgumentException>(() => bits.ToByte());
        }

        [Test]
        public void TestToByteWhenBitArrayLessThan8BitsLong()
        {
            var bits = new BitArray(4)
                       {
                           [0] = true,
                           [1] = true,
                           [2] = true,
                           [3] = true
                       };

            var value = bits.ToByte();

            Assert.AreEqual(15, value);
        }

        [Test]
        public void TestToBit()
        {
            var bits = new BitArray(8)
                       {
                           [0] = true,
                           [1] = true,
                           [2] = true,
                           [3] = true,
                           [4] = true,
                           [5] = true,
                           [6] = true,
                           [7] = true
                       };

            var value = bits.ToByte();

            Assert.AreEqual(255, value);

            bits[0] = true;
            bits[1] = true;
            bits[2] = true;
            bits[3] = true;
            bits[4] = true;
            bits[5] = true;
            bits[6] = true;
            bits[7] = false;

            value = bits.ToByte();

            Assert.AreEqual(127, value);

            bits[0] = true;
            bits[1] = false;
            bits[2] = false;
            bits[3] = false;
            bits[4] = false;
            bits[5] = false;
            bits[6] = false;
            bits[7] = false;

            value = bits.ToByte();

            Assert.AreEqual(1, value);
        }

        [Test]
        public void TestToBytes()
        {
            var address = new BitArray(12)
                          {
                              [0] = false,
                              [1] = false,
                              [2] = false,
                              [3] = false,
                              [4] = false,
                              [5] = false,
                              [6] = false,
                              [7] = true,
                              [8] = false,
                              [9] = false,
                              [10] = false,
                              [11] = true
                          };

            var bytes = address.ToBytes();

            Assert.AreEqual(128, bytes[0]);
            Assert.AreEqual(8, bytes[1]);
        }

        [Test]
        public void TestIsBitSet()
        {
            var binaryValue = 0b111;
            
            Assert.IsTrue(binaryValue.IsBitSet(2));
            Assert.IsTrue(binaryValue.IsBitSet(1));
            Assert.IsTrue(binaryValue.IsBitSet(0));

            binaryValue = 0b000;
            
            Assert.IsFalse(binaryValue.IsBitSet(2));
            Assert.IsFalse(binaryValue.IsBitSet(1));
            Assert.IsFalse(binaryValue.IsBitSet(0));

            binaryValue = 0b10;
            
            Assert.IsFalse(binaryValue.IsBitSet(2));
            Assert.IsTrue(binaryValue.IsBitSet(1));
            Assert.IsFalse(binaryValue.IsBitSet(0));
        }
    }
}
