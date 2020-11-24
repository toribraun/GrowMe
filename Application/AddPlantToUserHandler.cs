using System.Data.Common;

namespace Application
{
    public class AddPlantToUserHandler
    {
        public void Do(long userId, string plantName)
        {
            // var userRecord = db.GetUser(userId);
            // var user = UserConvertor.FromRecord(userRecord);
            //
            // user.AddPlant(plantName);
            // var newUserRecord = UserConvertor.ToRecord(user);
            // db.SaveUser(newUserRecord);
        }
    }
}