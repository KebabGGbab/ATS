using ATS.Models;

namespace ATS.ViewModels.Events
{
    public class AbonentChangedEventArgs
    {
        public AbonentModel? OldValue { get; }
        public AbonentModel? NewValue { get; }

        public ATSEventMode Mode { get; }

        public AbonentChangedEventArgs(AbonentModel? oldValue, AbonentModel? newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
            if (newValue == null)
            {
                Mode = ATSEventMode.Delete;
            }
            else
            {
                if (string.IsNullOrEmpty(newValue.Name) && string.IsNullOrEmpty(newValue.Surname) && string.IsNullOrEmpty(newValue.SecondName))
                {
                    Mode = ATSEventMode.Add;
                }
                else
                {
                    Mode = ATSEventMode.Change;
                }
            }
        }
    }
}
