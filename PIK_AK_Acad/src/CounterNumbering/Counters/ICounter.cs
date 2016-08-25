using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace PIK_AK_Acad.CounterNumbering.Counters
{
    interface ICounter
    {
        string Name { get; set; }
        int Number { get; set; }
        Point3d Position { get; set; }
        void InsertLeader (string blLeaderName,ref List<AcadLib.Blocks.CommonBlocks.Leader> leaders);
    }
}
