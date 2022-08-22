using MongoDB.Driver;

namespace SuggestionApplibrary.DataAccess
{
    public class MongoUserData : IUserData
    {
        private readonly IMongoCollection<UserModel> _users;
        public MongoUserData ( IDbConnection db )
        {
            _users = db.UserCollection;
        }

        public async Task<List<UserModel>> GetUsersAsync ()
        {
            var result = await _users.FindAsync(_ => true); //returns all records from db
            return result.ToList();
        }

        public async Task<UserModel> GetUserAsync ( string id )
        {
            var results = await _users.FindAsync(x => x.Id == id);
            return results.FirstOrDefault();
        }

        public async Task<UserModel> GetUserFromAuthenticationAsync ( string objectId )
        {
            var results = await _users.FindAsync(x => x.ObjectIdentifier == objectId); //for Azure authentication
            return results.FirstOrDefault();
        }

        public Task CreateUser ( UserModel user )
        {
            return _users.InsertOneAsync(user);
        }

        public Task UpdateUser ( UserModel user )
        {
            var filter = Builders<UserModel>.Filter.Eq("Id", user.Id);
            return _users.ReplaceOneAsync(filter, user, options: new ReplaceOptions { IsUpsert = true });
            //if there is no user with id provided it will insert one, but if it does find a user it will update
        }
    }
}
