using Microsoft.Extensions.Configuration;
using NLog;
using System;

namespace ConvertBOMDataCsvToJson
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                
                IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

                Console.WriteLine("Read BOM Weather Data in CSV and Return in JSON");
                CsvToJsonManager csvToJsonManager = new CsvToJsonManager(configuration);

                csvToJsonManager.ConvertCsvToJson(args[0]);

                Console.WriteLine("Conversion completed!");
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Stopped program because of exception: {ex}");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }

        }
    }
}
