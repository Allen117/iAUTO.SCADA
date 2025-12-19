using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scada.Core.Domain
{
    public abstract class clsControlDevice
    {
        public int TypeID { get; set; }
        public long MacId { get; set; }
        public long RecordId { get; set; }
        public string Name { get; set; }
    }
}
