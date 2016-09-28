using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.Blocks;
using AcadLib.Blocks.CommonBlocks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace PIK_SS_Acad.CounterNumbering.Counters
{
    abstract class CounterBase : BlockBase,ICounter 
    {
        static Tolerance Tolerance;
        public string Name { get; set; }
        public double Scale { get; set; }
        public int Number { get; set; }

        static CounterBase()
        {           
            
        }

        public CounterBase (BlockReference blRef, string blName) : base(blRef, blName)
        {
            Scale = DefineScale(blRef);            
        }

        private double DefineScale (BlockReference blRef)
        {
            var scale = blRef.ScaleFactors.X;            
            return scale;
        }

        public void InsertLeader (string blLeaderName, ref List<AcadLib.Blocks.CommonBlocks.Leader> leaders, 
            BlockTableRecord btrOwner, Transaction t)
        {            
            var ptLeader = GetLeaderPosition();
            string value = Name + Number;
            if (!FindOldLeader(ptLeader, value, ref leaders))
            {
                var blRefLeader = BlockInsert.InsertBlockRef(blLeaderName, ptLeader, btrOwner, t, 100);
                var leader = new AcadLib.Blocks.CommonBlocks.Leader(blRefLeader, blLeaderName);
                leader.SetName(value);                
                SetLeaderDynProp(blRefLeader);
            }
        }

        private bool FindOldLeader (Point3d ptLeader, string value,
            ref List<AcadLib.Blocks.CommonBlocks.Leader> leaders)
        {
            var cannoscale = AcadLib.Scale.ScaleHelper.GetCurrentAnnoScale(Db);
            Tolerance = new Tolerance(2 * cannoscale, 2 * cannoscale);
            var leadersOld = leaders.Where(l => l.Position.IsEqualTo(ptLeader, Tolerance)).
                                OrderBy(o=>(ptLeader-o.Position).Length).ToList();
            if (leadersOld.Any())
            {
                leadersOld.First().SetName(value);                
                return true;
            }
            return false;
        }

        private Point3d GetLeaderPosition ()
        {
            double x =0;
            double y=0;
            if (Bounds.HasValue)
            {
                var b = Bounds.Value;
                x = b.MinPoint.X + (b.MaxPoint.X - b.MinPoint.X) * 0.5;
                y = b.MinPoint.Y + (b.MaxPoint.Y - b.MinPoint.Y) * 0.1;
            }
            else
            {
                x = Position.X;
                y = Position.Y;
            }
            var ptLeader = new Point3d(x, y, 0);
            return ptLeader;
        }

        private void SetLeaderDynProp (BlockReference blRefLeader)
        {
            var scale = AcadLib.Scale.ScaleHelper.GetCurrentAnnoScale(Db);
            foreach (DynamicBlockReferenceProperty item in blRefLeader.DynamicBlockReferencePropertyCollection)
            {
                if (item.PropertyName == "Тип стрелки")
                {
                    item.Value = "Точка";                    
                }
                else if (item.PropertyName== "Ширина полочки")
                {
                    item.Value = 7 * scale;
                }
                else if (item.PropertyName == "Длина")
                {
                    item.Value = 7 * scale;
                }
                else if (item.PropertyName == "Угол")
                {
                    item.Value = 300d.ToRadians();
                }
            }
        }
    }
}
