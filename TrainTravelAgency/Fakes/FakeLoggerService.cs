using System.Collections.Generic;
using TrainTravelAgency.Services;

namespace TrainTravelAgency.Fakes
{
    public class FakeLoggerService : ILoggerService
    {
        public List<string> LoggedMessages { get; } = new List<string>();

        public void LogError(string message)
        {
            LoggedMessages.Add(message);
        }
    }
}
