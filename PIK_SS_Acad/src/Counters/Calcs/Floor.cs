using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIK_SS_Acad.Counters.Calcs
{
    /// <summary>
    /// этаж
    /// </summary>
    public class Floor
    {
        /// <summary>
        /// номер этажа
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Кол квартир на этаже
        /// </summary>
        public int ApartmentCount { get; set; }
        /// <summary>
        /// Счетчики
        /// </summary>
        public List<Counter> Counters { get; set; } = new List<Counter>();
    }
}
