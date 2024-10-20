using ATS.Models;
using ATS.ViewModels.Events;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ATS.ViewModels
{
    public class BenefitsViewModel : BaseViewModel
    {
        private ObservableCollection<BenefitsModel>? benefits = null;
        public ObservableCollection<BenefitsModel>? Benefits
        {
            get
            {
                return benefits;
            }
            private set
            {
                benefits = value;
                if (benefits != null)
                {
                    if (benefits.Count == 0)
                    {
                        benefits.Add(new BenefitsModel());
                    }
                    foreach (BenefitsModel benefitsModel in benefits)
                    {
                        UpdateEventOnCollectionItems(benefitsModel);
                    }
                    benefits.CollectionChanged += Benefits_CollectionChanged;
                }
                OnPropertyChanged();
            }
        }

        public BenefitsViewModel(AbonentViewModel abonentViewModel)
        {
            abonentViewModel.AbonentChanged += AbonentViewModel_AbonentChanged;
        }

        public void ChangeIsModifier()
        {
            IsDataModified = false;
        }

        private void AbonentViewModel_AbonentChanged(object sender, AbonentChangedEventArgs e)
        {
            Benefits = e.NewValue?.Benefits;
        }

        private void BenefitsModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (Benefits != null)
            {
                if (e.PropertyName == nameof(BenefitsModel.Name) && sender != null)
                {
                    BenefitsModel benefitsModel = (BenefitsModel)sender;
                    if (benefitsModel.Name == BenefitsTypes.None)
                    {
                        Benefits.Remove(benefitsModel);
                    }
                }
                if (Benefits.Count == 0 || Benefits.Last().Name != null)
                {
                    Benefits.Add(new BenefitsModel());
                }
            }
        }

        private void Benefits_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (object item in e.NewItems)
                {
                    UpdateEventOnCollectionItems((BenefitsModel)item);
                }
            }
        }

        private void UpdateEventOnCollectionItems(BenefitsModel benefitsModel)
        {
            benefitsModel.PropertyChanged -= BenefitsModel_PropertyChanged;
            benefitsModel.PropertyChanged += BenefitsModel_PropertyChanged;
            benefitsModel.PropertyChanged -= Data_PropertyChanged;
            benefitsModel.PropertyChanged += Data_PropertyChanged;
        }
    }
}
