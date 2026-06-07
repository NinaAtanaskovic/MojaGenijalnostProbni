using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TrainTravelAgency.Models;
using System.IO;
using System.Collections;

namespace TrainTravelAgency.Test
{
    public class MojPARSER
    {
        public static IEnumerable Parsiraj (string filename)
        {
            
            string path = $@"{AppDomain.CurrentDomain.BaseDirectory}\{filename}";
            string[] lines=File.ReadAllLines(path);
            List<TestCaseData> testCases = new List<TestCaseData>();
            foreach(string line in lines)
            {
                string[] values = line.Split('\t');
                SeatType tipSedista= (SeatType)Enum.Parse(typeof(SeatType), values[0]);
                double tezinaPrtljaga = double.Parse(values[1]);
                int satiPutovanja = int.Parse(values[2]);
                bool pice = bool.Parse(values[3]);
                TicketType ? karta;
                if (values[4].Equals("null"))
                    karta = null;
                else
                    karta = (TicketType)Enum.Parse(typeof(TicketType), values[4]);

                testCases.Add(new TestCaseData(tipSedista, tezinaPrtljaga, satiPutovanja, pice, karta));


            }
            return testCases;
        }
    }
}
