using System.Collections;
using System.IO;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using TtlDumper.Services;

namespace TtlDumper.Test.Services
{
    [TestFixture]
    [Ignore("Integration test, run when needed if hardware connected directly.")]
    public class IoServiceTest
    {
        [Test]
        public void TestSetAddressBits()
        {
            var path = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "..", "..", "..", "..",
                "TtlDumper",
                "appsettings.json");

            path = Path.GetFullPath(path);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(path)
                .Build();

            var service = new IoService(configuration);

            var address = new BitArray(14)
            {
                [0] = true,
                [1] = true,
                [2] = true,
                [3] = true,
                [4] = true,
                [5] = true,
                [6] = true,
                [7] = true,
                [8] = true,
                [9] = true,
                [10] = true,
                [11] = true,
                [12] = true,
                [13] = true
            };

            var value = service.ReadByte(address);

            Assert.AreEqual(0x00, value);

            service.Dispose();
        }
    }
}
