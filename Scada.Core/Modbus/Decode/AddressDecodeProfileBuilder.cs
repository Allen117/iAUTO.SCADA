using System;
using System.Collections.Generic;
using System.Globalization;

namespace Scada.Core.Modbus.Decode
{
    public static class AddressDecodeProfileBuilder
    {
        public static Dictionary<int, AddressDecodeProfile>
            BuildFromGroup(ParsedNodeGroup group)
        {
            var dict = new Dictionary<int, AddressDecodeProfile>();

            int addrCount = group.Addresses.Length;
            int scaleCount = group.Scales.Length;

            for (int i = 0; i < addrCount; i++)
            {
                int address = group.Addresses[i];

                string scaleText = "1";
                if (i < scaleCount && !string.IsNullOrWhiteSpace(group.Scales[i]))
                    scaleText = group.Scales[i].Trim();

                var profile = BuildProfile(address, scaleText);
                dict[address] = profile;
            }

            return dict;
        }

        /// <summary>
        /// 解析 Scale 字串（如：0.1、Float、Float@0.1、UInt32@0.01）
        /// </summary>
        private static AddressDecodeProfile BuildProfile(
            int address,
            string scaleText)
        {
            // ===== 預設 =====
            DecodeDataType dataType = DecodeDataType.Int16;
            int wordCount = 1;
            EndianType endian = EndianType.AB;
            bool signed = true;
            float scale = 1f;

            // ===== 拆解 @ =====
            // ex: "Float@0.1"
            string[] parts = scaleText.Split('@', StringSplitOptions.RemoveEmptyEntries);

            string typePart = parts[0];
            if (parts.Length > 1)
            {
                float.TryParse(
                    parts[1],
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out scale);
            }

            // ===== 判斷資料型態 =====
            switch (typePart.ToUpperInvariant())
            {
                case "105":
                    dataType = DecodeDataType.Float;
                    endian = EndianType.BA;
                    wordCount = 2;
                    signed = true;
                    scale = (parts.Length > 1) ? scale : 1f;
                    break;

                case "FloatingPt":
                    dataType = DecodeDataType.Float;
                    endian = EndianType.BA;
                    wordCount = 2;
                    signed = true;
                    scale = (parts.Length > 1) ? scale : 1f;
                    break;

                case "UINT32":
                    dataType = DecodeDataType.UInt32;
                    wordCount = 2;
                    signed = false;
                    scale = (parts.Length > 1) ? scale : 1f;
                    break;

                case "INT32":
                    dataType = DecodeDataType.Int32;
                    wordCount = 2;
                    signed = true;
                    scale = (parts.Length > 1) ? scale : 1f;
                    break;

                default:
                    // 預設視為「倍率字串」
                    // ex: "0.1"
                    float.TryParse(
                        typePart,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out scale);

                    dataType = DecodeDataType.Int16;
                    wordCount = 1;
                    signed = true;
                    break;
            }

            return new AddressDecodeProfile
            {
                Address = address,
                DataType = dataType,
                WordCount = wordCount,
                Endian = endian,
                Signed = signed,
                Scale = scale
            };
        }
    }
}
