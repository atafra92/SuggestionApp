using MongoDB.Bson.Serialization.Attributes;

namespace SuggestionApplibrary.Models
{
    public class CategoryModel 
    { 
        //Mongo db objectId
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
    }
}
