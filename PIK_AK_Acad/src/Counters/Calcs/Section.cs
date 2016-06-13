using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIK_AK_Acad.Counters.Calcs
{
    /// <summary>
    /// секция счетчиков
    /// </summary>
    public class Section
    {
        /// <summary>
        /// Имя секции
        /// </summary>
        public string Name { get; set; }         
        /// <summary>
        /// Номер УСПД
        /// </summary>
        public string USPD { get; set; }
        /// <summary>
        /// Этажи
        /// </summary>
        public List<Floor> Floors { get; set; } = new List<Floor>();        
    }
}
