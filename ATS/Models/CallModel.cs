using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ATS.Models
{
    public class CallModel : BaseMVVM
    {
        [BsonId]
        public ObjectId Id { get; private set; }

        [BsonElement("Telephone")]
        public ObjectId? TelephoneId { get; private set; }

        private int duration = 0;
        public int Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
                OnPropertyChanged();
            }
        }

        private string paymentCategory = string.Empty;
        public string PaymentCategory
        {
            get
            {
                return paymentCategory;
            }
            set
            {
                paymentCategory = value;
                OnPropertyChanged();
            }
        }

        private DateTime date = DateTime.UtcNow;
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
                OnPropertyChanged();
            }
        }

        [BsonIgnore]
        public TelephoneModel? Owner { get; private set; } = null;

        [BsonIgnore]
        private const string COLLECTIONNAME = "Calls";

        public CallModel()
        {
            Id = ObjectId.GenerateNewId();
        }

        public CallModel(TelephoneModel telephone) : this()
        {
            TelephoneId = telephone.Id;
            Owner = telephone;
        }

        public async static Task<List<CallModel>> GetCallsAsync(TelephoneModel telephone)
        {
            Dictionary<string, object> filter = new() { { "Telephone", telephone.Id } };
            List<CallModel> callModels = await RequestToMongoDB.GetDocumentManyAsync<CallModel>(COLLECTIONNAME, filter);
            foreach (CallModel callModel in callModels)
            {
                callModel.Owner = telephone;
            }
            return callModels;
        }

        public async static Task<ReplaceOneResult?> SaveCallOneAsync(CallModel call)
        {
            return await RequestToMongoDB.SaveDocumentOneAsync(COLLECTIONNAME, new Dictionary<string, object> { { "_id", call.Id } }, call);
        }

        public async static Task<BulkWriteResult> SaveCallManyAsync(ICollection<CallModel> calls)
        {
            Dictionary<BsonDocument, object> filterAndNewDocument = [];
            foreach (CallModel call in calls)
            {
                filterAndNewDocument.Add(new BsonDocument("_id", call.Id), call);
            }
            return await RequestToMongoDB.SaveDocumentManyAsync<CallModel>(COLLECTIONNAME, filterAndNewDocument);
        }

        public async static Task<DeleteResult> DeleteCallOneAsync(CallModel call)
        {
            return await RequestToMongoDB.DeleteDocumentOneAsync<CallModel>(COLLECTIONNAME, new Dictionary<string, object> { { "_id", call.Id } });
        }

        public async static Task<DeleteResult?> DeleteCallManyAsync(ICollection<CallModel> calls)
        {
            Dictionary<string, object> filter = [];
            foreach (CallModel call in calls)
            {
                filter.Add("_id", call.Id);
            }
            return await RequestToMongoDB.DeleteDocumentManyAsync<CallModel>(COLLECTIONNAME, filter);
        }
    }
}
