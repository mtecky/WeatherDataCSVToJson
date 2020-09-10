using System;
using System.Collections.Generic;
using System.Text;
using ConvertCsvToJson.Models;
using ConvertCsvToJson.Services;
using Microsoft.Extensions.Configuration;
using NLog;

namespace ConvertBOMDataCsvToJson
{
    public class CsvToJsonManager
    {
        private readonly IConfiguration Configuration;

        public CsvToJsonManager(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConvertCsvToJson(string csvFilePath)
        {

            // Read CSV
            var csvParserService = new CsvParserJsonWriterService();
            var result = csvParserService.ReadCsvFile(csvFilePath);

            // Convert to Object
            var convertDataService = new ConvertDataService();
            Root root = convertDataService.ConvertCSVToJson(result);

            // Write json File
            var jsonfilePath = Configuration["jsonfilePath"];
            csvParserService.WriteJsonFile(root, jsonfilePath);

        }

    }
}
