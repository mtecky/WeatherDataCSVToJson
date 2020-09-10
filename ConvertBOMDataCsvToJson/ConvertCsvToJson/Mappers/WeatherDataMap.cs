using CsvHelper.Configuration;
using ConvertCsvToJson.Models;

namespace ConvertCsvToJson.Mappers
{
    public sealed class WeatherDataMap : ClassMap<WeatherDataModel>
    { 
        public WeatherDataMap()
        {
            Map(m => m.ProductCode).Name(Constants.CsvHeaders.ProductCode);
            Map(m => m.BOMStationNumber).Name(Constants.CsvHeaders.BOMStationNumber);
            Map(m => m.Year).Name(Constants.CsvHeaders.Year);
            Map(m => m.Month).Name(Constants.CsvHeaders.Month);
            Map(m => m.Day).Name(Constants.CsvHeaders.Day);
            Map(m => m.RainfallAmount).Name(Constants.CsvHeaders.RainfallAmount);
            Map(m => m.Period).Name(Constants.CsvHeaders.Period);
            Map(m => m.Quality).Name(Constants.CsvHeaders.Quality);
        }
    }
}
