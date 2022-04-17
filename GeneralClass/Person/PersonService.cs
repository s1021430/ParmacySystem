using System;
using System.Linq;
using System.Text.RegularExpressions;
using GeneralClass.Customer;

namespace GeneralClass.Person
{
    public class PersonService
    {
        public static bool IdNumberValidation(string id)
        {
            if (id is null) return false;
            var regex = new Regex("^[A-Z]{1}[0-9]{9}$");
            if (!regex.IsMatch(id))
                return false;

            var seed = new int[10];
            var charMapping = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "X", "Y", "W", "Z", "I", "O" };
            var target = id.Substring(0, 1);
            for (var index = 0; index < charMapping.Length; index++)
            {
                if (charMapping[index] != target) continue;
                index += 10;
                seed[0] = index / 10;
                seed[1] = (index % 10) * 9;
                break;
            }
            for (var index = 2; index < 10; index++)
                seed[index] = Convert.ToInt32(id.Substring(index - 1, 1)) * (10 - index);

            return (10 - seed.Sum() % 10) % 10 == Convert.ToInt32(id.Substring(9, 1));
        }

        /// <summary>
        /// 檢核中華民國外僑及大陸人士在台居留證(舊式+新式)
        /// </summary>
        public static bool ResidentIDValidation(string idNumber)
        {
            if (idNumber is null) return false;
            idNumber = idNumber.ToUpper();
            var regex = new Regex(@"^([A-Z])(A|B|C|D|8|9)(\d{8})$");
            var match = regex.Match(idNumber);
            if (!match.Success)
            {
                return false;
            }
            const string oldResidentCharacter = "ABCD";
            return oldResidentCharacter.IndexOf(match.Groups[2].Value, StringComparison.Ordinal) >= 0 ?
                CheckOldResidentID(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value) :
                CheckNewResidentID(match.Groups[1].Value, match.Groups[2].Value + match.Groups[3].Value);
        }


        /// <summary>
        /// 舊式檢核
        /// </summary>
        /// <param name="firstLetter">第1碼英文字母(區域碼)</param>
        /// <param name="secondLetter">第2碼英文字母(性別碼)</param>
        /// <param name="num">第3~9流水號 + 第10碼檢查碼</param>
        /// <returns></returns>
        private static bool CheckOldResidentID(string firstLetter, string secondLetter, string num)
        {
            /*
            建立字母對應表(A~Z)
            A=10 B=11 C=12 D=13 E=14 F=15 G=16 H=17 J=18 K=19 L=20 M=21 N=22
            P=23 Q=24 R=25 S=26 T=27 U=28 V=29 X=30 Y=31 W=32  Z=33 I=34 O=35
            */
            var alphabet = "ABCDEFGHJKLMNPQRSTUVXYWZIO";
            var transferIdNo =
                $"{alphabet.IndexOf(firstLetter, StringComparison.Ordinal) + 10}" +
                $"{(alphabet.IndexOf(secondLetter, StringComparison.Ordinal) + 10) % 10}" +
                $"{num}";

            var idNoArray = transferIdNo.ToCharArray()
                .Select(c => Convert.ToInt32(c.ToString())).ToArray();

            var sum = idNoArray[0];
            var weight = new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 1 };
            sum += weight.Select((t, i) => t * idNoArray[i + 1]).Sum();
            return (sum % 10 == 0);
        }


        /// <summary>
        /// 新式檢核
        /// </summary>
        /// <param name="firstLetter">第1碼英文字母(區域碼)</param>
        /// <param name="num">第2碼(性別碼) + 第3~9流水號 + 第10碼檢查碼</param>
        /// <returns></returns>
        private static bool CheckNewResidentID(string firstLetter, string num)
        {
            /*
            建立字母對應表(A~Z)
            A=10 B=11 C=12 D=13 E=14 F=15 G=16 H=17 J=18 K=19 L=20 M=21 N=22
            P=23 Q=24 R=25 S=26 T=27 U=28 V=29 X=30 Y=31 W=32  Z=33 I=34 O=35
            */
            const string alphabet = "ABCDEFGHJKLMNPQRSTUVXYWZIO";
            var transferIdNo = $"{alphabet.IndexOf(firstLetter, StringComparison.Ordinal) + 10}" +
                               $"{num}";

            var idNoArray = transferIdNo.ToCharArray()
                .Select(c => Convert.ToInt32(c.ToString())).ToArray();

            var sum = idNoArray[0];
            var weight = new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 1 };
            sum += weight.Select((t, i) => (t * idNoArray[i + 1]) % 10).Sum();
            return (sum % 10 == 0);
        }

        public static ServiceResult GetSearchPattern(CustomerSearchCondition condition,Customer.Customer searchObject)
        {
            var searchPattern = condition switch
            {
                CustomerSearchCondition.IDNumber => new CustomerSearchPattern(condition, searchObject.IDNumber),
                CustomerSearchCondition.Name => new CustomerSearchPattern(condition, searchObject.Name),
                CustomerSearchCondition.Birthday => new CustomerSearchPattern(condition, searchObject.Birthday),
                CustomerSearchCondition.FirstPhoneNumber => new CustomerSearchPattern(condition, searchObject.FirstPhoneNumber),
                CustomerSearchCondition.SecondPhoneNumber => new CustomerSearchPattern(condition, searchObject.SecondPhoneNumber),
                _ => null
            };
            return searchPattern is null ? new ServiceResult {Success = false, ErrorMessage = "查詢條件異常"} : 
                new ServiceResult { Success = true, Result = searchPattern};
        }

        public static bool NameValidation(string name)
        {
            return !string.IsNullOrEmpty(name);
        }
    }
}
