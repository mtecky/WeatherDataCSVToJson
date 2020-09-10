using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ConvertCsvToJson.Models.OutputJson;
using System.Linq;

namespace ConvertCsvToJson.Models
{
    public class WeatherDataForMonth : WeatherDataBase
    {
        public string Month { get; set; }

        public decimal MedianDailyRainfall { get; set; }

    }
}
