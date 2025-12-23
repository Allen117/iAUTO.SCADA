using Scada.Core.Domain;
using Scada.Core.Runtime;

namespace Scada.Core.Binding
{
    public class SensorResolver : ISensorResolver
    {
        public SensorPoint? ResolveBySID(string sid)
        {
            if (string.IsNullOrWhiteSpace(sid)) return null;

            // 搜尋所有已載入設備中的感測點，比對 SID 字串
            return ScadaRuntime.gcolEndDeviceNode.Values
                .SelectMany(dev => dev.Sensors.Values)
                .FirstOrDefault(sp => sp.SID == sid);
        }
    }

}
