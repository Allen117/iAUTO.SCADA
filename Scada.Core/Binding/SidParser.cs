using System;

namespace Scada.Core.Binding
{
    public static class SidParser
    {
        /// <summary>
        /// 解析格式: MAC-Sx 例如 655361-s1
        /// </summary>
        public static bool TryParseSid(string sid, out long mac, out int index)
        {
            mac = 0;
            index = -1;

            if (string.IsNullOrWhiteSpace(sid))
                return false;

            var parts = sid.Split('-');
            if (parts.Length != 2)
                return false;

            if (!long.TryParse(parts[0], out mac))
                return false;

            var s = parts[1];   // s1 / s2 / s10
            if (!s.StartsWith("S"))
                return false;

            if (!int.TryParse(s.Substring(1), out index))
                return false;

            index--;   // ⭐ s1 = index 0
            return index >= 0;
        }
    }
}
