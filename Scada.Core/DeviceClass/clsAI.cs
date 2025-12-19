using Scada.Core.Binding;
using Scada.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scada.Core.DeviceClass
{
    public class clsAI : clsControlDevice, IBindableControlDevice
    {
        public string[] InputNames { get; } = new string[20];
        public string[] InputSIDs { get; } = new string[20];

        public List<SensorPoint> BoundSensors { get; } = new();

        public void Bind(ISensorResolver resolver)
        {
            for (int i = 0; i < 20; i++)
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
