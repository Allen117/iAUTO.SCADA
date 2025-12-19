using Scada.Core.Domain;
using Scada.Core.Runtime;

namespace Scada.Core.Binding
{
    public class SensorResolver : ISensorResolver
    {
        public SensorPoint? ResolveBySID(string sid)
        {
            // ⭐ 解析 sid: 655361-s1
            if (!SidParser.TryParseSid(sid, out long mac, out int sensorIndex))
                return null;

            // ⭐ 找 EndDevice
            if (!ScadaRuntime.gcolEndDeviceNode.TryGetValue(mac, out var dev))
                return null;

            if (dev.Sensors == null || dev.OrderedAddresses == null)
                return null;

            // ⭐ 檢查 index 是否存在
            if (sensorIndex < 0 || sensorIndex >= dev.OrderedAddresses.Count)
                return null;

            // ⭐ 取得真實 address
            int address = dev.OrderedAddresses[sensorIndex];

            // ⭐ 回傳 SensorPoint
            return dev.Sensors.TryGetValue(address, out var sp)
                ? sp
                : null;
        }

       

    }

}
