using System.Collections.Generic;

// ReSharper disable IdentifierTypo

namespace PazarYeri.Models.Common
{
    public class CommonOrder
    {
        public CommonOrderHeader Header { get; set; }
        
        public List<CommonOrderLine> Lines { get; set; }
    }
}