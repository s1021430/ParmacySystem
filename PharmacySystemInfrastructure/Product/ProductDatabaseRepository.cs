using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralClass.Product;

namespace PharmacySystemInfrastructure.Product
{
    public class ProductDatabaseRepository : IProductRepository
    {
        private readonly SqlConnection connection;
    }
}
