using ATS.Models;
using ATS.ViewModels.Events;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ATS.ViewModels
{
    public class CallViewModel : BaseViewModel
    {
        private ObservableCollection<CallModel>? calls = null;
        public ObservableCollection<CallModel>? Calls
        {
            get
            {
                return calls;
            }
            private set
            {
                calls = value;
                OnPropertyChanged();
                if (Calls != null)
                {
                    Calls.CollectionChanged += Calls_CollectionChanged;
                    foreach (CallModel call in Calls)
                    {
                        call.PropertyChanged += Data_PropertyChanged;
                    }
                }
            }
        }

        private CallModel? selectedCall;
        public CallModel? SelectedCall
        {
            get
            {
                return selectedCall;
            }
            set
            {
                selectedCall = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand? addCallCommand;
        public RelayCommand AddCallCommand
        {
            get
            {
                return addCallCommand ??= new RelayCommand(ExecuteAddCallCommand, CanExecuteAddCallCommand);
            }
        }

        private RelayCommand? deleteCallCommand;
        public RelayCommand DeleteCallCommand
        {
            get
            {
                return deleteCallCommand ??= new RelayCommand(ExecuteDeleteCallCommand, CanExecuteDeleteCallCommand);
            }
        }

        public CallViewModel(TelephoneViewModel telephoneViewModel)
        {
            telephoneViewModel.TelephoneCollectionChanged += TelephoneViewModel_TelephoneCollectionChanged;
            telephoneViewModel.SelectedTelephoneChanged += TelephoneViewModel_SelectedTelephoneChanged;
        }

        public async void SaveCallsAsync(ICollection<CallModel> callModels)
        {
            await CallModel.SaveCallManyAsync(callModels);
            IsDataModified = false;
        }

        private void TelephoneViewModel_TelephoneCollectionChanged(object sender, TelephoneCollectionChangedEventArgs e)
        {
            ObservableCollection<TelephoneModel>? telephones = e.NewValue;
            if (telephones != null)
            {
                Calls = telephones?.FirstOrDefault()?.Calls;
                SelectedCall = Calls?.FirstOrDefault();
            }
            else
            {
                Calls = null;
            }
            IsDataModified = false;
        }

        private void TelephoneViewModel_SelectedTelephoneChanged(object sender, SelectedTelephoneChangedEventArgs e)
        {
            TelephoneModel? telephone = e.NewValue;
            if (telephone != null)
            {
                Calls = telephone.Calls;
                SelectedCall = Calls?.FirstOrDefault();
            }
            else
            {
                Calls = null;
            }
        }

        private void Calls_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
            {
                IsDataModified = true;
            }
        }
        private void ExecuteAddCallCommand(object? parameter)
        {
            if (parameter == null)
            {
                return;
            }
            CallModel call = new((TelephoneModel)parameter);
            Calls?.Add(call);
            SelectedCall = call;
        }

        private bool CanExecuteAddCallCommand(object? parameter)
        {
            return calls != null;
        }

        private async void ExecuteDeleteCallCommand(object? parameter)
        {
            if (Calls != null && selectedCall != null)
            {
                await CallModel.DeleteCallOneAsync(selectedCall);
                Calls.Remove(selectedCall);
                SelectedCall = calls?.FirstOrDefault();
            }
        }

        private bool CanExecuteDeleteCallCommand(object? parameter)
        {
            return calls != null && selectedCall != null;
        }
    }
}
