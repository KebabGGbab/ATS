using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ATS
{
    public static class RequestToMongoDB
    {
        private static readonly MongoClient mongoClient;
        private static readonly IMongoDatabase database;

        static RequestToMongoDB()
        {
            string? conStr = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ConnectionMongoDB");
            if (conStr == null)
            {
                throw new Exception("Строка подключения \"ConnectionMongoDB\" не найдена.");
            }
            mongoClient = new(conStr);
            using IAsyncCursor<string> databaseNames = mongoClient.ListDatabaseNames();
            if (!databaseNames.ToList().Contains("ATS"))
            {
                throw new Exception($"База данных \"ATS\" не существует на сервере \"{conStr}\"!");
            }
            database = mongoClient.GetDatabase("ATS");
        }

        private async static Task<IMongoCollection<T>> GetCollectionAsync<T>(string collectionName)
        {
            using IAsyncCursor<string> collectionsName = await database.ListCollectionNamesAsync();
            if (!collectionsName.ToList().Contains(collectionName))
            {
                throw new Exception($"Коллекция {collectionName} не найдена в базе данных!");
            }
            return database.GetCollection<T>(collectionName);
        }


        public async static Task<T> GetDocumentOneAsync<T>(string collectionName, Dictionary<string, object> filter)
        {
            IMongoCollection<T> collection = await GetCollectionAsync<T>(collectionName);
            using IAsyncCursor<T> documentCursor = await collection.FindAsync<T>(new BsonDocument(filter));
            T document = await documentCursor.FirstOrDefaultAsync();
            return document;
        }

        public async static Task<List<T>> GetDocumentManyAsync<T>(string collectionName, Dictionary<string, object> filter)
        {
            IMongoCollection<T> collection = await GetCollectionAsync<T>(collectionName);
            using IAsyncCursor<T> documentCursor = await collection.FindAsync<T>(new BsonDocument(filter));
            List<T> document = await documentCursor.ToListAsync();
            return document;
        }

        public async static Task<ReplaceOneResult?> SaveDocumentOneAsync<T>(string collectionName, Dictionary<string, object> filter, T newDocument)
        {
            IMongoCollection<T> collection = await GetCollectionAsync<T>(collectionName);
            return await collection.ReplaceOneAsync(new BsonDocument(filter), newDocument, new ReplaceOptions { IsUpsert = true });
        }

        public async static Task<BulkWriteResult> SaveDocumentManyAsync<T>(string collectionName, Dictionary<BsonDocument, object> filterAndNewDocument)
        {
            IMongoCollection<T> collection = await GetCollectionAsync<T>(collectionName);
            ReplaceOneModel<T>[] replaces = new ReplaceOneModel<T>[filterAndNewDocument.Count];
            int counter = 0;
            foreach (KeyValuePair<BsonDocument, object> item in filterAndNewDocument)
            {
                replaces[counter] = new ReplaceOneModel<T>(item.Key, (T)item.Value) { IsUpsert = true };
                counter += 1;
            }
            return await collection.BulkWriteAsync(replaces);
        }
        public async static Task<DeleteResult> DeleteDocumentOneAsync<T>(string collectionName, Dictionary<string, object> filter)
        {
            IMongoCollection<T> collection = await GetCollectionAsync<T>(collectionName);
            return await collection.DeleteOneAsync(new BsonDocument(filter));
        }

        public async static Task<DeleteResult?> DeleteDocumentManyAsync<T>(string collectionName, Dictionary<string, object> filter)
        {
            IMongoCollection<T> collection = await GetCollectionAsync<T>(collectionName);
            if (filter.Count == 0)
            {
                return null;
            }
            return await collection.DeleteManyAsync(new BsonDocument(filter));
        }
    }
}
