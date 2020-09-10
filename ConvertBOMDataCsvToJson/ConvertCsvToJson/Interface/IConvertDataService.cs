using System;
using System.Collections.Generic;
using System.Text;
using ConvertCsvToJson.Models;

namespace ConvertCsvToJson.Interface
{
    interface IConvertDataService
    {
        Root ConvertCSVToJson(IEnumerable<WeatherDataModel> weatherDataModel);
    }
}
