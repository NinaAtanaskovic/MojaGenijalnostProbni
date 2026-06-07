using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainTravelAgency.Services;

namespace TrainTravelAgency.Fakes
{
    public class Fdc : IDistanceCalculationService

    {
        //property
        private readonly double Distance;

        //konstrukort da bi mogli da prolsedimo sta ocemo da namestimo kao resenje
        public Fdc(double distance)
        {
            Distance = distance;
        }

        //stub gde je namesteno resenje
        public double CalculateDistance(Guid source, Guid destination)
        {
            return Distance;
        }
    }

}
