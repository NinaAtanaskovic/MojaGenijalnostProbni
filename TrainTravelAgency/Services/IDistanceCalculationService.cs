using System;


namespace TrainTravelAgency.Services
{
    public interface IDistanceCalculationService
    {
        double CalculateDistance(Guid source, Guid destination);
    }
}
