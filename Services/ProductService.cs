using demo2.Helpers;
using demo2.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;

namespace demo2.Services
{
    public class ProductService
    {
        private readonly DemoDb2Context _context;

        public ProductService(DemoDb2Context context)
        {
            _context = context;
        }

        private List<Product> GetProductsFromDb()
        {
            var products = _context.Products
                .Include(p => p.IdCategoryNavigation)
                .Include(p => p.IdManufacturerNavigation)
                .Include(p => p.IdSupplierNavigation)
                .Include(p => p.OrderItems)
                .AsQueryable();

            return products.ToList();
        }

        public ObservableCollection<ProductCard> GetProducts()
        {
            var productsFromDb = GetProductsFromDb();

            ObservableCollection<ProductCard> products = [];

            foreach (var item in productsFromDb)
            {
                ProductCard product = new ProductCard()
                {
                    Id = item.IdProduct,
                    Name = item.Name,
                    Category = item.IdCategoryNavigation.Name,
                    Image = $"/Images/{item.Image ?? "picture.png"}",
                    Description = item.Description,
                    ManufacturerName = item.IdManufacturerNavigation.Name,
                    SupplierName = item.IdSupplierNavigation.Name,
                    UnitOfMeasurement = item.UnitOfMeasurement,
                    CountInStock = item.CountInStock,
                    CurrentDiscount = item.CurrentDiscount,
                    OldPrice = item.Price,
                    Price = item.CurrentDiscount > 0 ? item.Price - item.Price * item.CurrentDiscount / 100m : item.Price,
                    BackgroundColor = item.CurrentDiscount > 15 ? " #2E8B57" : "#7FFF00"
                };

                Visibility oldProceVisibilyty = product.OldPrice == product.Price ? Visibility.Collapsed : Visibility.Visible;

                product.OldPriceVisibility = oldProceVisibilyty;

                products.Add(product);
            }

            return products;
        }
    }
}
