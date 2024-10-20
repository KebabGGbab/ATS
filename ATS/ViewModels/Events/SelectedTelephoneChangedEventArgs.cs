using ATS.Models;

namespace ATS.ViewModels.Events
{
    public class SelectedTelephoneChangedEventArgs(TelephoneModel? oldValue, TelephoneModel? newValue) : EventArgs
    {
        public TelephoneModel? OldValue { get; } = oldValue;
        public TelephoneModel? NewValue { get; } = newValue;
    }
}
