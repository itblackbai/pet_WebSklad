using webSklad.Models;

namespace webSklad.Interfaces
{
    public interface IProductRepository
    {
        public IEnumerable<Product> GetProducts();
        public Product GetProduct(string sku);

        Task<bool> SaveAsync();
        Task<bool> Save();
        Task<bool> CreateProduct(Product product);

        bool AnyProductWithBarcodeOwn(string barcodeOwn);

        IEnumerable<Product> FilterProducts(string userProductId, string productCode, string productName, string barcode, string barcodeOwn);

        Task<Product> GetProductById(int productId);

        Task<bool> DeleteProduct(Product product);
    }
}
