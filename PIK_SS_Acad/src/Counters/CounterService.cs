using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace PIK_SS_Acad.Counters
{
    public static class CounterService
    {
        /// <summary>
        /// Расчет счетчиков и вставка таблиц
        /// </summary>
        public static void CalcAndInsertTables (Document doc)
        {
            // Расчет счетчиков
            Calcs.CalcService calcService = new Calcs.CalcService();
            var scheme = calcService.Calc(doc.Name);

            // Вставка таблицы
            Tables.TableService tableService = new Tables.TableService(doc);
            tableService.CreateAndInsert(scheme);
        }
    }
}
