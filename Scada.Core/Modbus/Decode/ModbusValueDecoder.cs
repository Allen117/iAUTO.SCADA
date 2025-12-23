using System;

namespace Scada.Core.Modbus.Decode
{
    public static class ModbusValueDecoder
    {
        /// <summary>
        /// 依 AddressDecodeProfile 解出單一數值
        /// 目前支援：
        /// - Int16 (WordCount = 1)
        /// - Float  (WordCount = 2, AB)
        /// </summary>
        public static double Decode(ushort[] regs, int startAddress, AddressDecodeProfile profile)
        {
            int index = profile.Address - startAddress;
            double rawValue = 0;

            switch (profile.DataType)
            {
                case DecodeDataType.Float:
                    rawValue = DecodeFloat(regs, index, profile.Endian);
                    break;
                default:
                    rawValue = DecodeInt16(regs, index);
                    break;
            }

            // ⭐ 必須套用倍率運算 (依 VB 邏輯：實際值 = 原始值 * Scale)
            return profile.Scale != 0 ? rawValue * profile.Scale : rawValue;
        }

        private static double DecodeInt16(ushort[] regs, int index)
        {
            ushort raw = regs[index];
            short value = unchecked((short)raw);
            return value;
        }

        /// <summary>
        /// Float (2 word, AB)
        /// regs[index]     = High word
        /// regs[index + 1] = Low word
        /// </summary>
        private static double DecodeFloat(
            ushort[] regs,
            int index,
            EndianType endian)
        {
            if (index + 1 >= regs.Length)
                throw new IndexOutOfRangeException(
                    $"Float decode needs 2 words, but index={index}, length={regs.Length}");

            ushort w0 = regs[index];
            ushort w1 = regs[index + 1];

            uint raw = endian switch
            {
                EndianType.AB => ((uint)w0 << 16) | w1,
                EndianType.BA => ((uint)w1 << 16) | w0,
                _ => ((uint)w0 << 16) | w1
            };

            float value = BitConverter.Int32BitsToSingle((int)raw);
            return value;
        }

    }
}
