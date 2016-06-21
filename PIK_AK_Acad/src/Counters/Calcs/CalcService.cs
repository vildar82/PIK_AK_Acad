using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace PIK_AK_Acad.Counters.Calcs
{
    public class CalcService
    {
        const string reqiredExcelFileName = "Таблица парам. счетчиков.xlsx";
        const string startCellName = "Схема счетчиков";
        const int startCellMaxRow = 25;
        const int startCellMaxColumn = 25;
        const int maxFloors = 50;

        string fileXls;

        /// <summary>
        /// Расчет счетчиков в Excel файле - рядом с чертежом dwg
        /// </summary>
        /// <param name="dwgFile">Полный путь к чертежу</param>
        /// <returns>Схема счетчиков</returns>
        public Scheme Calc(string dwgFile)
        {
            Scheme scheme = new Scheme();

            fileXls = getExcelfileScheme(dwgFile);

            using (var xlPackage = new ExcelPackage())
            {
                using (var stream = File.OpenRead(fileXls))
                {
                    xlPackage.Load(stream);
                }
                var worksheet = xlPackage.Workbook.Worksheets[1];

                // Стартовая ячейка схемы
                var cellstartScheme = getCellStartScheme(worksheet);
                // Имя жилого дома схемы
                //scheme.Name = worksheet.Cells[cellstartScheme.Row - 1, cellstartScheme.Column].Text.Trim();
                // определение первого этажа и последнего
                int colFloors = cellstartScheme.Column;
                int rowFloorLast = cellstartScheme.Row + 1;
                int floorFirstNumber;
                // Этажи должны идти по возрастанию нумераци 1,2 и т.д. до макс 50
                int rowFloorFirst = getRowFloorFirst(worksheet, colFloors, rowFloorLast, out floorFirstNumber);
                // Определение Секций
                scheme.Sections = getSections(worksheet, rowFloorFirst+1, colFloors+1, rowFloorLast, floorFirstNumber);
            }

            // Нумерация счетчиков.
            counterNumbering(scheme);

            return scheme;
        }        

        private string getExcelfileScheme(string dwgFile)
        {            
            if (!File.Exists(dwgFile))
            {
                throw new Exception($"Файл чертежа не найден. Нужно запускать эту команду из чертежа сохраненного на диске, и рядом с файлом чертежа должен лежать файл схемы счетчиков 'Таблица парам. счетчиков.xlsx'.");
            }

            string fileExel = Path.Combine(Path.GetDirectoryName(dwgFile), reqiredExcelFileName);
            if (!File.Exists(fileExel))
            {
                throw new Exception($"Не найден файл схемы счетчиков '{reqiredExcelFileName}', он должен лежать в той же папке что и чертеж из которого запускается команда - {dwgFile}");
            }

            return fileExel;
        }

        private ExcelCellAddress getCellStartScheme(ExcelWorksheet worksheet)
        {            
            for (int r = 1; r < startCellMaxRow; r++)
            {
                for (int c = 1; c < startCellMaxColumn; c++)
                {
                    if (worksheet.Cells[r, c].Text.TrimStart().StartsWith(startCellName, StringComparison.OrdinalIgnoreCase))
                    {
                        return new ExcelCellAddress(r, c);
                    }
                }
            }
            throw new Exception($"Не найдена стартовая ячейка '{startCellName}'");
        }

        /// <summary>
        /// Определение строки первого эьажа
        /// </summary>        
        private int getRowFloorFirst(ExcelWorksheet worksheet, int colFloors, int rowFloorLast, out int numberFirstFloor)
        {
            int rowFloorFirst = rowFloorLast;
            int floorNumberPrev = getFloorNumber(colFloors, rowFloorLast, worksheet.Cells[rowFloorLast, colFloors].Text);
            floorNumberPrev++;
            if (floorNumberPrev > maxFloors)
            {
                throw new Exception($"Определено больше допустимого кол этажей. Максимальное кол этажей - {maxFloors}. Столбец этажей {colFloors}, строка этажа {rowFloorFirst}, в файле '{fileXls}'.");
            }
            string floorText = worksheet.Cells[rowFloorFirst, colFloors].Text.Trim();
            while (!string.IsNullOrEmpty(floorText))
            {
                // Проверка номера этажа                                                
                int curFloorNumber = getFloorNumber(colFloors, rowFloorFirst, floorText);

                if (floorNumberPrev - curFloorNumber != 1)
                {
                    throw new Exception($"Нарушен порядок нумерации этажей в ячейке [{rowFloorFirst},{colFloors}]={floorText}. Файл '{fileXls}'.");
                }
                floorNumberPrev = curFloorNumber;
                rowFloorFirst++;
                floorText = worksheet.Cells[rowFloorFirst, colFloors].Text.Trim();
            }
            rowFloorFirst--;
            numberFirstFloor = floorNumberPrev;
            return rowFloorFirst;
        }

        private int getFloorNumber(int colFloors, int rowFloor, string floorText)
        {
            var matchs = Regex.Match(floorText, @"(^этаж )(\d{1,2})$", RegexOptions.IgnoreCase);
            if (matchs.Success)
            {
                var res = matchs.Groups[2].Value;
                int floor;
                int.TryParse(res, out floor);
                if (floor == 0) throw new Exception();
                return floor;
            }
            else
            {
                throw new Exception($"Ошибка определение номера этажа в ячейке [{rowFloor},{colFloors}]={floorText}. Ячейка этажа должно иметь вид 'этаж 1'. Файл '{fileXls}'.");
            }
        }

        /// <summary>
        /// Определение секций из Excel
        /// </summary>                
        private List<Section> getSections(ExcelWorksheet worksheet, int rowSections, 
            int colSectionFirst, int rowFloorLast, int floorNumberFirst)
        {
            List<Section> sections = new List<Section>();            
            int curColSec = colSectionFirst;
            int rowUspd = rowSections + 1;
            string sectionText = worksheet.Cells[rowSections, curColSec].Text.Trim();

            while (!string.IsNullOrEmpty(sectionText))
            {
                Section sec = new Section();                
                sections.Add(sec);
                sec.Name = sectionText;
                sec.USPD = worksheet.Cells[rowUspd, curColSec].Text.Trim();

                // Этажи секции
                int curRowFloor = rowSections - 1;
                int curFloorNumber = floorNumberFirst;
                string floorText = worksheet.Cells[curRowFloor, curColSec].Text.Trim();
                while (!string.IsNullOrEmpty(floorText) && curRowFloor >= rowFloorLast)
                {
                    Floor floor = new Floor();
                    sec.Floors.Add(floor);
                    floor.Number = curFloorNumber;

                    var matchs = Regex.Match(floorText, @"(^\d{1,2})");
                    if (matchs.Success)
                    {
                        floor.ApartmentCount = int.Parse(matchs.Groups[1].Value);
                    }
                    else
                    {
                        throw new Exception($"Не определено кол квартир в ячейке [{curRowFloor},{curColSec}] = {floorText}. Файл '{fileXls}'.");
                    }

                    curRowFloor--;
                    curFloorNumber++;
                    floorText = worksheet.Cells[curRowFloor, curColSec].Text.Trim();
                }

                curColSec++;
                sectionText = worksheet.Cells[rowSections, curColSec].Text.Trim();
            }

            return sections;
        }

        /// <summary>
        /// Нумерация счетчиков
        /// </summary>        
        private void counterNumbering(Scheme scheme)
        {
            // Начиная с первой секции с первого этажа на котором есть квартиры
            // Нумерция счетчиков начинается с 001
            // Нумерация начинается с начала для нового УСПД
            int apartmentCount = 0;
            int counterCount = 0;
            string curUspd = string.Empty;
            foreach (var sec in scheme.Sections)
            {
                if (sec.USPD != curUspd)
                {
                    counterCount = 0;
                }
                curUspd = sec.USPD;

                foreach (var floor in sec.Floors)
                {                    
                    if (floor.ApartmentCount != 0)
                    {
                        for (int i = 1; i <= floor.ApartmentCount; i++)
                        {
                            apartmentCount++;
                            counterCount++;
                            Counter counter = new Counter
                            {
                                Section = sec.Name,
                                USPD = sec.USPD,
                                ApartmentNumber = apartmentCount,
                                Floor = floor.Number,
                                Number = counterCount                                
                            };
                            floor.Counters.Add(counter);
                        }
                    }
                }
            }
        }
    }
}
