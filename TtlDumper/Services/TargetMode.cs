namespace TtlDumper.Services
{
    /// <summary>
    /// Enumeration to help set the Chip Enable, Output Enable and Write Enable levels to control the targeted chip.
    /// See Function Table in the chip's data-sheet.
    /// </summary>
    /// <remarks>
    /// Binary notation: 0b...
    /// 1 means bit level is high. 0 means bit level is low.
    /// (NB: you might be controlling "enabled when low" inputs, don't forget to configure the pins to be inverted!)
    /// - Bit 2 (most significant) represents Chip Enable
    /// - Bit 1 represents Output Enable
    /// - Bit 0 (least significant) represents Write Enable
    /// </remarks>
    public enum TargetMode
    {
        /// <summary>
        /// Data output
        /// </summary>
        ReadCycle = 0b101,
        /// <summary>
        /// Data input
        /// </summary>
        WriteCycle = 0b110,
        /// <summary>
        /// High-impedance
        /// </summary>
        OutputDisable = 0b111,
        /// <summary>
        /// High-impedance
        /// </summary>
        Unselected = 0b000
    }
}