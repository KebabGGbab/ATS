using System.ComponentModel;

namespace ATS.ViewModels
{
    public class BaseViewModel : BaseMVVM
    {
        public bool IsDataModified { get; private protected set; } = false;
        private protected void Data_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            IsDataModified = true;
        }
    }
}
