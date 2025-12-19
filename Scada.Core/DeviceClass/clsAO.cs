using Scada.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scada.Core.DeviceClass
{
    public class clsAO : clsControlDevice
    {
        public string[] OutputNames { get; } = new string[20];
        public string[] OutputCIDs { get; } = new string[20];

        // 可擴充：
        // public double[] CurrentOutputs;
        // public double[] Setpoints;
        // public RangeLimit[] Limits;

        public clsAO()
        {
            TypeID = 158;
        }
    }
}
