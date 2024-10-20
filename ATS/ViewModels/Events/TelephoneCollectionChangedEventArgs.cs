using ATS.Models;
using System.Collections.ObjectModel;

namespace ATS.ViewModels.Events
{
    public class TelephoneCollectionChangedEventArgs(ObservableCollection<TelephoneModel>? oldValue, ObservableCollection<TelephoneModel>? newValue)
    {
        public ObservableCollection<TelephoneModel>? OldValue { get; } = oldValue;
        public ObservableCollection<TelephoneModel>? NewValue { get; } = newValue;
    }
}
