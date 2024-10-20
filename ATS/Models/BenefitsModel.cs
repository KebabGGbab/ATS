namespace ATS.Models
{
    public class BenefitsModel : BaseMVVM
    {
        private byte discount = 0;
        private BenefitsTypes? name = null;

        public BenefitsTypes? Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                UpdateDiscount();
                OnPropertyChanged();
            }
        }

        public byte Discount
        {
            get
            {
                return discount;
            }
            set
            {
                if (value < 0)
                {
                    discount = 0;
                }
                else if (value > 100)
                {
                    discount = 100;
                }
                else
                {
                    discount = value;
                }
                OnPropertyChanged();
            }
        }

        private void UpdateDiscount()
        {
            switch (name)
            {
                case BenefitsTypes.InvalidFirstDegree:
                case BenefitsTypes.VeteranVOV:
                    {
                        Discount = 20;
                        break;
                    }
                case BenefitsTypes.InvalidSecondDegree:
                    {
                        Discount = 15;
                        break;
                    }
                case BenefitsTypes.InvalidThirdDegree:
                case BenefitsTypes.LargeFamily:
                    {
                        Discount = 10;
                        break;
                    }
                case BenefitsTypes.Other:
                    {
                        Discount = 5;
                        break;
                    }
                default:
                    Discount = 0;
                    break;
            }
        }
    }
}
