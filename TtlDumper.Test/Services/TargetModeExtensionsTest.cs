using NUnit.Framework;
using TtlDumper.Services;

namespace TtlDumper.Test.Services
{
    [TestFixture]
    public class TargetModeExtensionsTest
    {
        [Test]
        public void TestToControlLevels()
        {
            var levels = TargetMode.ReadCycle.ToControlLevels();

            Assert.IsFalse(levels.ChipEnableLevel);
            Assert.IsFalse(levels.OutputEnableLevel);
            Assert.True(levels.WriteEnableLevel);

            levels = TargetMode.WriteCycle.ToControlLevels();

            Assert.IsFalse(levels.ChipEnableLevel);
            Assert.IsTrue(levels.OutputEnableLevel);
            Assert.IsFalse(levels.WriteEnableLevel);

            levels = TargetMode.OutputDisable.ToControlLevels();

            Assert.IsFalse(levels.ChipEnableLevel);
            Assert.IsTrue(levels.OutputEnableLevel);
            Assert.IsTrue(levels.WriteEnableLevel);

            levels = TargetMode.Unselected.ToControlLevels();

            Assert.IsTrue(levels.ChipEnableLevel);
            Assert.IsFalse(levels.OutputEnableLevel);
            Assert.IsFalse(levels.WriteEnableLevel);
        }
    }
}
