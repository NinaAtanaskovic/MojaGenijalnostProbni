using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainTravelAgency.Services;

namespace TrainTravelAgency.Fakes
{
    public class Fls : ILoggerService
    {   
        public string Message  { get; set; }

        public void LogError(string message)
        {
            Message = message;
        }
    }
}
