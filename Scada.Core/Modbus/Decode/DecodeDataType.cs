using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scada.Core.Modbus.Decode
{
    public enum DecodeDataType
    {
        Int16,
        UInt16,
        Int32,
        UInt32,
        Float,
        Double,
        Bcd,
        Bit
    }
}
