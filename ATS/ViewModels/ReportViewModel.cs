using ATS.Models;
using ATS.Tools;
using System.IO;
using System.Windows;

namespace ATS.ViewModels
{
    public class ReportViewModel : BaseMVVM
    {
        public AbonentModel? Abonent { get; private set; } = null;
        public TelephoneModel? Telephone { get; private set; } = null;
        public List<CallModel>? Calls { get; private set; } = null;

        private string phoneNumber = string.Empty;
        public string PhoneNumber 
        {
            get
            {
                return phoneNumber;
            }
            set
            {
                phoneNumber = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand? generateReport;
        public RelayCommand GenerateReport
        {
            get
            {
                return generateReport ??= new RelayCommand(ExecuteGenerateReport, CanExecuteGenerateReport);
            }
        }

        private async void ExecuteGenerateReport(object? parameter)
        {
            Telephone = await TelephoneModel.GetTelephonesAsync(phoneNumber);
            Abonent = await AbonentModel.GetAbonentAsync(Telephone);
            Calls = (await CallModel.GetCallsAsync(Telephone)).Where(call => call.Date >= DateTime.Now.AddMonths(-1)).ToList();
            Guid guid = Guid.NewGuid();
            string benefits = string.Empty;
            if (Abonent.Benefits != null)
            {
                foreach (BenefitsModel benefit in Abonent.Benefits)
                {
                    if (benefit.Name != null)
                    {
                        benefits += $"{benefit.Name} - {benefit.Discount}%,";
                    }
                }
                if (benefits.Length > 0)
                {
                    benefits = benefits.Remove(benefits.Length - 1);
                }
            }
            List<CallModel> localCall = Calls.Where(calls => calls.PaymentCategory == "Местный").ToList();
            int durationLocalCall = localCall.Select(call => call.Duration).Sum();
            decimal feeLocalCall = Math.Round((decimal)durationLocalCall / 60 * 2, 2);

            List<CallModel> longDistanceCall = Calls.Where(calls => calls.PaymentCategory == "Междугородний").ToList();
            int durationlongDistanceCall = longDistanceCall.Select(call => call.Duration).Sum();
            decimal feeLongDistanceCall = Math.Round((decimal)durationlongDistanceCall / 60 * 15, 2);

            List<CallModel> internationalCall = Calls.Where(calls => calls.PaymentCategory == "Международний").ToList();
            int durationInternationalCall = internationalCall.Select(call => call.Duration).Sum();
            decimal feeInternationalCall = Math.Round((decimal)durationInternationalCall / 60 * 150, 2);

            string path = Path.GetFullPath($"../../../Reports/{guid}.txt");
            using StreamWriter sw = new(File.Open(path, FileMode.CreateNew));
            await sw.WriteAsync($"Фамилия: {Abonent.Surname}\n" +
                $"Имя: {Abonent.Name}\n" +
                $"Отчество: {Abonent.SecondName}\n" +
                $"Адрес: {Abonent.Address}\n" +
                $"Льготы: {(benefits != string.Empty ? benefits : "Нет" )}\n" +
                $"Номер телефона: {Telephone.Number}\n" +
                $"Категория телефона: {Telephone.Category}\n" +
                $"Абоненсткая плата: {Telephone.Fee}\n" +
                $"Всего местных звонков: {localCall.Count}, их длительность составила: {durationLocalCall} секунд, а сумма оплаты: {feeLocalCall} рублей по тарифу 2 руб./мин\n" +
                $"Всего междугородних звонков: {longDistanceCall.Count}, их длительность составила: {durationlongDistanceCall} секунд, а сумма оплаты: {feeLongDistanceCall} рублей по тарифу 15 руб./мин\n" +
                $"Всего международных звонков: {internationalCall.Count}, их длительность составила: {durationInternationalCall} секунд, а сумма оплаты: {feeInternationalCall} рублей по тарифу 150 руб./мин\n" +
                $"Итоговая сумма к оплате, с учётом льгот: {Math.Round(Telephone.Fee + feeLocalCall + feeLongDistanceCall + feeInternationalCall - (Abonent?.Benefits?.Max(benefit => benefit.Discount)) ?? 0, 2)}");

            MessageBox.Show($"Отчёт записан в файл {path}", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanExecuteGenerateReport(object? parameter)
        {
            return RegexTools.StringIsPhoneNumber(phoneNumber);
        }
    }
}
