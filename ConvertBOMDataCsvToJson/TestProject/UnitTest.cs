using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using ConvertCsvToJson.Models;
using ConvertCsvToJson.Services;
using Xunit;

namespace TestProject
{
    public class UnitTest
    {
        private string _filePath;
        public UnitTest()
        {
            string[] directories = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()))
                .Split(Path.DirectorySeparatorChar);

            directories = directories.Take(directories.Count() - 1).ToArray();

            _filePath = $"{Path.Combine(directories.ToArray())}\\TestFiles\\";
        }

        [Fact]
        public void ReadCSV_TestRowCount()
        {
            CsvParserJsonWriterService cps = new CsvParserJsonWriterService();
            List<WeatherDataModel> weatherDataModel = cps.ReadCsvFile($"{_filePath}Full_Download_Data.csv");
            Assert.Equal(59413, weatherDataModel.Count);
        }

        [Fact]
        public void ReadCSV_TestHeaderRow()
        {
            CsvParserJsonWriterService cps = new CsvParserJsonWriterService();
            Assert.Throws<Exception>(() => cps.ReadCsvFile($"{_filePath}Invalid_Header_Error.csv"));
        }

        //[Fact]
        //public void Convert_IncorrectFormatInLastRow()
        //{
        //    CsvParserJsonWriterService cps = new CsvParserJsonWriterService();
        //    List<WeatherDataModel> weatherDataModel = cps.ReadCsvFile($"{_filePath}Incorrect_Format_Data_Last_Row.csv");
        //}


        [Theory]
        [InlineData(2015)]
        [InlineData(2016)]
        [InlineData(2017)]
        [InlineData(2018)]
        [InlineData(2019)]
        [InlineData(2020)]
        public void GetLongestNumberOfDaysRaining_Test(int year)
        {

            CsvParserJsonWriterService cps = new CsvParserJsonWriterService();
            List<WeatherDataModel> weatherDataModel = cps.ReadCsvFile($"{_filePath}Full_Download_Data.csv");

            var weatherDataModelForYear = (from x in weatherDataModel
                                           where int.Parse(x.Year) == year
                                           orderby x.Month, x.Day
                                           select x);

            int longestNumber = 0;
            int longestNumberSoFar = 0;

            foreach (var w in weatherDataModelForYear)
            {
                if (string.IsNullOrWhiteSpace(w.RainfallAmount))
                    continue;

                if (decimal.Parse(w.RainfallAmount) > 0)
                {
                    longestNumber++;
                }
                else
                {
                    longestNumberSoFar = longestNumber > longestNumberSoFar ? longestNumber : longestNumberSoFar;
                    longestNumber = 0;
                }
            }

            var convertDataService = new ConvertDataService();
            Root root = convertDataService.ConvertCSVToJson(weatherDataModel);

            var longestNoInObject =
                (from y in root.WeatherData.WeatherDataForYear
                 where y.Year == year
                 select y.LongestNumberOfDaysRaining).FirstOrDefault();

            Assert.Equal(longestNumberSoFar, longestNoInObject);

        }

        [Theory]
        [InlineData(9)]
        public void FutureDatesRemoved_Test(int monthNo)
        {
            CsvParserJsonWriterService cps = new CsvParserJsonWriterService();
            List<WeatherDataModel> weatherDataModel = cps.ReadCsvFile($"{_filePath}Data_With_Future_Dates.csv");

            var convertDataService = new ConvertDataService();
            Root root = convertDataService.ConvertCSVToJson(weatherDataModel);

            Assert.Equal(2020, root.WeatherData.WeatherDataForYear.LastOrDefault().Year);

            Assert.Equal(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNo),
                root.WeatherData.WeatherDataForYear.LastOrDefault().MonthlyAggregates.WeatherDataForMonth
                    .LastOrDefault().Month);

            // Future Data entered with 1.0 as Rainfall for each day until 20/Sep, test to ensure , data read is less than today's date
            if (DateTime.Now < new DateTime(2020, 09, 21))
            {
                Assert.Equal(DateTime.Now.Day,
                root.WeatherData.WeatherDataForYear.LastOrDefault().MonthlyAggregates.WeatherDataForMonth
                    .LastOrDefault().TotalRainfall);
            }

        }


        [Theory]
        [InlineData("1858")]
        [InlineData("2019")]
        public void FirstRecordedDate_Year_Test(string year)
        {
            CsvParserJsonWriterService cps = new CsvParserJsonWriterService();
            List<WeatherDataModel> weatherDataModel = cps.ReadCsvFile($"{_filePath}Full_Download_Data.csv");

            var convertDataService = new ConvertDataService();
            Root root = convertDataService.ConvertCSVToJson(weatherDataModel);


            List<DateTime> alldateTimes = new List<DateTime>(); 

            foreach (var w in weatherDataModel)
            {
                if (w.Year == year && !string.IsNullOrWhiteSpace(w.RainfallAmount))
                {
                    alldateTimes.Add(new DateTime(int.Parse(w.Year), int.Parse(w.Month), int.Parse(w.Day)));
                }
            }

            var theDRecord  = from y in root.WeatherData.WeatherDataForYear
                where y.Year == int.Parse(year)
                select y;

            var theDate = theDRecord.FirstOrDefault().FirstRecordedDate;

            Assert.Equal(theDate, alldateTimes.Min());

        }


        public static readonly object[][] DataTwo =
        {
            new object[] { "1858" , new DateTime(1858, 12,31)},
            new object[] { "2020", new DateTime(2020, 8, 31)}
        };


        [Theory]
        [InlineData("1858")]
        [InlineData("2020")]
        public void LastRecordedDate_Year_Test(string year)
        {
            CsvParserJsonWriterService cps = new CsvParserJsonWriterService();
            List<WeatherDataModel> weatherDataModel = cps.ReadCsvFile($"{_filePath}Full_Download_Data.csv");

            var convertDataService = new ConvertDataService();
            Root root = convertDataService.ConvertCSVToJson(weatherDataModel);


            List<DateTime> alldateTimes = new List<DateTime>();

            foreach (var w in weatherDataModel)
            {
                if (w.Year == year && !string.IsNullOrWhiteSpace(w.RainfallAmount))
                {
                    alldateTimes.Add(new DateTime(int.Parse(w.Year), int.Parse(w.Month), int.Parse(w.Day)));
                }
            }

            var theDRecord = from y in root.WeatherData.WeatherDataForYear
                where y.Year == int.Parse(year)
                select y;

            var theDate = theDRecord.FirstOrDefault().LastRecordedDate;

            Assert.Equal(theDate, alldateTimes.Max());


        }

        [Theory]
        [InlineData("2019")]
        [InlineData("2020")]
        public void TotalRainfall_Year_Test(string year)
        {
            CsvParserJsonWriterService cps = new CsvParserJsonWriterService();
            List<WeatherDataModel> weatherDataModel = cps.ReadCsvFile($"{_filePath}Full_Download_Data.csv");

            var convertDataService = new ConvertDataService();
            Root root = convertDataService.ConvertCSVToJson(weatherDataModel);


            List<DateTime> alldateTimes = new List<DateTime>();

            decimal totalRainfall = 0;

            foreach (var w in weatherDataModel)
            {
                if (w.Year == year && !string.IsNullOrWhiteSpace(w.RainfallAmount))
                {
                    totalRainfall = totalRainfall + decimal.Parse(w.RainfallAmount);
                }
            }

            var theDRecord = from y in root.WeatherData.WeatherDataForYear
                where y.Year == int.Parse(year)
                select y;

            var theTotalRainfall = theDRecord.FirstOrDefault().TotalRainfall;

            Assert.Equal(theTotalRainfall, totalRainfall);

        }

        [Theory]
        [InlineData("2019")]
        [InlineData("2020")]
        public void AverageDailyRainfall_Year_Test(string year)
        {
            CsvParserJsonWriterService cps = new CsvParserJsonWriterService();
            List<WeatherDataModel> weatherDataModel = cps.ReadCsvFile($"{_filePath}Full_Download_Data.csv");

            var convertDataService = new ConvertDataService();
            Root root = convertDataService.ConvertCSVToJson(weatherDataModel);


            List<decimal> totalRainfall = new List<decimal>();

            foreach (var w in weatherDataModel)
            {
                if (w.Year == year && !string.IsNullOrWhiteSpace(w.RainfallAmount))
                {
                    totalRainfall.Add(decimal.Parse(w.RainfallAmount));
                }
            }

            var theDRecord = from y in root.WeatherData.WeatherDataForYear
                where y.Year == int.Parse(year)
                select y;

            var theAverageDailyRainfall = theDRecord.FirstOrDefault().AverageDailyRainfall;

            Assert.Equal(theAverageDailyRainfall, Math.Round(totalRainfall.Average(), 3));
        }

        [Theory]
        [InlineData("2019")]
        [InlineData("2020")]
        public void DaysWithNoRainfall_Year_Test(string year)
        {
            CsvParserJsonWriterService cps = new CsvParserJsonWriterService();
            List<WeatherDataModel> weatherDataModel = cps.ReadCsvFile($"{_filePath}Full_Download_Data.csv");

            var convertDataService = new ConvertDataService();
            Root root = convertDataService.ConvertCSVToJson(weatherDataModel);

            int countDaysWithNoRainfall = 0;

            foreach (var w in weatherDataModel)
            {
                if (w.Year == year && !string.IsNullOrWhiteSpace(w.RainfallAmount) && decimal.Parse(w.RainfallAmount) == 0)
                {
                    countDaysWithNoRainfall += 1; 
                }
            }

            var theDRecord = from y in root.WeatherData.WeatherDataForYear
                where y.Year == int.Parse(year) 
                select y;

            var theDaysWithNoRainfall = theDRecord.FirstOrDefault().DaysWithNoRainfall;

            Assert.Equal(theDaysWithNoRainfall, countDaysWithNoRainfall);
        }

        [Theory]
        [InlineData("2019")]
        [InlineData("2020")]
        public void DaysWithRainfall_Year_Test(string year)
        {
            CsvParserJsonWriterService cps = new CsvParserJsonWriterService();
            List<WeatherDataModel> weatherDataModel = cps.ReadCsvFile($"{_filePath}Full_Download_Data.csv");

            var convertDataService = new ConvertDataService();
            Root root = convertDataService.ConvertCSVToJson(weatherDataModel);

            int countDaysWithRainfall = 0;

            foreach (var w in weatherDataModel)
            {
                if (w.Year == year && !string.IsNullOrWhiteSpace(w.RainfallAmount) && decimal.Parse(w.RainfallAmount) > 0)
                {
                    countDaysWithRainfall += 1;
                }
            }

            var theDRecord = from y in root.WeatherData.WeatherDataForYear
                where y.Year == int.Parse(year)
                select y;

            var theDaysWithRainfall = theDRecord.FirstOrDefault().DaysWithRainfall;

            Assert.Equal(theDaysWithRainfall, countDaysWithRainfall);

        }

     
        public void FirstRecordedDate_Month_Test(string year, string month)
        {
        }

        [Fact]
        public void LastRecordedDate_Month_Test()
        {

        }

        [Fact]
        public void TotalRainfall_Month_Test()
        {

        }

        [Fact]
        public void AverageDailyRainfall_Month_Test()
        {

        }

        [Fact]
        public void DaysWithNoRainfall_Month_Test()
        {

        }

        [Fact]
        public void DaysWithRainfall_Month_Test()
        {

        }

        [Fact]
        public void MedianDailyRainfall_Month_Test()
        {

        }






    }
}
