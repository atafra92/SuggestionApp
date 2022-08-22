using MongoDB.Bson.Serialization.Attributes;

namespace SuggestionApplibrary.Models;

public class SuggestionModel
{
    //Mongo db objectId
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    public string Suggestion { get; set; }
    public string Description { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public CategoryModel Category { get; set; }
    public BasicUserModel Author { get; set; }
    public HashSet<string> UserVotes { get; set; } = new(); //list that has to be conststed of unique values, will not allow not unique values insert
    public StatusModel Status { get; set; }
    public string OwnerNotes { get; set; }
    public bool ApprovedForRelease { get; set; } //by default false
    public bool Archived { get; set; }
    public bool Rejected { get; set; }
}
