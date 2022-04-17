using System;
using System.Text;

namespace HisApiBase
{
    public class ConvertData
    {
        //身分註記轉中文
        public static string NoteTheIdentityCode(string code)
        {
            switch (code)
            {
                case "1":
                    return "福民";
                case "2":
                    return "榮民";
                case "3":
                    return "一般";
                default:
                    return string.Empty;
            }
        }

        //byte轉字串
        public static string BytesToString(byte[] bytes, int start, int length)
        {
            var data = Encoding.GetEncoding(950).GetString(bytes, start, length);
            return data;
        }

        public static string SubStr(string aSrcStr, int aStartIndex, int aCnt)
        {
            var lEncoding = Encoding.GetEncoding("UTF8", new EncoderExceptionFallback(),
                new DecoderReplacementFallback(""));
            var lByte = lEncoding.GetBytes(aSrcStr);
            if (aCnt <= 0)
                return "";
            //例若長度10 
            //若a_StartIndex傳入9 -> ok, 10 ->不行 
            if (aStartIndex + 1 > lByte.Length)
                return "";

            if (aStartIndex + aCnt > lByte.Length)
                aCnt = lByte.Length - aStartIndex;

            return lEncoding.GetString(lByte, aStartIndex, aCnt);
        }

        public static byte Asc(string s)
        {
            return Encoding.ASCII.GetBytes(s.ToCharArray(), 0, 1)[0];
        }

        //字串轉btyes
        public static byte[] StringToBytes(string newString)
        {
            return Encoding.GetEncoding(950).GetBytes(newString);
        }

        //擷取位元資料
        public static byte[] ByteGetSubArray(byte[] input, int indexStart, int length)
        {
            var newArray = new byte[length];
            Array.Copy(input, indexStart, newArray, 0, length);
            return newArray;
        }

        public static DateTime TaiwanCalendarStringToDateTime(string date,TimeUnit unit = TimeUnit.Day)
        {
            var year = int.Parse(date.Substring(0, 3)) + 1911;
            var month = int.Parse(date.Substring(3, 2));
            var day = int.Parse(date.Substring(5, 2));
            if(unit.Equals(TimeUnit.Day))
                return new DateTime(year, month, day);
            var hour = int.Parse(date.Substring(7, 2));
            var min = int.Parse(date.Substring(9, 2));
            var sec = int.Parse(date.Substring(11, 2));
            return new DateTime(year, month, day, hour, min, sec);
        }

        public static string DateTimeToTaiwanCalendarString(DateTime dateTime,TimeUnit unit = TimeUnit.Day)
        {
            var year = (dateTime.Year - 1911).ToString().PadLeft(3, '0');;
            var month = (dateTime.Month).ToString().PadLeft(2, '0');
            var day = (dateTime.Day).ToString().PadLeft(2, '0');
            var hour = (dateTime.Hour).ToString().PadLeft(2, '0');
            var minute = (dateTime.Minute).ToString().PadLeft(2, '0');
            var sec = (dateTime.Second).ToString().PadLeft(2, '0');
            switch (unit)
            {
                case TimeUnit.Year:
                    return year;
                case TimeUnit.Month:
                    return $"{year}{month}";
                case TimeUnit.Hour:
                    return $"{year}{month}{day}{hour}";
                case TimeUnit.Minute:
                    return $"{year}{month}{day}{hour}{minute}";
                case TimeUnit.Sec:
                    return $"{year}{month}{day}{hour}{minute}{sec}";
                default:
                    return $"{year}{month}{day}";
            }
        }
    }

    public enum TimeUnit
    {
        Year,
        Month,
        Day,
        Hour,
        Minute,
        Sec
    }
}