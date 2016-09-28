using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace PIK_SS_Acad.CounterNumbering.Counters
{
    class CounterByAttr : CounterBase
    {
        public const string BlockNameStart = "СС_Счетчик";
        public CounterByAttr (BlockReference blRef, string blName) : base(blRef, blName)
        {
            Name = GetPropValue<string>("Имя");
        }
    }
}
