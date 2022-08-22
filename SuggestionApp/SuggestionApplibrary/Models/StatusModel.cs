using MongoDB.Bson.Serialization.Attributes;

namespace SuggestionApplibrary.Models
{
    public class StatusModel
    {
        //Mongo db objectId
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string StatusName { get; set; }
        public string StatusDescription { get; set; }
    }
}
