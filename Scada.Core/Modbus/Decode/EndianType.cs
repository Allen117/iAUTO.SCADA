namespace Scada.Core.Modbus.Decode
{
    public enum EndianType
    {
        AB,     // Big-endian word
        BA,     // Little-endian word
        CDAB,   // 32-bit swap
        DCBA    // 32-bit full swap
    }
}
