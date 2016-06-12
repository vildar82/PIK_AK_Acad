using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace PIK_AK_Acad.Counters
{
    public static class CounterService
    {
        /// <summary>
        /// Расчет счетчиков и вставка таблиц
        /// </summary>
        public static void CalcAndInsertTables (Document doc)
        {
            // Расчет счетчиков
            Calc.CalcService calcService = new Calc.CalcService();
            var scheme = calcService.Calc();                
        }
    }
}
