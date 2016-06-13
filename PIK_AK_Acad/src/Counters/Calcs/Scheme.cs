using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIK_AK_Acad.Counters.Calcs
{
    /// <summary>
    /// Схема счетчиков - из Excel
    /// </summary>
    public class Scheme
    {
        /// <summary>
        /// имя жилого дома - "Жилой дом Бунинский 1.4/2"
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Секции в доме
        /// </summary>
        public List<Section> Sections { get; set; } = new List<Section>();
    }
}
