namespace TtlDumper.Services
{
    /// <summary>
    /// Extension methods to help set the levels to control how the chip behaves.
    /// </summary>
    public static class TargetModeExtensions
    {
        /// <summary>
        /// Returns a tuple describing the chip, output and write control levels.
        /// </summary>
        /// <param name="mode">The desired chip mode.</param>
        /// <returns></returns>
        public static (bool ChipEnableLevel, bool OutputEnableLevel, bool WriteEnableLevel) ToControlLevels(this TargetMode mode)
        {
            var chipEnableLevel = mode.IsBitSet(2);
            var outputEnableLevel = mode.IsBitSet(1);
            var writeEnableLevel = mode.IsBitSet(0);

            return (chipEnableLevel, outputEnableLevel, writeEnableLevel);
        }
    }
}