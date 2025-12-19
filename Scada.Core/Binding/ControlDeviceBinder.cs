using Scada.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scada.Core.Binding
{
    public class ControlDeviceBinder
    {
        private readonly ISensorResolver _resolver;

        public ControlDeviceBinder(ISensorResolver resolver)
        {
            _resolver = resolver;
        }

        public void BindAll(IEnumerable<clsControlDevice> devices)
        {
            foreach (var d in devices)
            {
                if (d is IBindableControlDevice bindable)
                    bindable.Bind(_resolver);
            }
        }
    }
}
