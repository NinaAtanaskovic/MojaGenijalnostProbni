using System;
using TrainTravelAgency.Exceptions;
using TrainTravelAgency.Models;
using TrainTravelAgency.Services;

namespace TrainTravelAgency.Fakes
{
    public class FakeUserService : IUserService
    {
        private readonly User _user;
        private readonly bool _shouldThrow;

        public FakeUserService(User user, bool shouldThrow)
        {
            _user = user;
        _shouldThrow = shouldThrow;
        }
        public FakeUserService(User user)
        {
            _user = user;
        }
        public User GetUserById(Guid userId)
        {
            if (_shouldThrow)
            {
                throw new ExternalServiceErrorException("External user service error.");
            }
            return _user;
        }
    }
}
