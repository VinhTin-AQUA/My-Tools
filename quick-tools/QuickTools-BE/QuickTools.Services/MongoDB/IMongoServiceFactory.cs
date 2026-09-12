using QuickTools.Services.Icons;

namespace QuickTools.Services.MongoDB
{
    public interface IMongoServiceFactory
    {
        (MongoDbContext, IconService) CreateIconService(string connectionString, string databaseName);
    }

    public class MongoServiceFactory : IMongoServiceFactory
    {
        public (MongoDbContext, IconService) CreateIconService(string connectionString, string databaseName)
        {
            var context = new MongoDbContext(connectionString, databaseName);
            return (context, new IconService(context));
        }
    }
}