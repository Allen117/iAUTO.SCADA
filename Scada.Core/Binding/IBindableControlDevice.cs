using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scada.Core.Binding
{
    public interface IBindableControlDevice
    {
        void Bind(ISensorResolver resolver);
    }
}
