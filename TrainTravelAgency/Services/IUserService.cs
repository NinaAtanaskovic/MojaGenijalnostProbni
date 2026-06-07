using System;

using TrainTravelAgency.Models;

namespace TrainTravelAgency.Services
{
    public interface IUserService
    {
        User GetUserById(Guid userId);
    }
}
