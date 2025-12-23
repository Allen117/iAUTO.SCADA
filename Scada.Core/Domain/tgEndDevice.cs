using Scada.Core.Modbus.Decode;
using System;
using System.Text;

namespace Scada.Core.Domain
{
    public class tgEndDevice
    {
        public Dictionary<int, AddressDecodeProfile> AddressProfiles { get; set; }
        public Dictionary<int, SensorPoint> Sensors { get; set; }

        public List<int> OrderedAddresses { get; private set; } = new();

        public tgEndDevice()
        {
            AddressProfiles = new Dictionary<int, AddressDecodeProfile>();
            Sensors = new Dictionary<int, SensorPoint>();
        }

        public void BuildOrderedAddresses()
        {
            OrderedAddresses = Sensors.Keys
                .OrderBy(x => x)
                .ToList();
        }
        // Node Settings
        public long lngMac;
    }
}
