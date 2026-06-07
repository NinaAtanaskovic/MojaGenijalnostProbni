using System;
using TrainTravelAgency.Models;

namespace TrainTravelAgency.Services
{
    public interface IReservationService
    {
        TicketType? RecommendTicketType(SeatType seatType, double luggageWeight, bool beverage, int travelHour);
        double CalculateTicketPriceForUser(double distance, TicketType ticketType, Guid userId);
        double GetDistanceBetweenCities(Guid cityFromId, Guid cityToId);
    }
}
