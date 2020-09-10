using System;
using System.Collections.Generic;
using System.Text;
using ConvertCsvToJson.Models;

namespace ConvertCsvToJson
{
    public interface ICsvParserJsonWriterService
    {
        List<WeatherDataModel> ReadCsvFile(string path);
        void WriteJsonFile(Root root, string filePath);
    }
}
