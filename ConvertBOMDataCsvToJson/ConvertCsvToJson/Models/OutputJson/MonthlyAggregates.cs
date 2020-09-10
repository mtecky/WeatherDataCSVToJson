using System;
using System.Collections.Generic;
using System.Text;

namespace ConvertCsvToJson.Models
{
    public class MonthlyAggregates
    {
        public IEnumerable<WeatherDataForMonth> WeatherDataForMonth { get; set; }
    }
}
