using System;
using TrainTravelAgency.Exceptions;
using TrainTravelAgency.Services;

namespace TrainTravelAgency.Fakes
{
    public class FakeDistanceCalculationService : IDistanceCalculationService
    {
        private readonly double _distanceInMiles;
        private readonly bool _shouldThrow;

        public FakeDistanceCalculationService(double distanceInMiles, bool shouldThrow = false)
        {
            _distanceInMiles = distanceInMiles;
            _shouldThrow = shouldThrow;
        }

        public double CalculateDistance(Guid source, Guid destination)
        {
            if (_shouldThrow)
            {
                throw new ExternalServiceErrorException("External distance calculation service error.");
            }
            return _distanceInMiles;
        }
    }
}
