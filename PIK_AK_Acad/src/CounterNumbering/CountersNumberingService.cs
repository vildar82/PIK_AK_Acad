using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using PIK_AK_Acad.CounterNumbering.Counters;

namespace PIK_AK_Acad.CounterNumbering
{
    class CountersNumberingService
    {
        static string BlLeaderName = "Обозначение_Выноска_ПИК";
        public static Document Doc { get; set; }
        public static Database Db { get; set; }
        public static Editor Ed { get; set; }

        public CountersNumberingService(Document doc)
        {
            Doc = doc;
            Db = doc.Database;
            Ed = doc.Editor;
        }

        /// <summary>
        /// Нумерация блоков счетчиков на плане и простановка выносок
        /// </summary>        
        public void Numbering ()
        {
            List<AcadLib.Blocks.CommonBlocks.Leader> leaders;
            // Выбор блоков  
            var counters = Select(out leaders);
            // Группировка по типам счетчиков
            var groupCounters = IGrouping(counters);
            // Нумерация счетчиков
            Numbering(groupCounters);
            // Простановка выносок
            SetNumbering(counters, leaders);
        }        

        private List<ICounter> Select (out List<AcadLib.Blocks.CommonBlocks.Leader> leaders)
        {
            var counters = new List<ICounter>();
            leaders = new List<AcadLib.Blocks.CommonBlocks.Leader>();
            var sel = Ed.SelectBlRefs("\nВыбор блоков счетчиков на одном плане:");

            using (var t = Db.TransactionManager.StartTransaction())
            {
                foreach (var item in sel)
                {
                    var counter = CounterFactory.Create(item);
                    if (counter != null)
                    {
                        counters.Add(counter);
                    }
                    else
                    {
                        // Сбор выносок
                        var blRef = item.GetObject(OpenMode.ForRead) as BlockReference;
                        if (blRef != null)
                        {
                            string blName = blRef.GetEffectiveName();
                            if (blName == AcadLib.Blocks.CommonBlocks.Leader.BlockName)
                            {
                                var leader = new AcadLib.Blocks.CommonBlocks.Leader(blRef, blName);
                                leaders.Add(leader);
                            }
                        }
                    }
                }
                t.Commit();
            }
            return counters;
        }

        private List<List<ICounter>> IGrouping (List<ICounter> counters)
        {
            var groups = counters.GroupBy(g => g.Name).Select(s=>s.ToList()).ToList();
            return groups;
        }

        private void Numbering (List<List<ICounter>> groups)
        {
            var comparer = new AcadLib.Comparers.DoubleEqualityComparer(2000);
            foreach (var item in groups)
            {
                var sort = item.OrderBy(p => p.Position.X).GroupBy(p => p.Position.Y, comparer)
                     .OrderByDescending(g => g.Key).SelectMany(g => g).ToList();
                int number = 1;
                foreach (var s in sort)
                {
                    s.Number = number++;
                }
            }
        }

        private void SetNumbering (List<ICounter> counters, List<AcadLib.Blocks.CommonBlocks.Leader> leaders)
        {
            using (var t = Db.TransactionManager.StartTransaction())
            {
                // Блок выноски
                var leaderBtrId = GetLeaderBlock();
                var btrOwner = Db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                foreach (var item in counters)
                {
                    item.InsertLeader(BlLeaderName, ref leaders, btrOwner, t);
                }
                t.Commit();
            }
        }

        private ObjectId GetLeaderBlock ()
        {
            ObjectId idBtrLeader;
            var bt = Db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
            if (bt.Has(BlLeaderName))            
                idBtrLeader = bt[BlLeaderName];            
            else            
                idBtrLeader = AcadLib.Blocks.Block.CopyCommonBlockFromTemplate(BlLeaderName, Db);            
            if (idBtrLeader.IsNull)            
                throw new Exception($"Не определен блок выноски - {BlLeaderName}.");
            return idBtrLeader;
        }
    }
}
