using Scada.Core.Domain;
using Scada.Core.Modbus;
using Scada.Core.Modbus.Decode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scada.Core.Data.Repository
{
    public sealed class EndDeviceRepository
    {
        public tgEndDevice[] CreateFromCoordinator(tgCoordinator coordinator, ParsedCoordinatorFile parsed)
        {
            if (coordinator == null)
                throw new ArgumentNullException(nameof(coordinator));

            switch (coordinator.u8Type)
            {
                case 30:
                    return CreateType30(coordinator, parsed);

                case 53:
                    return CreateType53(coordinator);

                default:
                    return Array.Empty<tgEndDevice>();
            }
        }
        private tgEndDevice[] CreateType30(
            tgCoordinator coordinator,
            ParsedCoordinatorFile parsed)
        {
            var devices = new List<tgEndDevice>();

            foreach (var group in parsed.Groups)
            {
                var device = new tgEndDevice
                {
                    // VB 等價邏輯
                    lngMac = (long)coordinator.lngMac * 65536L
                             + 1
                             + group.GroupIndex,
                };
                var profiles =
                    AddressDecodeProfileBuilder.BuildFromGroup(group);

                device.AddressProfiles = profiles;

                devices.Add(device);
            }

            return devices.ToArray();
        }

        private IEnumerable<int> ParseModbusFormat(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
                yield break;

            format = format.Trim();

            // 例：1-5
            if (format.Contains("-"))
            {
                var parts = format.Split('-', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int start) &&
                    int.TryParse(parts[1], out int end))
                {
                    for (int i = start; i <= end; i++)
                        yield return i;
                }
                yield break;
            }

            // 例：1,3,5
            var tokens = format.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens)
            {
                if (int.TryParse(token.Trim(), out int value))
                    yield return value;
            }
        }

        private tgEndDevice[] CreateType53(tgCoordinator coordinator)
        {
            var devices = new List<tgEndDevice>();

            foreach (int x in ParseModbusFormat(coordinator.strModbusFormat))
            {
                var device = new tgEndDevice
                {
                    lngMac = (long)coordinator.lngMac * 65536L
                       + (long)x * 256L
                       + 1
                };

                devices.Add(device);
            }

            return devices.ToArray();
        }




    }

}
