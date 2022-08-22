

using Microsoft.Extensions.Caching.Memory;

namespace SuggestionApplibrary.DataAccess
{
    public class MongoSuggestionData : ISuggestionData
    {
        private readonly IDbConnection _db;
        private readonly IUserData _userData;
        private readonly IMemoryCache _cache;
        private readonly IMongoCollection<SuggestionModel> _suggestions;
        private const string cacheName = "SuggestionData";

        public MongoSuggestionData (IDbConnection db, IUserData userData, IMemoryCache cache)
        {
            _db = db;
            _userData = userData;
            _cache = cache;
            _suggestions = db.SuggestionCollection;
        }

        public async Task<List<SuggestionModel>> GetAllSuggestionsAsync ()
        {
            var output = _cache.Get<List<SuggestionModel>>(cacheName);

            if (output == null)
            {
                var results = await _suggestions.FindAsync(x => x.Archived == false);
                output = results.ToList();

                _cache.Set(cacheName, output, TimeSpan.FromMinutes(1));
            }

            return output;
        }

        public async Task<List<SuggestionModel>> GetAllAprovedSuggestions ()
        {
            var output = await GetAllSuggestionsAsync();
            return output.Where(x => x.ApprovedForRelease).ToList();
        }

        public async Task<SuggestionModel> GetSuggestion ( string id )
        {
            return (await _suggestions.FindAsync(x => x.Id == id)).FirstOrDefault();
        }

        public async Task<List<SuggestionModel>> GetAllSuggestionsWaitingForApproval ()
        {
            var output = await GetAllSuggestionsAsync();

            return output.Where(x => x.ApprovedForRelease == false && x.Rejected == false).ToList();
        }

        public async Task UpdateSuggestion ( SuggestionModel suggestion )
        {
            await _suggestions.ReplaceOneAsync(x => x.Id == suggestion.Id, suggestion);
            _cache.Remove(cacheName);
        }

        public async Task UpvoteSuggestion (string suggestionId, string userId )
        {
            var client = _db.Client;

            using var session = await client.StartSessionAsync();

            session.StartTransaction();

            try
            {
                var db = client.GetDatabase(_db.DBName);
                var suggestionsInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
                var suggestion = (await suggestionsInTransaction.FindAsync(x => x.Id == suggestionId)).First();

                bool isUpvote = suggestion.UserVotes.Add(userId); //return true/false

                if (isUpvote == false)
                {
                    suggestion.UserVotes.Remove(userId);
                }

                await suggestionsInTransaction.ReplaceOneAsync(x => x.Id == suggestionId, suggestion);

                var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
                var user = await _userData.GetUserAsync(suggestion.Author.Id);

                if (isUpvote)
                {
                    user.VotedOnSuggestions.Add(new BasicSuggestionModel(suggestion));
                }
                else
                {
                    var suggestionToRemove = user.VotedOnSuggestions.Where(x => x.Id == suggestionId).First();
                    user.VotedOnSuggestions.Remove(suggestionToRemove);
                }
                await usersInTransaction.ReplaceOneAsync(x => x.Id == userId, user);

                await session.CommitTransactionAsync();

                _cache.Remove(cacheName);
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                throw;
            }
        }

        public async Task CreateSuggestion ( SuggestionModel suggestion )
        {
            var client = _db.Client;

            using var session = await client.StartSessionAsync();

            session.StartTransaction();

            try
            {
                var db = client.GetDatabase(_db.DBName);
                var suggestionsInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
                await suggestionsInTransaction.InsertOneAsync(suggestion);

                var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
                var user = await _userData.GetUserAsync(suggestion.Author.Id);
                user.AuthoredSuggestions.Add(new BasicSuggestionModel(suggestion));
                await usersInTransaction.ReplaceOneAsync(x => x.Id == user.Id, user);

                await session.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                throw;
            }
        }
    }
}
