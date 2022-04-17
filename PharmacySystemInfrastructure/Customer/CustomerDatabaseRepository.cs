using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using GeneralClass;
using GeneralClass.Customer;
using GeneralClass.Customer.EntityIndex;

namespace PharmacySystemInfrastructure.Customer
{
    public class CustomerDatabaseRepository : ICustomerRepository
    {
        public const string customerColumnSQL =
            "ID, Name, Gender, IDNumber, Birthday, FirstPhoneNumber, SecondPhoneNumber, Address, EMail, Line, Note, IsEnable";

        public List<GeneralClass.Customer.Customer> GetCustomerByIDNumber(string idNumber)
        {
            var sql = $"Select {customerColumnSQL} from Customer.Master where IDNumber like '%{idNumber}%'";
            List<GeneralClass.Customer.Customer> result;
            using (var conn = new SqlConnection(DBInvoker.ConnectionString))
            {
                conn.Open();

                result = conn.Query<GeneralClass.Customer.Customer>(sql).ToList();

                conn.Close();
            }

            return result;
        }

        public List<GeneralClass.Customer.Customer> GetCustomerByBirthday(DateTime birthday)
        {
            var sql = $"Select {customerColumnSQL} from Customer.Master where Birthday = {birthday:yyyy-dd-mm}";
            List<GeneralClass.Customer.Customer> result;
            using (var conn = new SqlConnection(DBInvoker.ConnectionString))
            {
                conn.Open();

                result = conn.Query<GeneralClass.Customer.Customer>(sql).ToList();

                conn.Close();
            }

            return result;
        }

        public List<GeneralClass.Customer.Customer> GetCustomerByName(string name)
        {
            var sql = $"Select {customerColumnSQL} from Customer.Master where Name Like '%{name}%'";
            List<GeneralClass.Customer.Customer> result;
            using (var conn = new SqlConnection(DBInvoker.ConnectionString))
            {
                conn.Open();

                result = conn.Query<GeneralClass.Customer.Customer>(sql).ToList();

                conn.Close();
            }

            return result;
        }

        public List<GeneralClass.Customer.Customer> GetCustomerByPhoneNumber(string phoneNumber)
        {
            var sql = $"Select {customerColumnSQL} from Customer.Master where FirstPhoneNumber Like '%{phoneNumber}%' or SecondPhoneNumber Like '%{phoneNumber}%'";
            List<GeneralClass.Customer.Customer> result;
            using (var conn = new SqlConnection(DBInvoker.ConnectionString))
            {
                conn.Open();

                result = conn.Query<GeneralClass.Customer.Customer>(sql).ToList();

                conn.Close();
            }

            return result;
        }

        public bool CreateOrUpdateCustomer(GeneralClass.Customer.Customer customer)
        {
            var selectCountSQL = $"Select {customerColumnSQL} from Customer.Master where IDNumber = '{customer.IDNumber}'; ";
            using (var conn = new SqlConnection(DBInvoker.ConnectionString))
            {
                conn.Open();

                var result = conn.QueryFirstOrDefault<GeneralClass.Customer.Customer>(selectCountSQL);
                if (result == null)
                {
                    int maxID = conn.QueryFirstOrDefault<int>("Select Max(ID) +1 from Customer.Master; ");

                    var createSQL = $@"Insert into Customer.Master
                                (ID, Name, Gender, IDNumber, Birthday, FirstPhoneNumber, SecondPhoneNumber, Address, EMail, Line, Note, IsEnable)
                          Values({maxID}, @Name,@Gender, @IDNumber, @Birthday, @FirstPhoneNumber, @SecondPhoneNumber, @Address, @EMail, @Line, @Note,1);";

                    conn.Execute(createSQL, customer);
                    conn.Close();
                    customer.ID = new CustomerID(maxID);
                    return true;
                }
                else
                {
                    var updateSQL = @$"Update Customer.Master 
                                       Set Name = @Name,
                                        Gender= @Gender, IDNumber=@IDNumber, Birthday= @Birthday, 
                                        FirstPhoneNumber=@FirstPhoneNumber, SecondPhoneNumber= @SecondPhoneNumber, Address=@Address, 
                                        EMail=@EMail, Line=@Line, Note=@Note
                                       where ID={customer.ID};";

                    conn.Execute(updateSQL, customer);
                    conn.Close();
                    return false;
                }
            }
        }

        public bool Save(GeneralClass.Customer.Customer customer)
        {
            throw new NotImplementedException();
        }

        public bool Create(GeneralClass.Customer.Customer customer)
        {
            throw new NotImplementedException();
        }

        public bool Delete(CustomerID id)
        {
            throw new NotImplementedException();
        }

        public GeneralClass.Customer.Customer GetCustomerByCusID(CustomerID cusID)
        {
            var sql = $"Select {customerColumnSQL} from Customer.Master where ID = '{cusID.ID}'";
            GeneralClass.Customer.Customer result;
            using (var conn = new SqlConnection(DBInvoker.ConnectionString))
            {
                conn.Open();

                result = conn.QueryFirst<GeneralClass.Customer.Customer>(sql);

                conn.Close();
            }

            return result;
        }

        public List<GeneralClass.Customer.Customer> GetCustomersByCustomerID(IEnumerable<CustomerID> customersId)
        {
            var sql = $"Select {customerColumnSQL} from Customer.Master where ID in ({string.Join(",", customersId.Select(c => c.ID))})";
            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            conn.Open();
            var result = conn.Query<GeneralClass.Customer.Customer>(sql).ToList();
            conn.Close();

            return result;
        }
    }
}
