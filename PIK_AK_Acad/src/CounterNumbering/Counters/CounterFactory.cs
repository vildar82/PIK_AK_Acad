using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace PIK_AK_Acad.CounterNumbering.Counters
{
    static class CounterFactory
    {
        static Dictionary<string, Type> dictCountersType = new Dictionary<string, Type>()
        {
            { CounterQ.BlockName, typeof(CounterQ) },
            { CounterByAttr.BlockName, typeof(CounterByAttr) },
        };

        public static ICounter Create (ObjectId id)
        {
            ICounter counter = null;
            var blRef = id.GetObject(OpenMode.ForRead) as BlockReference;
            if (blRef != null)
            {
                Type blType;
                string blName = blRef.GetEffectiveName();
                if (dictCountersType.TryGetValue(blName, out blType))
                {
                    counter = (ICounter)Activator.CreateInstance(blType, blRef, blName);
                }
            }
            return counter;
        }
    }
}
