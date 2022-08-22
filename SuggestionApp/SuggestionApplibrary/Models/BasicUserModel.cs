using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuggestionApplibrary.Models
{
    public class BasicUserModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string DisplayName { get; set; }

        public BasicUserModel ()
        {

        }

        public BasicUserModel (UserModel user)
        {
            Id = user.Id;
            DisplayName = user.DisplayName;
        }
    }
}
