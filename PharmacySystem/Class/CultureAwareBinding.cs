using System.Globalization;
using System.Windows.Data;

namespace PharmacySystem.Class
{
    public class CultureAwareBinding : Binding
    {
        public CultureAwareBinding()
        {
            var twCulture = new CultureInfo("zh-TW", true);
            twCulture.DateTimeFormat.Calendar = twCulture.OptionalCalendars[1];//TaiwanCalendar
            twCulture.DateTimeFormat.ShortDatePattern = "yyy/MM/dd";
            ConverterCulture = twCulture;
        }
    }
}
