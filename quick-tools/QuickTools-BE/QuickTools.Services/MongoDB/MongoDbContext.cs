using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using QuickTools.Core.Models;

namespace QuickTools.Services.MongoDB
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            try
            {
                // Validate connection string trước
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new ArgumentException(
                        "MongoDB connection string is empty.",
                        nameof(connectionString));
                }

                if (string.IsNullOrWhiteSpace(databaseName))
                {
                    throw new ArgumentException(
                        "MongoDB database name is empty.",
                        nameof(databaseName));
                }

                // Register mapping chỉ một lần
                RegisterClassMaps();

                // Nếu connection string sai format,
                // MongoClient sẽ throw exception ở đây
                var client = new MongoClient(connectionString);

                _database = client.GetDatabase(databaseName);
            }
            catch (MongoConfigurationException ex)
            {
                throw new InvalidOperationException(
                    $"MongoDB connection string is invalid: {ex.Message}",
                    ex);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException(
                    $"MongoDB configuration is invalid: {ex.Message}",
                    ex);
            }
        }

        private static void RegisterClassMaps()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Entity)))
            {
                BsonClassMap.RegisterClassMap<Entity>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIsRootClass(true);

                    cm.MapMember(c => c.Id)
                        .SetIdGenerator(StringObjectIdGenerator.Instance)
                        .SetSerializer(new StringSerializer(BsonType.ObjectId))
                        .SetElementName("_id");
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(IconModel)))
            {
                BsonClassMap.RegisterClassMap<IconModel>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIsRootClass(false);
                });
            }
        }

        public IMongoCollection<IconModel> Icons =>
            _database.GetCollection<IconModel>("Icons");

        public async Task<bool> CheckConnectionAsync()
        {
            try
            {
                await _database.RunCommandAsync<BsonDocument>(
                    new BsonDocument("ping", 1)
                );
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MongoDB connection failed: {ex.Message}");
                return false;
            }
        }
    }
}