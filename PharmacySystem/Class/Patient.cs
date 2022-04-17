using System;
using GalaSoft.MvvmLight;
using GeneralClass.Customer;
using GeneralClass.Customer.EntityIndex;

namespace PharmacySystem.Class
{
    public class PatientDataViewModel : ObservableObject
    {
        public static explicit operator PatientDataViewModel(GeneralClass.Customer.Customer customer)
        {
            return new PatientDataViewModel(customer);
        }

        public static implicit operator GeneralClass.Customer.Customer(PatientDataViewModel patient)
        {
            var gender = patient.gender.Equals(Gender.男);
            return new GeneralClass.Customer.Customer(new CustomerID(patient.ID), patient.Name, gender, patient.IDNumber, patient.Birthday, patient.FirstPhoneNumber, patient.SecondPhoneNumber, patient.Address, string.Empty, patient.Line, patient.Note, patient.email, true);
        }

        public PatientDataViewModel()
        {

        }

        private PatientDataViewModel(GeneralClass.Customer.Customer c)
        {
            ID = c.ID.ID;
            IDNumber = c.IDNumber;
            Name = c.Name;
            Birthday = c.Birthday;
            FirstPhoneNumber = c.FirstPhoneNumber;
            SecondPhoneNumber = c.SecondPhoneNumber;
            Address = c.Address;
            Note = c.Note;
            Gender = c.Gender ? Gender.男 : Gender.女;
            Line = c.Line;
        }

        public int ID;
        private string idNumber = string.Empty;
        public string IDNumber 
        { 
            get => idNumber;
            set
            {
                Set(() => IDNumber, ref idNumber, value);
            }
        }
        private string name = string.Empty;
        public string Name
        {
            get => name;
            set
            {
                Set(() => Name, ref name, value);
            }
        }
        private DateTime? birthday;
        public DateTime? Birthday
        {
            get => birthday;
            set
            {
                Set(() => Birthday, ref birthday, value);
            }
        }
        private string firstPhoneNumber = string.Empty;
        public string FirstPhoneNumber
        {
            get => firstPhoneNumber;
            set
            {
                Set(() => FirstPhoneNumber, ref firstPhoneNumber, value);
            }
        }
        private string secondPhoneNumber = string.Empty;
        public string SecondPhoneNumber
        {
            get => secondPhoneNumber;
            set
            {
                Set(() => SecondPhoneNumber, ref secondPhoneNumber, value);
            }
        }
        private string address = string.Empty;
        public string Address
        {
            get => address;
            set
            {
                Set(() => Address, ref address, value);
            }
        }
        private string note = string.Empty;
        public string Note
        {
            get => note;
            set
            {
                Set(() => Note, ref note, value);
            }
        }

        private string email = string.Empty;
        public string Email
        {
            get => email;
            set
            {
                Set(() => Email, ref email, value);
            }
        }

        private string line = string.Empty;
        public string Line
        {
            get => line;
            set
            {
                Set(() => Line, ref line, value);
            }
        }

        private Gender gender = Gender.Unset;
        public Gender Gender
        {
            get => gender;
            set
            {
                Set(() => Gender, ref gender, value);
            }
        }
    }
}
