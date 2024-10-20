using ATS.Models;
using ATS.ViewModels.Events;
using System.Windows;

namespace ATS.ViewModels
{
    public class AbonentViewModel : BaseViewModel
    {
        private AbonentModel? abonent = null;
        public AbonentModel? Abonent
        {
            get
            {
                return abonent;
            }
            private set
            {
                OnAbonentChanged(abonent, value);
                abonent = value;
                OnPropertyChanged();
                if (abonent != null)
                {
                    abonent.PropertyChanged += Data_PropertyChanged;
                }
            }
        }

        public TelephoneViewModel TelephoneViewModel { get; private set; }

        public BenefitsViewModel BenefitsViewModel { get; private set; }

        private RelayCommand? getAbonentCommand = null;
        public RelayCommand GetAbonentCommand
        {
            get
            {
                return getAbonentCommand ??= new RelayCommand(ExecuteGetAbonentCommand, CanExecuteGetAbonentCommand);
            }
        }

        private RelayCommand? saveAbonentCommand = null;
        public RelayCommand SaveAbonentCommand
        {
            get
            {
                return saveAbonentCommand ??= new RelayCommand(ExecuteSaveAbonentCommand, CanExecuteSaveAbonentCommand);
            }
        }

        private RelayCommand? addAbonentCommand = null;
        public RelayCommand AddAbonentCommand
        {
            get
            {
                return addAbonentCommand ??= new RelayCommand(ExecuteAddAbonentCommand);
            }
        }

        private RelayCommand? deleteAbonentCommand = null;
        public RelayCommand DeleteAbonentCommand
        {
            get
            {
                return deleteAbonentCommand ??= new RelayCommand(ExecuteDeleteAbonentCommand, CanExecuteDeleteAbonentCommand);
            }
        }

        public AbonentViewModel()
        {
            TelephoneViewModel = new TelephoneViewModel(this);
            BenefitsViewModel = new BenefitsViewModel(this);
        }

        private async void ExecuteGetAbonentCommand(object? parameter)
        {
            if (parameter != null)
            {
                if (CheckDataModified())
                {
                    bool? isDataSaved = AskUserNeedSaveData("Программа содержит несохраненные данные. Сохранить перед загрузкой данных?");
                    if (isDataSaved == true && SaveAbonentCommand.CanExecute(null))
                    {
                        SaveAbonentCommand.Execute(null);
                    }
                    else if (isDataSaved == null)
                    {
                        return;
                    }
                }
                string[] FIO = ((string)parameter).Split(' ');
                AbonentModel? abonentModel = await AbonentModel.GetAbonentAsync(FIO[0], FIO[1], FIO[2]);
                if (abonentModel != null)
                {
                    await abonentModel.SearchTelephonesAsync();
                }
                Abonent = abonentModel;
                IsDataModified = false;
            }
        }

        private bool CanExecuteGetAbonentCommand(object? parameter)
        {
            if (parameter != null)
            {
                if (((string)parameter).Count(ch => ch == ' ') == 2)
                {
                    return true;
                }
            }
            return false;
        }

        private void ExecuteAddAbonentCommand(object? parameter)
        {
            if (CheckDataModified())
            {
                bool? isDataSaved = AskUserNeedSaveData("Программа содержит несохраненные данные. Сохранить перед созданием новых данных?");
                if (isDataSaved == true && SaveAbonentCommand.CanExecute(null))
                {
                    SaveAbonentCommand.Execute(null);
                }
                else if (isDataSaved == null)
                {
                    return;
                }
            }
            Abonent = new AbonentModel();
        }

        private async void ExecuteDeleteAbonentCommand(object? parameter)
        {
            if (abonent != null)
            {
                if (TelephoneViewModel.Telephones != null)
                {
                    List<CallModel> calls = [];
                    foreach (TelephoneModel telephone in TelephoneViewModel.Telephones)
                    {
                        if (telephone.Calls != null)
                        {
                            calls.AddRange(telephone.Calls);
                        }
                    }
                    await CallModel.DeleteCallManyAsync(calls);
                    await TelephoneModel.DeleteTelephoneManyAsync(TelephoneViewModel.Telephones);
                }
                await AbonentModel.DeleteAbonentOneAsync(abonent);
                Abonent = null;
            }
        }

        private bool CanExecuteDeleteAbonentCommand(object? parameter)
        {
            if (abonent != null)
            {
                return true;
            }
            return false;
        }

        private async void ExecuteSaveAbonentCommand(object? parameter)
        {
            if (abonent != null && (IsDataModified || BenefitsViewModel.IsDataModified))
            {
                await AbonentModel.SaveAbonentAsync(abonent);
                IsDataModified = false;
                BenefitsViewModel.ChangeIsModifier();
            }
            if (TelephoneViewModel.Telephones != null && TelephoneViewModel.IsDataModified)
            {
                TelephoneViewModel.SaveTelephones();
            }
            if (TelephoneViewModel.Telephones != null && TelephoneViewModel.CallViewModel.Calls != null && TelephoneViewModel.CallViewModel.IsDataModified)
            {
                List<CallModel> calls = [];
                foreach (TelephoneModel telephone in TelephoneViewModel.Telephones)
                {
                    if (telephone.Calls != null)
                    {
                        calls.AddRange(telephone.Calls);
                    }
                }
                TelephoneViewModel.CallViewModel.SaveCallsAsync(calls);
            }
        }

        private bool CanExecuteSaveAbonentCommand(object? parameter)
        {
            return CheckDataModified();
        }

        public bool CheckDataModified()
        {
            return IsDataModified || TelephoneViewModel.IsDataModified || TelephoneViewModel.CallViewModel.IsDataModified || BenefitsViewModel.IsDataModified;
        }

        public static bool? AskUserNeedSaveData(string message)
        {
            MessageBoxResult msResult = MessageBox.Show(message, "Требуется подтверждение", MessageBoxButton.YesNoCancel);
            if (msResult == MessageBoxResult.Yes)
            {
                return true;
            }
            else if (msResult == MessageBoxResult.Cancel)
            {
                return null;
            }
            return false;
        }

        public event AbonentChangedEventHandler? AbonentChanged;
        private void OnAbonentChanged(AbonentModel? oldValue, AbonentModel? newValue)
        {
            AbonentChanged?.Invoke(this, new AbonentChangedEventArgs(oldValue, newValue));
        }
    }
}
