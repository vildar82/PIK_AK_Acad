using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIK_AK_Acad.Counters.Calc
{
    /// <summary>
    /// секция счетчиков
    /// </summary>
    public class Section
    {
        /// <summary>
        /// Номер секции
        /// </summary>
        public int Number { get; set; }         
        /// <summary>
        /// Номер УСПД
        /// </summary>
        public int USPD { get; set; }
        /// <summary>
        /// Этажи
        /// </summary>
        public List<Floor> Floors { get; set; }
    }
}
