using ABElectronicsUK;
using TtlDumper.Configuration;

namespace TtlDumper.Services
{
    /// <summary>
    /// Helper class to keep a reference to the bus and how it should be used/configured.
    /// </summary>
    public class BusInfo : BusDefinition
    {
        public BusInfo(IOPi bus, BusDefinition definition) : base(definition)
        {
            Bus = bus;
        }

        public IOPi Bus;
    }
}