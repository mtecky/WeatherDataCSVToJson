using System;
using System.Collections.Generic;
using System.Text;

namespace ConvertCsvToJson.Models.OutputJson
{
    public class WeatherDataBase
    {
        public DateTimeOffset FirstRecordedDate { get; set; }

        public DateTimeOffset LastRecordedDate { get; set; }

        public decimal TotalRainfall { get; set; }

        public decimal AverageDailyRainfall { get; set; }

        public int DaysWithNoRainfall { get; set; }

        public int DaysWithRainfall { get; set; }
    }
}
