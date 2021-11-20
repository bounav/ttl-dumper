using System.Runtime.Serialization;

namespace TtlDumper.Configuration
{
    /// <summary>
    /// Enum listing the different kind of buses.
    /// </summary>
    [DataContract]
    public enum BusType
    {
        Address,
        ChipEnable,
        OutputEnable,
        WriteEnable,
        Data
    }
}