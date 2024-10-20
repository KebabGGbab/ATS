using ATS.Tools;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ATS.Models
{
    public class AbonentModel : BaseMVVM, IDataErrorInfo
    {
        [BsonId]
        public ObjectId Id { get; private set; }

        private string surname = string.Empty;
        public string Surname
        {
            get
            {
                return surname;
            }
            set
            {
                surname = value;
                OnPropertyChanged();
            }
        }

        private string name = string.Empty;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private string secondName = string.Empty;
        public string SecondName
        {
            get
            {
                return secondName;
            }
            set
            {
                secondName = value;
                OnPropertyChanged();
            }
        }

        private string address = string.Empty;
        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<BenefitsModel>? benefits = [];
        public ObservableCollection<BenefitsModel>? Benefits
        {
            get
            {
                return benefits;
            }
            private set
            {
                benefits = value;
                OnPropertyChanged();
            }
        }

        [BsonIgnore]
        public ICollection<TelephoneModel>? Telephones { get; private set; } = null;

        [BsonIgnore]
        private const string COLLECTIONNAME = "Abonents";

        [BsonIgnore]
        public string Error => throw new NotImplementedException();

        public string this[string propertyName]
        {
            get
            {
                string error = string.Empty;
                switch (propertyName)
                {
                    case nameof(Surname):
                        {
                            if (string.IsNullOrWhiteSpace(surname))
                            {
                                error = "Поле, содержащее фамилию абонента не может быть пустым.";
                            }
                            else if (RegexTools.StringContainsDigit(surname))
                            {
                                error = "Фамилия не может содержать цифр.";
                            }
                            break;
                        }
                    case nameof(Name):
                        {
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                error = "Поле, содержащее имя абонента не может быть пустым.";
                            }
                            else if (RegexTools.StringContainsDigit(name))
                            {
                                error = "Имя не может содержать цифр.";
                            }
                            break;
                        }
                    case nameof(SecondName):
                        {
                            if (RegexTools.StringContainsDigit(secondName))
                            {
                                error = "Отчество не может содержать цифр.";
                            }
                            break;
                        }
                }
                return error;
            }
        }

        public AbonentModel()
        {
            Id = ObjectId.GenerateNewId();
            Telephones = [];
        }

        public async Task SearchTelephonesAsync()
        {
            Telephones = await TelephoneModel.GetTelephonesAsync(this);
        }

        public async static Task<AbonentModel?> GetAbonentAsync(string surname, string name, string secondName)
        {
            Dictionary<string, object> filter = new() { { "Surname", surname }, { "Name", name }, { "SecondName", secondName } };
            return await RequestToMongoDB.GetDocumentOneAsync<AbonentModel>(COLLECTIONNAME, filter);
        }

        public async static Task<AbonentModel> GetAbonentAsync(TelephoneModel telephone)
        {
            Dictionary<string, object> filter = new() { { "_id", telephone.AbonentId } };
            return await RequestToMongoDB.GetDocumentOneAsync<AbonentModel>(COLLECTIONNAME, filter);
        }

        public async static Task<ReplaceOneResult?> SaveAbonentAsync(AbonentModel abonent)
        {
            return await RequestToMongoDB.SaveDocumentOneAsync(COLLECTIONNAME, new Dictionary<string, object> { { "_id", abonent.Id } }, abonent);
        }

        public async static Task<DeleteResult> DeleteAbonentOneAsync(AbonentModel abonent)
        {
            return await RequestToMongoDB.DeleteDocumentOneAsync<AbonentModel>(COLLECTIONNAME, new Dictionary<string, object> { { "_id", abonent.Id } });
        }
    }
}
