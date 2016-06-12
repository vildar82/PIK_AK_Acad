using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIK_AK_Acad.Counters.Calc
{
    /// <summary>
    /// Счетчик - одно подключение
    /// </summary>
    public class Counter
    {
        /// <summary>
        /// Номер этажа
        /// </summary>
        public int Floor { get; set; }
        /// <summary>
        /// Номер квартиры
        /// </summary>
        public int ApartmentNumber { get; set; }
        /// <summary>
        /// Номер счетчика в сети (УСПД)
        /// </summary>
        public int Number { get; set; }
    }
}
