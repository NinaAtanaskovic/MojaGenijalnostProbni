using System;


namespace TrainTravelAgency.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int NumberOfTicketsPurchasedInTheLastMonth {get; set;}
    }
}
