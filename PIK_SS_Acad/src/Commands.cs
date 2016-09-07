using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;


[assembly: CommandClass(typeof(PIK_SS_Acad.Commands))]

namespace PIK_SS_Acad
{
    public class Commands
    {
        public const string Group = "PIK";

        [CommandMethod(Group, nameof(SS_Counters), CommandFlags.Modal)]
        public void SS_Counters()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                Counters.CounterService.CalcAndInsertTables(doc);
            });
        }

        [CommandMethod(Group, nameof(SS_CountersNumbering), CommandFlags.Modal)]
        public void SS_CountersNumbering ()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                var counter = new CounterNumbering.CountersNumberingService(doc);
                counter.Numbering();
            });
        }
    }
}
