using Scada.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scada.Core.Runtime
{
    /// <summary>
    /// 整個系統的全域 Runtime 狀態中心
    /// （Coordinator、EndDevice、ControlDevice 都集中管理）
    /// </summary>
    /// 
    public static class ScadaRuntime
    {
        // ================= Coordinator =================
        public static readonly Dictionary<string, tgCoordinator> gcolCoordinator
            = new Dictionary<string, tgCoordinator>();

        // ================= End Devices =================
        public static readonly Dictionary<long, tgEndDevice> gcolEndDeviceNode
            = new Dictionary<long, tgEndDevice>();

        // ================= Control Devices =================
        public static readonly List<clsControlDevice> gcolControlDevice
            = new List<clsControlDevice>();
    }
}
