using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainTravelAgency.Models;
using TrainTravelAgency.Services;
using TrainTravelAgency.Exceptions;

namespace TrainTravelAgency.Fakes
{
    public class Fus : IUserService
    {
        //property 
        private User _user;
        //sada dodajemo i exeption 
        private bool _shouldThrow;
        //konstruktor
        public Fus(User user, bool shouldThrow)
        {
            _user = user;
            _shouldThrow = shouldThrow;
        }
        public Fus(User user)
        {
            _user = user;
            
        }
        // metoda(stub)
        public User GetUserById(Guid userId)
        {            
            if (_shouldThrow == true)
            {
                throw new ExternalServiceErrorException("External user service error.");
            }
            return _user;
        }
    }
}
