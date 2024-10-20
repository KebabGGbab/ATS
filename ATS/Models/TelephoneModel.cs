using ATS.Tools;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ATS.Models
{
    public class TelephoneModel : BaseMVVM, IDataErrorInfo
    {
        [BsonId]
        public ObjectId Id { get; private set; }

        private string number = string.Empty;
        public string Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
                OnPropertyChanged();
            }
        }

        [BsonElement("Abonent")]
        public ObjectId AbonentId { get; private set; }

        private string category = string.Empty;
        public string Category
        {
            get
            {
                return category;
            }
            set
            {
                category = value;
                OnPropertyChanged();
            }
        }

        private decimal fee = 0;
        public decimal Fee
        {
            get
            {
                return fee;
            }
            set
            {
                fee = value;
                OnPropertyChanged();
            }
        }

        [BsonIgnore]
        public AbonentModel? Owner { get; private set; } = null;

        [BsonIgnore]
        public ObservableCollection<CallModel>? Calls { get; private set; } = [];

        [BsonIgnore]
        private const string COLLECTIONNAME = "Telephones";

        [BsonIgnore]
        public string Error => throw new NotImplementedException();

        public string this[string propertyName]
        {
            get
            {
                string error = string.Empty;
                switch (propertyName)
                {
                    case nameof(Number):
                        {
                            if (!RegexTools.StringIsPhoneNumber(number))
                            {
                                error = "Неверный формат телефона.\nПример правильного формата:\n\"+7 (900) 000-00-00\"";
                            }
                            break;
                        }
                }
                return error;
            }
        }

        public TelephoneModel()
        {
            Id = ObjectId.GenerateNewId();
        }

        public TelephoneModel(AbonentModel abonent) : this()
        {
            AbonentId = abonent.Id;
            Owner = abonent;
        }

        public async Task SearchCallsAsync()
        {
            Calls = new ObservableCollection<CallModel>(await CallModel.GetCallsAsync(this));
        }

        public async static Task<List<TelephoneModel>> GetTelephonesAsync(AbonentModel abonent)
        {
            Dictionary<string, object> filter = new() { { "Abonent", abonent.Id } };
            List<TelephoneModel> telephoneModels = await RequestToMongoDB.GetDocumentManyAsync<TelephoneModel>(COLLECTIONNAME, filter);
            foreach (TelephoneModel telephoneModel in telephoneModels)
            {
                telephoneModel.Owner = abonent;
            }
            return telephoneModels;
        }

        public async static Task<TelephoneModel> GetTelephonesAsync(string telephoneNumber)
        {
            Dictionary<string, object> filter = new() { { "Number",  telephoneNumber } };
            return await RequestToMongoDB.GetDocumentOneAsync<TelephoneModel>(COLLECTIONNAME, filter);
        }

        public async static Task<ReplaceOneResult?> SaveTelephoneOneAsync(TelephoneModel telephone)
        {
            return await RequestToMongoDB.SaveDocumentOneAsync(COLLECTIONNAME, new Dictionary<string, object> { { "_id", telephone.Id } }, telephone);
        }

        public async static Task<BulkWriteResult> SaveTelephoneManyAsync(ICollection<TelephoneModel> telephones)
        {
            Dictionary<BsonDocument, object> filterAndNewDocument = [];
            foreach (TelephoneModel telephone in telephones)
            {
                filterAndNewDocument.Add(new BsonDocument("_id", telephone.Id), telephone);
            }
            return await RequestToMongoDB.SaveDocumentManyAsync<TelephoneModel>(COLLECTIONNAME, filterAndNewDocument);
        }

        public async static Task<DeleteResult> DeleteTelephoneOneAsync(TelephoneModel telephone)
        {
            return await RequestToMongoDB.DeleteDocumentOneAsync<TelephoneModel>(COLLECTIONNAME, new Dictionary<string, object> { { "_id", telephone.Id } });
        }

        public async static Task<DeleteResult?> DeleteTelephoneManyAsync(ICollection<TelephoneModel> telephones)
        {
            Dictionary<string, object> filter = [];
            foreach (TelephoneModel telephone in telephones)
            {
                filter.Add("_id", telephone.Id);
            }
            return await RequestToMongoDB.DeleteDocumentManyAsync<TelephoneModel>(COLLECTIONNAME, filter);
        }
    }
}
