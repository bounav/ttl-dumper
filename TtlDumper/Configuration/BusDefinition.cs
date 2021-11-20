using System.Runtime.Serialization;

namespace TtlDumper.Configuration
{
    /// <summary>
    /// Model representing how pins should be configured for given <see cref="Type"/>.
    /// </summary>
    [DataContract]
    public class  BusDefinition
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BusDefinition()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="definition"></param>
        public BusDefinition(BusDefinition definition)
        {
            Type = definition.Type;
            Io = definition.Io;
            Address = definition.Address;
            EnablePullUpResistors = definition.EnablePullUpResistors;
            InvertPins = definition.InvertPins;
        }

        /// <summary>
        /// The type of bus (Address, Control or Data).
        /// </summary>
        public BusType Type { get; set; }

        /// <summary>
        /// The range of IO pins to use (e.g. 1 for pin one).
        /// </summary>
        /// <remarks>
        /// When <see cref="Io"/> is not set or empty, <see cref="Port"/> is used (and will default to pins 1 to 8).
        /// </remarks>
        public byte[] Io { get; set; }

        /// <summary>
        /// The port to use (0 = pins 1 to 8, 1 = pins 9 to 16)
        /// </summary>
        /// <remarks>
        /// When <see cref="Io"/> is not set or empty, <see cref="Port"/> is used (and will default to pins 1 to 8).
        /// </remarks>
        public byte Port { get; set; }

        /// <summary>
        /// 0x20 for bus "A", 0x21 for Bus "B"
        /// </summary>
        public byte Address { get; set; }
        
        /// <summary>
        /// When true, pull-up resistors will be activated.
        /// </summary>
        public bool EnablePullUpResistors { get; set; }

        /// <summary>
        /// When true, all the <see cref="Io" /> pin levels will be inverted (only valid for inputs).
        /// </summary>
        /// <remarks>Will only affect reading values (inputs)</remarks>
        public bool InvertPins { get; set; }
    }
}
