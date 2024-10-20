using ATS.Models;
using System.Globalization;
using System.Windows.Data;

namespace ATS.Views.Converters
{
    public class BenefitsNameConverter : IValueConverter
    {
        private static readonly Dictionary<BenefitsTypes, string> BenefitsToStringMap = new()
        {
        { BenefitsTypes.None, "Нет" },
        { BenefitsTypes.InvalidFirstDegree, "Инвалид 1-ой степени" },
        { BenefitsTypes.InvalidSecondDegree, "Инвалид 2-ой степени" },
        { BenefitsTypes.InvalidThirdDegree, "Инвалид 3-ей степени" },
        { BenefitsTypes.LargeFamily, "Многодетная семья" },
        { BenefitsTypes.VeteranVOV, "Ветеран ВОВ" },
        { BenefitsTypes.Other, "Другое" }
    };

        private static readonly Dictionary<string, BenefitsTypes> StringToBenefitsMap = BenefitsToStringMap.ToDictionary(kv => kv.Value, kv => kv.Key);

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BenefitsTypes benefits)
            {
                return BenefitsToStringMap.TryGetValue(benefits, out var result) ? result : null;
            }

            if (value is BenefitsTypes[] arrayBenefits)
            {
                return arrayBenefits.Select(benefit => BenefitsToStringMap.TryGetValue(benefit, out var result) ? result : Enum.GetName(typeof(BenefitsTypes), BenefitsTypes.None)).ToArray();
            }

            return null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string benefits)
            {
                return StringToBenefitsMap.TryGetValue(benefits, out var result) ? result : null;
            }

            if (value is string[] arrayBenefits)
            {
                return arrayBenefits.Select(benefit => StringToBenefitsMap.TryGetValue(benefit, out var result) ? result : BenefitsTypes.None).ToArray();
            }

            return null;
        }
    }

}
