using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace PIK_SS_Acad.CounterNumbering.Counters
{
    /// <summary>
    /// Счетчик Q
    /// </summary>
    class CounterQ : CounterBase
    {
        public const string BlockName = "АК_Счетчик_Q";
        public CounterQ (BlockReference blRef, string blName) : base(blRef, blName)
        {
            Name = "Q";
        }
    }
}
