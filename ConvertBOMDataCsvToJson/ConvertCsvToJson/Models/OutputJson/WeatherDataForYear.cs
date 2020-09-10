using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ConvertCsvToJson.Models.OutputJson;

namespace ConvertCsvToJson.Models
{
    public class WeatherDataForYear : WeatherDataBase
    {
        public int Year { get; set; }
        public int LongestNumberOfDaysRaining { get; set; }
        public MonthlyAggregates MonthlyAggregates { get; set; }

    }
}
