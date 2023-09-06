using Microsoft.EntityFrameworkCore;
using webSklad.Data;
using webSklad.Interfaces;
using webSklad.Models;

namespace webSklad.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly WebSkladContext _context;

        public ProductRepository(WebSkladContext context)
        {
            _context = context;
        }

        public Task<bool> CreateProduct(Product product)
        {
            _context.Products.Add(product);
            return Save();
        }

        public Product GetProduct(string sku)
        {
            return GetProducts().Where(w => w.Sku == sku).FirstOrDefault();
        }

        public IEnumerable<Product> GetProducts()
        {
            return _context.Products.ToList();
        }

        public async Task<bool> SaveAsync()
        {
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public bool AnyProductWithBarcodeOwn(string barcodeOwn)
        {
            return  _context.Products.Any(p => p.BarCodeOwn == barcodeOwn);
        }

        public IEnumerable<Product> FilterProducts(string userProductId, string productCode, string productName, string barcode, string barcodeOwn)
        {
            var filteredProducts = _context.Products
                .Where(p =>
                    p.UserProductId == userProductId &&
                    (string.IsNullOrEmpty(productCode) || p.Sku.Contains(productCode)) &&
                    (string.IsNullOrEmpty(productName) || p.ProductName.Contains(productName)) &&
                    (string.IsNullOrEmpty(barcode) || p.BarCode.Contains(barcode)) &&
                    (string.IsNullOrEmpty(barcodeOwn) || p.BarCodeOwn.Contains(barcodeOwn))
                )
                .Take(10)
                .ToList();

            return filteredProducts;
        }


        public async Task<Product> GetProductById(int productId)
        {
            return _context.Products.FirstOrDefault(p => p.Id == productId);
        }



        public Task<bool> DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            return Save();
        }
    }
}
