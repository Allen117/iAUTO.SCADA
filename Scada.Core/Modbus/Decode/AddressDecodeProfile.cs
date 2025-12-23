namespace Scada.Core.Modbus.Decode
{
    public sealed class AddressDecodeProfile
    {
        public int Address { get; init; }
        public string? Name { get; init; }
        public byte FunctionCode { get; init; }
        /// <summary>點位順序 (1, 2, 3...)，用於產生 SID</summary>
        public int SequenceIndex { get; init; }

        public float Scale { get; init; } = 1f;
        public int WordCount { get; init; } = 1;
        public DecodeDataType DataType { get; init; } = DecodeDataType.Int16;
        public EndianType Endian { get; init; } = EndianType.AB;
        public bool Signed { get; init; } = true;
    }
}