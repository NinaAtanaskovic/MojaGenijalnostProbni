using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TrainTravelAgency.Models;

namespace TrainTravelAgency.Test
{
    /// <summary>
    /// Parser klasa za citanje PICT Results fajla i generisanje NUnit TestCaseData objekata
    /// koji se koriste kao TestCaseSource u testovima F1 funkcionalnosti (RecommendTicketType).
    /// </summary>
    public static class PictParser
    {
        private static readonly string PictResultsPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "PICT Results.txt");

        /// <summary>
        /// Parsira PICT Results.txt fajl i vraca kolekciju TestCaseData objekata.
        /// Format fajla: tab-separated header + data rows.
        /// Kolone: SeatType, LuggageWeight, Beverage, TravelHour, ExpectedResult
        /// </summary>
        public static IEnumerable<TestCaseData> GetTestCases()
        {
            string[] lines = File.ReadAllLines(PictResultsPath);

            // Prva linija je header, preskacemo je
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split('\t');
                if (parts.Length < 5)
                    continue;

                SeatType seatType = (SeatType)Enum.Parse(typeof(SeatType), parts[0].Trim());
                double luggageWeight = double.Parse(parts[1].Trim());
                bool beverage = bool.Parse(parts[2].Trim());
                int travelHour = int.Parse(parts[3].Trim());
                string expectedResultStr = parts[4].Trim();

                TicketType? expectedResult = expectedResultStr.Equals("null", StringComparison.OrdinalIgnoreCase)
                    ? (TicketType?)null
                    : (TicketType)Enum.Parse(typeof(TicketType), expectedResultStr);

                yield return new TestCaseData(seatType, luggageWeight, beverage, travelHour, expectedResult)
                    .SetName($"RecommendTicketType_{seatType}_{luggageWeight}kg_Beverage={beverage}_Hour={travelHour}_Expected={expectedResultStr}");
            }
        }
    }
}
