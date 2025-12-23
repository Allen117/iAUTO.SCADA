using Scada.Core.Binding;
using Scada.Core.Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scada.Core.DeviceClass
{
    public class clsAI : clsControlDevice, IBindableControlDevice
    {
        private const int PointNum = 20;
        public string[] InputNames { get; } = new string[PointNum];
        public string[] InputSIDs { get; } = new string[PointNum];

        public List<SensorPoint> BoundSensors { get; } = new();

        public void Bind(ISensorResolver resolver)
        {
            for (int i = 0; i < PointNum; i++)
            {
                var sid = InputSIDs[i];
                if (string.IsNullOrWhiteSpace(sid))
                    continue;

                var sensor = resolver.ResolveBySID(sid);

                if (sensor != null)
                    BoundSensors.Add(sensor);
            }
        }
    }
}
