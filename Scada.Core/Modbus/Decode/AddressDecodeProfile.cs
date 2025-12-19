namespace Scada.Core.Modbus.Decode
{
    /// <summary>
    /// 描述單一 Modbus Address 的解碼規則
    /// （對齊 VB 中的 Scale / ScaleCode 邏輯）
    /// </summary>
    public sealed class AddressDecodeProfile
    {
        /// <summary>Modbus 位址</summary>
        public int Address { get; init; }

        /// <summary>
        /// 顯示倍率（VB 的 msngScale）
        /// 實際值 = RawValue / Scale
        /// </summary>
        public float Scale { get; init; } = 1f;

        /// <summary>需要幾個 Modbus word</summary>
        public int WordCount { get; init; } = 1;

        /// <summary>資料型態</summary>
        public DecodeDataType DataType { get; init; } = DecodeDataType.Int16;

        /// <summary>位元組順序</summary>
        public EndianType Endian { get; init; } = EndianType.AB;

        /// <summary>是否為 Signed</summary>
        public bool Signed { get; init; } = true;
    }
}
