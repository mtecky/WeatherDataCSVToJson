using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ConvertCsvToJson.Models
{
    public class WeatherData
    {
        [JsonProperty("WeatherDataForYear")]
        public IEnumerable<WeatherDataForYear> WeatherDataForYear { get; set; }
    }
}
