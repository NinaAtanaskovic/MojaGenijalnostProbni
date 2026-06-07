using System;


namespace TrainTravelAgency.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public Guid FromCityId { get; set; }
        public Guid ToCityId { get; set; }
        public TicketType Type { get; set; }
        public double Price { get; set; }
        public Guid UserId { get; set; }
    }
}
