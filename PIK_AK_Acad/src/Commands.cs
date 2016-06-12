using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;


[assembly: CommandClass(typeof(PIK_AK_Acad.Commands))]

namespace PIK_AK_Acad
{
    public class Commands
    {
        public const string group = "PIK";

        [CommandMethod(group, nameof(AK_Counters), CommandFlags.Modal)]
        public void AK_Counters()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                Counters.CounterService.CalcAndInsertTables(doc);
            });
        }
    }
}
