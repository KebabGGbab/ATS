using ATS.Models;
using ATS.ViewModels.Events;
using MongoDB.Driver;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ATS.ViewModels
{
    public class TelephoneViewModel : BaseViewModel
    {
        private ObservableCollection<TelephoneModel>? telephones = null;
        public ObservableCollection<TelephoneModel>? Telephones
        {
            get
            {
                return telephones;
            }
            private set
            {
                OnTelephoneCollectionChanged(Telephones, value);
                telephones = value;
                OnPropertyChanged(nameof(Telephones));
                if (telephones != null)
                {
                    telephones.CollectionChanged += OnTelephoneCollectionChanged;
                    foreach (TelephoneModel telephone in telephones)
                    {
                        telephone.PropertyChanged += Data_PropertyChanged;
                    }
                }
                IsDataModified = false;
            }
        }

        private TelephoneModel? selectedTelephone = null;
        public TelephoneModel? SelectedTelephone
        {
            get
            {
                return selectedTelephone;
            }
            set
            {
                OnSelectedTelephoneChanged(selectedTelephone, value);
                selectedTelephone = value;
                OnPropertyChanged(nameof(SelectedTelephone));
            }
        }

        public CallViewModel CallViewModel { get; private set; }

        private RelayCommand? addTelephoneCommand = null;
        public RelayCommand AddTelephoneCommand
        {
            get
            {
                return addTelephoneCommand ??= new RelayCommand(ExecuteAddTelephoneCommand, CanExecuteAddTelephoneCommand);
            }
        }

        private RelayCommand? deleteTelephoneCommand = null;
        public RelayCommand DeleteTelephoneCommand
        {
            get
            {
                return deleteTelephoneCommand ??= new RelayCommand(ExecuteDeleteTelephoneCommand, CanExecuteDeleteTelephoneCommand);
            }
        }

        public TelephoneViewModel(AbonentViewModel abonentViewModel)
        {
            CallViewModel = new CallViewModel(this);
            abonentViewModel.AbonentChanged += AbonentViewModel_AbonentChanged;
        }

        public async void SaveTelephones()
        {
            if (telephones != null)
            {
                await TelephoneModel.SaveTelephoneManyAsync(telephones);
                IsDataModified = false;
            }
        }

        private async void AbonentViewModel_AbonentChanged(object sender, AbonentChangedEventArgs e)
        {
            AbonentModel? abonent = e.NewValue;
            if (abonent != null)
            {
                ICollection<TelephoneModel>? telephones = abonent.Telephones;
                if (telephones != null)
                {
                    foreach (TelephoneModel telephone in telephones)
                    {
                        await telephone.SearchCallsAsync();
                    }
                }
                Telephones = new ObservableCollection<TelephoneModel>(abonent.Telephones ?? []);
                SelectedTelephone = Telephones?.FirstOrDefault();
            }
            else
            {
                Telephones = null;
            }
        }

        private void OnTelephoneCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
            {
                IsDataModified = true;
            }
        }

        private void ExecuteAddTelephoneCommand(object? parameter)
        {
            if (parameter == null)
            {
                return;
            }
            TelephoneModel telephone = new((AbonentModel)parameter);
            Telephones?.Add(telephone);
            SelectedTelephone = telephone;
        }

        private bool CanExecuteAddTelephoneCommand(object? parameter)
        {
            return Telephones != null;
        }

        private async void ExecuteDeleteTelephoneCommand(object? parameter)
        {
            if (Telephones != null && selectedTelephone != null)
            {
                ObservableCollection<CallModel>? calls = selectedTelephone.Calls;
                if (calls != null)
                {
                    await CallModel.DeleteCallManyAsync(calls);
                }
                await TelephoneModel.DeleteTelephoneOneAsync(selectedTelephone);
                Telephones.Remove(selectedTelephone);
                SelectedTelephone = Telephones.FirstOrDefault();
            }
        }

        private bool CanExecuteDeleteTelephoneCommand(object? parameter)
        {
            return telephones != null && selectedTelephone != null;
        }

        public event SelectedTelephoneChangedEventHandler? SelectedTelephoneChanged;
        private void OnSelectedTelephoneChanged(TelephoneModel? oldValue, TelephoneModel? newValue)
        {
            SelectedTelephoneChanged?.Invoke(this, new SelectedTelephoneChangedEventArgs(oldValue, newValue));
        }

        public event TelephoneCollectionChangedEventHandler? TelephoneCollectionChanged;
        private void OnTelephoneCollectionChanged(ObservableCollection<TelephoneModel>? oldValue, ObservableCollection<TelephoneModel>? newValue)
        {
            TelephoneCollectionChanged?.Invoke(this, new TelephoneCollectionChangedEventArgs(oldValue, newValue));
        }
    }
}
