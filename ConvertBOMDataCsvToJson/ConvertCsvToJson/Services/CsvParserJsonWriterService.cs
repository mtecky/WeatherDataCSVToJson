using CsvHelper;
using ConvertCsvToJson.Mappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ConvertCsvToJson.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ConvertCsvToJson.Services
{
    public class CsvParserJsonWriterService : ICsvParserJsonWriterService
    {
        public List<WeatherDataModel> ReadCsvFile(string path)
        {
            try
            {
                using (var reader = new StreamReader(path, Encoding.Default))
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.RegisterClassMap<WeatherDataMap>();
                    var records = csv.GetRecords<WeatherDataModel>().ToList();
                    return records;
                }
            }
            catch (FieldValidationException e)
            {
                throw new Exception(e.Message);
            }
            catch (CsvHelperException e)
            {
                throw new Exception(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public void WriteJsonFile(Root root, string filePath)
        {
            try
            {
                IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
                timeFormat.DateTimeFormat = "yyyy-MM-dd";
                string json = JsonConvert.SerializeObject(root, Newtonsoft.Json.Formatting.Indented, timeFormat);

                File.WriteAllText(Path.Combine(filePath, $"WeatherData_{DateTime.Now:yyyyMMdd_hhmmss}.json"), json);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
