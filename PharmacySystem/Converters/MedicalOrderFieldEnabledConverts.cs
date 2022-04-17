using System;
using System.Globalization;
using System.Windows.Data;
using GeneralClass.Prescription.MedicalOrders;
using PharmacySystem.Class;

namespace PharmacySystem.Converters
{
    public class MedicalOrderFieldEnabledConverts : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is MedicalOrderViewModel order)) return false;
            switch (order.Type)
            {
                case MedicalOrderType.Medicine:
                case MedicalOrderType.NoCharged:
                case MedicalOrderType.SpecialMaterial:
                case MedicalOrderType.SpecialMaterialNoSubsidy:
                case MedicalOrderType.SpecialMaterialNotInPaymentRule:
                    return true;
                case MedicalOrderType.Virtual:
                case MedicalOrderType.MedicalService:
                    return false;
                default:
                    return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
