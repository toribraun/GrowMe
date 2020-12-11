using System;

namespace Application.Replies
{
    public class ReplyOnGetPlantPhoto : IReply
    {
        public long UserId { get; }
        public string PlantName { get; }
        public string FirstPhotoId { get; }
        public string LastPhotoId { get; }
        public DateTime AddingDatetime { get; }
        public bool IsExist { get; }

        public ReplyOnGetPlantPhoto(long userId, string plantName,
            string firstPhotoId, string lastPhotoId,
            DateTime addingDatetime, bool isExist)
        {
            UserId = userId;
            PlantName = plantName;
            FirstPhotoId = firstPhotoId;
            LastPhotoId = lastPhotoId;
            AddingDatetime = addingDatetime;
            IsExist = isExist;
        }
        
        public ReplyOnGetPlantPhoto(long userId, bool isExist)
        {
            UserId = userId;
            IsExist = isExist;
        }
    }
}