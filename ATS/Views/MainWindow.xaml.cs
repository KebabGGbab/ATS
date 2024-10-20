using ATS.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace ATS.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AbonentViewModel abonentViewModel = new();
            DataContext = abonentViewModel;
            TelephoneGB.DataContext = abonentViewModel.TelephoneViewModel;
            CallGB.DataContext = abonentViewModel.TelephoneViewModel.CallViewModel;
            BenefitsDG.DataContext = abonentViewModel.BenefitsViewModel;
            ReportPanel.DataContext = new ReportViewModel();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            AbonentViewModel abonentViewModel = (AbonentViewModel)DataContext;
            if (abonentViewModel.CheckDataModified())
            {
                bool? isSaved = AbonentViewModel.AskUserNeedSaveData("Программа содержит несохраненные данные. Сохранить перед закрытием?");
                if (isSaved == true && abonentViewModel.SaveAbonentCommand.CanExecute(null))
                {
                    abonentViewModel.SaveAbonentCommand.Execute(null);
                }
                else if (isSaved == null)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}