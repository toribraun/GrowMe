using System.Collections.Generic;
using Domain;

namespace Infrastructure
{
    public interface IDataBase
    {
        void AddUser(User user);
        void AddPlant(Plant plant);
        IEnumerable<User> GetUsers();
        IEnumerable<Plant> GetPlantsByUser(User user);
        User GetUserById(long id);
        void UpdateUser(User currentUser);
        void UpdatePlant(Plant currentPlant);
        void DeletePlant(Plant currentPlant);
    }
}