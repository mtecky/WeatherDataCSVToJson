using System;
using CsvHelper.Configuration.Attributes;

namespace ConvertCsvToJson.Models
{
    public class WeatherDataModel
    {
        [Name(Constants.CsvHeaders.ProductCode)]
        public string ProductCode { get; set; }

        [Name(Constants.CsvHeaders.BOMStationNumber)]
        public string BOMStationNumber { get; set; }

        [Name(Constants.CsvHeaders.Year)]
        public string Year { get; set; }

        [Name(Constants.CsvHeaders.Month)]
        public string Month { get; set; }

        [Name(Constants.CsvHeaders.Day)]
        public string Day { get; set; }

        [Name(Constants.CsvHeaders.RainfallAmount)]
        public string RainfallAmount { get; set; }

        [Name(Constants.CsvHeaders.Period)]
        public string Period { get; set; }

        [Name(Constants.CsvHeaders.Quality)]
        public string Quality { get; set; }

        public DateTime FullDate => new DateTime(int.Parse(Year), int.Parse(Month), int.Parse(Day));
    }
}
 