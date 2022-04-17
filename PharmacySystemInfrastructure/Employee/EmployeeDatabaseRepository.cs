using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using Dapper;
using GeneralClass;
using GeneralClass.Employee;
using GeneralClass.Employee.EntityIndex;

namespace PharmacySystemInfrastructure.Employee
{

    public class EmployeeDatabaseRepository : IEmployeeRepository
    {
        public bool CreateNewEmployee(GeneralClass.Employee.Employee employee)
        {
            var selectCountSQL = $"Select Count(*) from Employee.Master where IDNumber = '{employee.IDNumber}'; ";

            using (var scope = new TransactionScope())
            using (var conn = new SqlConnection(DBInvoker.ConnectionString))
            {
                conn.Open();

                if (conn.QueryFirstOrDefault(selectCountSQL) == 0)
                {
                    try
                    {
                        int maxID = conn.QueryFirstOrDefault("Select Max(ID) +1 from Employee.Master; ");

                        var createSQL = $@"Insert Into Employee.Master
                                        (ID,Name,IDNumber,Note,IsEnable)
                                  Values({maxID},@Name,@IDNumber,@Note,{true})";
                        conn.Execute(createSQL, employee);

                        var createAccountSQL = $@" Insert Into Employee.Account
                                     (EMP_ID,Account,Password,AUTH_ID)
                               Values({maxID},@Account,@Password,@AUTH_ID)";
                        conn.Execute(createAccountSQL, employee);

                        scope.Complete();
                        conn.Close();
                        employee.ID = new EmployeeID(maxID);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                {
                    conn.Close();
                    return false;
                }
            }
        }

        public bool Save(GeneralClass.Employee.Employee employee)
        {
            throw new NotImplementedException();
        }

        public bool Create(GeneralClass.Employee.Employee employee)
        {
            throw new NotImplementedException();
        }

        public bool Delete(EmployeeID id)
        {
            throw new NotImplementedException();
        }

        public List<GeneralClass.Employee.Employee> GetAllEmployee()
        {
            List<GeneralClass.Employee.Employee> result;
            var sql = @"Select m.ID,m.Name,m.IDNumber,m.Note,m.IsEnable,acc.Account,acc.Password,acc.AUTH_ID
                            from Employee.Master m join Employee.Account acc on m.ID = acc.EMP_ID
                            join Employee.Authority auth on acc.AUTH_ID = auth.ID
                            where m.IsEnable = 1;";

            using (var conn = new SqlConnection(DBInvoker.ConnectionString))
            {
                conn.Open();
                result = conn.Query<GeneralClass.Employee.Employee>(sql).ToList();
                conn.Close();
            }
            return result;
        }

        public GeneralClass.Employee.Employee Login(string account, string password)
        {
            GeneralClass.Employee.Employee result;
            using (var conn = new SqlConnection(DBInvoker.ConnectionString))
            {
                var sql = $@"Select *
                                from 
                                	Employee.Authority as auth ,
                                	Employee.Master master,
                                	Employee.Account account
                                where
                                	master.ID = account.EMP_ID and 
                                	account.AUTH_ID = auth.ID and
                                	Account = '{account}' and 
                                	Password = '{password}';";
                conn.Open();
                result = conn.QueryFirst<GeneralClass.Employee.Employee>(sql);

                conn.Close();
            }
            return result;
        }
    }
}
