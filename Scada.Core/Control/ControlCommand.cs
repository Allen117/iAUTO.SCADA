using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scada.Core.Control
{
    public class ControlCommand
    {
        public long MacId { get; init; }      // 設備 MAC
        public string CID { get; init; }      // 控制編號 (例如 123456-C1)
        public double Value { get; init; }    // 欲寫入的數值
        public string Description { get; init; } = "";
        public DateTime Timestamp { get; init; } = DateTime.Now;
    }
}
