using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.Jigs;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using PIK_AK_Acad.Counters.Calcs;

namespace PIK_AK_Acad.Counters.Tables
{
    public class TableService
    {
        Document doc;
        Database db;
        Editor ed;
        double scale;

        public TableService(Document doc)
        {
            this.doc = doc;
            db = doc.Database;
            ed = doc.Editor;
        }

        public void CreateAndInsert(Scheme scheme)
        {
            using (var t = db.TransactionManager.StartTransaction())
            {
                var cs = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                scale = AcadLib.Scale.ScaleHelper.GetCurrentAnnoScale(db);
                // Создание таблиц                
                List<ObjectId> idsTable = new List<ObjectId>();
                Point3d ptCurTable = Point3d.Origin;
                foreach (var sec in scheme.Sections)
                {
                    Table table = getTable(sec);
                    table.Position = ptCurTable;
                    table.TransformBy(Matrix3d.Scaling(scale, table.Position));

                    ptCurTable = new Point3d(ptCurTable.X+table.Width+25*scale, ptCurTable.Y, 0);
                    
                    cs.AppendEntity(table);
                    t.AddNewlyCreatedDBObject(table, true);
                    idsTable.Add(table.Id);
                }

                // Вставка таблиц
                if (!DragSel.Drag(ed, idsTable.ToArray(), Point3d.Origin))
                {
                    foreach (var idTable in idsTable)
                    {
                        var table = idTable.GetObject(OpenMode.ForWrite);
                        table.Erase();
                    }
                }                                    
                t.Commit();
            }
        }
                
        private Table getTable(Calcs.Section sec)
        {
            Table table = new Table();
            table.SetDatabaseDefaults(db);
            table.TableStyle = db.GetTableStylePIK();

            var counters = sec.Floors.SelectMany (s => s.Counters);
            int rowsTotal = counters.Count() + 2;

            table.SetSize(rowsTotal, 4);
            table.SetBorders(LineWeight.LineWeight050);
            table.SetRowHeight(8);

            // Название таблицы
            var rowTitle = table.Cells[0, 0];
            rowTitle.Alignment = CellAlignment.MiddleCenter;
            rowTitle.TextHeight = 3;
            rowTitle.TextString = sec.Name + " " + sec.USPD;

            // столбец Этаж
            var col = table.Columns[0];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 15;
            // столбец №кв
            col = table.Columns[1];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 15;
            // столбец №сч
            col = table.Columns[2];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 20;
            // столбец №сч заводской
            col = table.Columns[3];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 30;

            // Заголовок Этаж
            var cellColName = table.Cells[1, 0];
            cellColName.TextString = "Этаж";
            // Заголовок №кв
            cellColName = table.Cells[1, 1];
            cellColName.TextString = "№ кв.";
            // Заголовок №сч
            cellColName = table.Cells[1, 2];
            cellColName.TextString = "№ счетчика в сети";
            // Заголовок №сч заводской
            cellColName = table.Cells[1, 3];
            cellColName.TextString = "№ счетчика заводской";
            

            // Строка заголовков столбцов
            var rowHeaders = table.Rows[1];
            rowHeaders.Height = 15;
            var lwBold = rowHeaders.Borders.Top.LineWeight;
            rowHeaders.Borders.Bottom.LineWeight = lwBold;

            int row = 2;            
            foreach (var counter in counters)
            {
                // Этаж
                table.Cells[row, 0].TextString = counter.Floor.ToString();
                // Кв
                table.Cells[row, 1].TextString = counter.ApartmentNumber.ToString();
                // №сч
                table.Cells[row, 2].TextString = counter.Number.ToString("000");                
                row++;
            }

            var lastRow = table.Rows.Last();
            lastRow.Borders.Bottom.LineWeight = lwBold;

            table.GenerateLayout();
            return table;
        }       
    }
}
