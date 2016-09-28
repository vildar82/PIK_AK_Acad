using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace PIK_SS_Acad.CounterNumbering.Counters
{
    static class CounterFactory
    {
        //static Dictionary<string, Type> dictCountersType = new Dictionary<string, Type>()
        //{            
        //    { CounterByAttr.BlockNameStart, typeof(CounterByAttr) },
        //};

        public static ICounter Create (ObjectId id)
        {
            ICounter counter = null;
            var blRef = id.GetObject(OpenMode.ForRead) as BlockReference;
            if (blRef != null)
            {                
                string blName = blRef.GetEffectiveName();
                if (blName.StartsWith(CounterByAttr.BlockNameStart, StringComparison.OrdinalIgnoreCase))
                {
                    counter = (ICounter)Activator.CreateInstance(typeof(CounterByAttr), blRef, blName);
                }
            }
            return counter;
        }
    }
}
