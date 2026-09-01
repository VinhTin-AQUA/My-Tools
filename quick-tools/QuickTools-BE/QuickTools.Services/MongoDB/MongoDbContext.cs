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
            // Đăng ký mapping cho Icon entity
            RegisterClassMaps();

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        private static void RegisterClassMaps()
        {
            // Đăng ký map cho Entity base class
            BsonClassMap.RegisterClassMap<Entity>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true); // Quan trọng: Đánh dấu là root class
                cm.MapMember(c => c.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId))
                    .SetElementName("_id"); // Map thành _id trong MongoDB
            });

            // Đăng ký map cho IconModel (kế thừa từ Entity)
            BsonClassMap.RegisterClassMap<IconModel>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(false); // Kế thừa từ Entity
            });
        }

        public IMongoCollection<IconModel> Icons => _database.GetCollection<IconModel>("Icons");
    }
}