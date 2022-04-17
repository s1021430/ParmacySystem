using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralClass.Product;
using PharmacySystemInfrastructure.Product;

namespace PharmacySystem.ApplicationServiceLayer
{
    public static class ProductApplicationServiceFactory
    {
        public static ProductApplicationService GetProductApplicationService()
        {
            var productDatabaseRepository = RepositoryProvider.ProductRepository;
            return new ProductApplicationService(productDatabaseRepository);
        }
    }
    public class ProductApplicationService
    {
        private readonly IProductRepository productRepository;
        public ProductApplicationService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
    }
}
