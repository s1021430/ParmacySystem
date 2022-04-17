using System;
using GeneralClass.Customer.EntityIndex;

namespace GeneralClass.Customer
{
    public enum Gender
    {
        Unset,
        男,
        女
    }

    public enum CustomerSearchCondition
    {
        IDNumber,
        Name,
        Birthday,
        FirstPhoneNumber,
        SecondPhoneNumber
    }

    public class CustomerSearchPattern
    {
        public CustomerSearchCondition Condition { get; }
        public object Content { get; }

        public CustomerSearchPattern(CustomerSearchCondition condition, object content)
        {
            Condition = condition;
            Content = content;
        }
    }

    public class Customer
    {
        public Customer(CustomerID id, string name, bool gender, string idNumber, DateTime? birthday, string firstPhoneNumber, string secondPhoneNumber, string address, string eMail, string line, string note,string email, bool isEnable)
        {
            ID = id;
            IDNumber = idNumber;
            Name = name;
            Birthday = birthday;
            Gender = gender;
            FirstPhoneNumber = firstPhoneNumber;
            SecondPhoneNumber = secondPhoneNumber;
            Address = address;
            Note = note;
            Line = line;
            EMail = eMail;
            IsEnabled = isEnable;
        }
        public Customer(string idNum, string name, DateTime birthday, Gender gender)
        {
            ID = new CustomerID(-1);
            IDNumber = idNum;
            Name = name;
            Birthday = birthday;
            Gender = gender.Equals(GeneralClass.Customer.Gender.男);
        }

        public Customer(){}
        
        public CustomerID ID { get; set; }
        public string IDNumber { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
        public string FirstPhoneNumber { get; set; }
        public string SecondPhoneNumber { get; set; }
        public string Address { get; set; }
        public string Note { get; set; } 
        public string Line { get; set; }
        public bool Gender { get; set; }
        public string EMail { get; set; }
        public bool IsEnabled { get; set; }
    }
}
