using Scada.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scada.Core.Binding
{
    public interface ISensorResolver
    {
        SensorPoint? ResolveBySID(string sid);
    }
}
