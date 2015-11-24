using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EB360Observer.Entity
{    
    public enum ElectricBusinessEnum
    {
        [Description("京东")]
        JD = 1,
        [Description("一号店")]
        YHD = 2,
        [Description("苏宁")]
        SUNING = 3
    }
}
