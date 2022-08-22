using MongoDB.Bson.Serialization.Attributes;

namespace SuggestionApplibrary.Models
{
    public class UserModel
    {
        //Mongo db objectId
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string ObjectIdentifier { get; set; } //important to Azure Active directory
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public List<BasicSuggestionModel> AuthoredSuggestions { get; set; } = new();
        public List<BasicSuggestionModel> VotedOnSuggestions { get; set; } = new();
    }
}
