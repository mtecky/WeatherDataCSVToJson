using System;

namespace ConvertCsvToJson
{
    public static class Constants
    {
        public class CsvHeaders
        {
            public const string ProductCode = "Product code";
            public const string BOMStationNumber = "Bureau of Meteorology station number";
            public const string Year = "Year";
            public const string Month = "Month";
            public const string Day = "Day";
            public const string RainfallAmount = "Rainfall amount (millimetres)";
            public const string Period = "Period over which rainfall was measured (days)";
            public const string Quality = "Quality";
        }
    }
}
