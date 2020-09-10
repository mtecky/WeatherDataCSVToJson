using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using ConvertCsvToJson.Interface;
using ConvertCsvToJson.Models;

namespace ConvertCsvToJson.Services
{
    public class ConvertDataService : IConvertDataService
    {
        public Root ConvertCSVToJson(IEnumerable<WeatherDataModel> weatherDataModel)
        {
            try
            {
                var wdy = from c in weatherDataModel
                          where (!string.IsNullOrWhiteSpace(c.RainfallAmount))
                          && c.FullDate <= DateTime.Now
                          group c by c.Year
                into grouped
                          select new WeatherDataForYear
                          {
                              Year = int.Parse(grouped.Key),
                              FirstRecordedDate = grouped.Min(c => c.FullDate),
                              LastRecordedDate = grouped.Max(c => c.FullDate),
                              TotalRainfall = grouped.Sum(c => decimal.Parse(c.RainfallAmount)),
                              AverageDailyRainfall = Math.Round(grouped.Average(c => decimal.Parse(c.RainfallAmount)), 3),
                              DaysWithNoRainfall = DaysWithNoRainfall(grouped),
                              DaysWithRainfall = DaysWithRainfall(grouped),
                              LongestNumberOfDaysRaining = GetLongestNumberOfDaysRaining(grouped),
                              MonthlyAggregates = GetMonthlyAggregates(grouped)
                          };

                var root = new Root();
                var weatherData = new WeatherData
                {
                    WeatherDataForYear = wdy
                };
                root.WeatherData = weatherData;

                return root;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private int DaysWithNoRainfall(IEnumerable<WeatherDataModel> wdm)
        {
            var q = from w in wdm
                    where !string.IsNullOrWhiteSpace(w.RainfallAmount) && decimal.Parse(w.RainfallAmount) == 0
            select w;

            return q.Count();
        }

        private int DaysWithRainfall(IEnumerable<WeatherDataModel> wdm)
        {
            var q = from w in wdm
                where !string.IsNullOrWhiteSpace(w.RainfallAmount) && decimal.Parse(w.RainfallAmount) > 0
                select w;

            return q.Count();
        }


        private static int GetLongestNumberOfDaysRaining(IEnumerable<WeatherDataModel> wdm)
        {
            int maxLength = 0;
            int tempLength = 0;

            foreach (var v in wdm)
            {
                if (decimal.Parse(v.RainfallAmount) > 0)
                {
                    tempLength++;
                }
                else
                {
                    tempLength = 0;
                }

                if (tempLength > maxLength)
                {
                    maxLength = tempLength;
                }
            }

            return maxLength;
        }

        private MonthlyAggregates GetMonthlyAggregates(IEnumerable<WeatherDataModel> wdm)
        {
            var monthlyAggregates = new MonthlyAggregates();

            var weatherDataForMonth = from c in wdm
                                      group c by c.Month
                into grouped
                                      select new WeatherDataForMonth
                                      {
                                          Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(grouped.Key)),
                                          FirstRecordedDate = grouped.Min(c => c.FullDate),
                                          LastRecordedDate = grouped.Max(c => c.FullDate),
                                          TotalRainfall = grouped.Sum(c => decimal.Parse(c.RainfallAmount)),
                                          AverageDailyRainfall = Math.Round(grouped.Average(c => decimal.Parse(c.RainfallAmount)), 3),
                                          DaysWithNoRainfall = DaysWithNoRainfall(grouped),
                                          DaysWithRainfall = DaysWithRainfall(grouped),
                                          MedianDailyRainfall = (decimal)FindMedianOfWeatherData(grouped)
                                      };

            monthlyAggregates.WeatherDataForMonth = weatherDataForMonth;

            return monthlyAggregates;
        }

        public double FindMedianOfWeatherData(IGrouping<string, WeatherDataModel> wdm)
        {

            var ra = wdm.Select(x => decimal.Parse(x.RainfallAmount));

            var n = ra.Count();
            if (n % 2 != 0)
                return (double)ra.ElementAt(n / 2);

            return (double)(ra.ElementAt((n - 1) / 2) + ra.ElementAt(n / 2)) / 2.0;
        }
    }
}
