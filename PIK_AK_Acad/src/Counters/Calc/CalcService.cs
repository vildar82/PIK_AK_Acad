using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace PIK_AK_Acad.Counters.Calc
{
    public class CalcService
    {
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

            using (var xlPackage = new ExcelPackage(new FileInfo(fileXls)))
            {
                var worksheet = xlPackage.Workbook.Worksheets[1];

                // Стартовая ячейка схемы
                var cellstartScheme = getCellStartScheme(worksheet);
                // Имя жилого дома схемы
                scheme.Name = worksheet.Cells[cellstartScheme.Row + 1, cellstartScheme.Column].Text.Trim();
                // определение первого этажа и последнего
                int colFloors = cellstartScheme.Column;
                int rowFloorLast = cellstartScheme.Row + 2;
                int floorFirstNumber;
                // Этажи должны идти по возрастанию нумераци 1,2 и т.д. до макс 50
                int rowFloorFirst = getRowFloorFirst(worksheet, colFloors, rowFloorLast, out floorFirstNumber);
            }

            return scheme;
        }

        

        private string getExcelfileScheme(string dwgFile)
        {            
            if (!File.Exists(dwgFile))
            {
                throw new Exception($"Файл чертежа не найден. Нужно запускать эту команду из чертежа сохраненного на диске, и рядом с файлом чертежа должен лежать файл схемы счетчиков 'Таблица парам. счетчиков.xlsx'.");
            }

            string fileExel = Path.Combine(Path.GetDirectoryName(dwgFile), "Таблица парам. счетчиков.xlsx");
            if (!File.Exists(fileExel))
            {
                throw new Exception($"Не найден файл схемы счетчиков 'Таблица парам. счетчиков.xlsx', он должен лежать в той же папке что и чертеж из которого запускается команда - {dwgFile}");
            }

            return fileExel;
        }

        private ExcelCellAddress getCellStartScheme(ExcelWorksheet worksheet)
        {            
            for (int r = 0; r < startCellMaxRow; r++)
            {
                for (int c = 0; c < startCellMaxColumn; c++)
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
            if (floorNumberPrev > maxFloors)
            {
                throw new Exception($"Определено больше допустимого кол этажей. Максимальное кол этажей - {maxFloors}. Столбец этажей {colFloors}, строка этажа {rowFloorFirst}, в файле '{fileXls}'.");
            }            
            while (!string.IsNullOrEmpty(worksheet.Cells[rowFloorFirst, colFloors].Text.Trim()))
            {
                // Проверка номера этажа                                
                string floorText = worksheet.Cells[rowFloorFirst, colFloors].Text.Trim();
                int curFloorNumber = getFloorNumber(colFloors, rowFloorFirst, floorText);

                if (floorNumberPrev - curFloorNumber != 1)
                {
                    throw new Exception($"Нарушен порядок нумерации этажей в ячейке [{rowFloorFirst},{colFloors}]={floorText}. Файл '{fileXls}'.");
                }
                floorNumberPrev = curFloorNumber;
                rowFloorFirst++;                
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
    }
}
