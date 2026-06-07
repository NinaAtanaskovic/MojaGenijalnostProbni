using System;


namespace TrainTravelAgency.Exceptions
{
    public class ExternalServiceErrorException: Exception
    {
        public ExternalServiceErrorException()
        {

        }

        public ExternalServiceErrorException(string message): base(message)
        {

        }
    }
}
