using System;
using System.Collections.Generic;
using System.Globalization;

namespace Scada.Core.Modbus.Decode
{
    public static class AddressDecodeProfileBuilder
    {
        public static Dictionary<int, AddressDecodeProfile> BuildFromGroup(ParsedNodeGroup group)
        {
            var dict = new Dictionary<int, AddressDecodeProfile>();
            int addrCount = group.Addresses.Length;

            for (int i = 0; i < addrCount; i++)
            {
                int address = group.Addresses[i];
                string scaleText = (i < group.Scales.Length) ? group.Scales[i] : "1";
                string? nodeName = (i < group.Names.Length) ? group.Names[i] : null;

                // 傳入 i + 1 作為 S 序號 (S1, S2...)
                var profile = BuildProfile(address, scaleText, i + 1, nodeName);
                dict[address] = profile;
            }
            return dict;
        }

        private static AddressDecodeProfile BuildProfile(int address, string scaleText, int seqIndex, string? nodeName)
        {
            DecodeDataType dataType = DecodeDataType.Int16;
            int wordCount = 1;
            EndianType endian = EndianType.AB;
            float scale = 1f;

            // ⭐ 新增：根據地址區間自動判定功能碼
            byte functionCode;
            if ((address >= 40001 && address <= 49999) || (address >= 400001 && address <= 465535))
                functionCode = 3; // Holding Registers
            else if ((address >= 30001 && address <= 39999) || (address >= 300001 && address <= 365535))
                functionCode = 4; // Input Registers
            else if ((address >= 10001 && address <= 19999) || (address >= 100001 && address <= 165535))
                functionCode = 2; // Discrete Inputs
            else
                functionCode = 1; // Coils

            string[] parts = scaleText.Split('@', StringSplitOptions.RemoveEmptyEntries);
            string typePart = parts[0];

            if (parts.Length > 1)
                float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out scale);

            switch (typePart.ToUpperInvariant())
            {
                case "105":
                case "FLOATINGPT":
                    dataType = DecodeDataType.Float;
                    endian = EndianType.BA;
                    wordCount = 2;
                    break;
                case "UINT32":
                    dataType = DecodeDataType.UInt32;
                    wordCount = 2;
                    break;
                case "INT32":
                    dataType = DecodeDataType.Int32;
                    wordCount = 2;
                    break;
                default:
                    // 如果 typePart 本身就是數字（倍率），則直接解析為 scale
                    float.TryParse(typePart, NumberStyles.Float, CultureInfo.InvariantCulture, out scale);
                    break;
            }

            return new AddressDecodeProfile
            {
                Address = address,
                Name = nodeName,
                SequenceIndex = seqIndex,
                DataType = dataType,
                WordCount = wordCount,
                Endian = endian,
                Scale = scale == 0 ? 1f : scale,
                // ⭐ 確保 AddressDecodeProfile 類別中有這個屬性
                FunctionCode = functionCode
            };
        }
    }
}