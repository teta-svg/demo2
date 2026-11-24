using demo2.Helpers;
using demo2.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.IO;
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

        private ProductCard FromProductToProductCard(Product item)
        {
            ProductCard product = new ProductCard()
            {
                Id = item.IdProduct,
                Artikul = item.Artikul,
                Name = item.Name,
                Category = new CategotyInfo
                {
                    Id = item.IdCategoryNavigation.IdCategory,
                    Name = item.IdCategoryNavigation.Name
                },
                Image = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", item.Image ?? "picture.png"),
                Description = item.Description,
                Manufacturer = new ManufacturerInfo
                {
                    Id = item.IdManufacturerNavigation.IdManufacturer,
                    Name = item.IdManufacturerNavigation.Name
                },
                Supplier = new SupplierInfo
                {
                    Id = item.IdSupplierNavigation.IdSupplier,
                    Name = item.IdSupplierNavigation.Name
                },
                UnitOfMeasurement = item.UnitOfMeasurement,
                CountInStock = item.CountInStock,
                CurrentDiscount = item.CurrentDiscount,
                OldPrice = item.Price,
                Price = item.CurrentDiscount > 0 ? item.Price - item.Price * item.CurrentDiscount / 100m : item.Price,
                BackgroundColor = item.CurrentDiscount > 15 ? " #2E8B57" : "#7FFF00"
            };

            Visibility oldProceVisibilyty = product.OldPrice == product.Price ? Visibility.Collapsed : Visibility.Visible;

            product.OldPriceVisibility = oldProceVisibilyty;

            return product;
        }

        private List<Product> GetProductsFromDb(int supplierId, string? searchText = null)
        {
            var products = _context.Products
                .AsNoTracking()
                .Include(p => p.IdCategoryNavigation)
                .Include(p => p.IdManufacturerNavigation)
                .Include(p => p.IdSupplierNavigation)
                .Include(p => p.OrderItems)
                .AsQueryable();

            
            if (supplierId != 0)
            {
                products = products.Where(p => p.IdSupplier == supplierId);
            };

            if (searchText != null)
            {
                products = products
                    .Where(p => p.Name.Contains(searchText) ||
                    p.Artikul.Contains(searchText) ||
                    (p.Description != null && p.Description.Contains(searchText)) ||
                    p.IdManufacturerNavigation.Name.Contains(searchText) ||
                    p.IdSupplierNavigation.Name.Contains(searchText) ||
                    p.IdCategoryNavigation.Name.Contains(searchText) ||
                    p.UnitOfMeasurement.Contains(searchText));
            }

            return products.ToList();
        }

        private Product? GetProductByIdFromDb(int productId)
        {
            return _context.Products
                .Include(p => p.IdCategoryNavigation)
                .Include(p => p.IdManufacturerNavigation)
                .Include(p => p.IdSupplierNavigation)
                .Include(p => p.OrderItems)
                .FirstOrDefault(p => p.IdProduct == productId);
        }

        private List<Manufacturer> GetAllManufacturersFromDb()
        {
            return _context.Manufacturers.ToList();
        }

        private List<Caterory> GetAllCategoriesFromDb()
        {
            return _context.Caterories.ToList();
        }

        public List<Supplier> GetAllSuppliersFromDb()
        {
            var suppliers = _context.Suppliers.ToList();

            return suppliers;
        }

        public ObservableCollection<ProductCard> GetProducts(int supplierId, string? searchText = null)
        {
            var productsFromDb = GetProductsFromDb(supplierId, searchText);

            ObservableCollection<ProductCard> products = [];

            foreach (var item in productsFromDb)
            {
                products.Add(FromProductToProductCard(item));
            }

            return products;
        }

        public ObservableCollection<CategotyInfo> GetAllCategories()
        {
            ObservableCollection<CategotyInfo> categories = [];

            var categoriesFromDb = GetAllCategoriesFromDb();

            foreach (var category in categoriesFromDb)
            {
                categories.Add(new CategotyInfo
                {
                    Id = category.IdCategory,
                    Name = category.Name
                });
            }

            return categories;
        }

        public ObservableCollection<ManufacturerInfo> GetAllManufacturers()
        {
            ObservableCollection<ManufacturerInfo> manufacturers = [];

            var manufacturersFromDb = GetAllManufacturersFromDb();

            foreach (var manufacturer in manufacturersFromDb)
            {
                manufacturers.Add(new ManufacturerInfo
                {
                    Id = manufacturer.IdManufacturer,
                    Name = manufacturer.Name
                });
            }

            return manufacturers;
        }

        public ObservableCollection<SupplierInfo> GetAllSuppliers()
        {
            ObservableCollection<SupplierInfo> suppliers = [];

            var suppliersFromDb = GetAllSuppliersFromDb();

            suppliers.Add(new SupplierInfo
            {
                Id = 0,
                Name = "Все поставщики"
            });

            foreach (var supplier in suppliersFromDb)
            {
                suppliers.Add(new SupplierInfo
                {
                    Id = supplier.IdSupplier,
                    Name = supplier.Name
                });
            }
            return suppliers;
        }

        public string DeleteProductById(int productId)
        {
            var product = GetProductByIdFromDb(productId);

            if (product == null) return "товар не найден";

            bool isProductInList = product.OrderItems?.Any(oi => oi.IdProduct == productId) ?? false;

            if (isProductInList) return "товар есть в заказе";

            _context.Products.Remove(product);
            _context.SaveChanges();

            return "товар успешно удален";
        }

        public ProductCard? GetProductById(int productId)
        {
            var productFromDb = GetProductByIdFromDb(productId);

            if (productFromDb == null) return null;

            return FromProductToProductCard(productFromDb);

        }

        private int FindSupplier(string name)
        {
            var supplier = _context.Suppliers.FirstOrDefault(x => x.Name.ToLower().Contains(name.ToLower()));

            if (supplier == null) return 0;

            return supplier.IdSupplier;
        }

        public bool AddProduct(ProductCard productInfo)
        {
            try
            {
                int newProductId = _context.Products.Max(p => p.IdProduct) + 1;

                int idSupplier = FindSupplier(productInfo.Supplier.Name);

                if (idSupplier == 0)
                {
                    idSupplier = _context.Suppliers.Max(s => s.IdSupplier) + 1;

                    _context.Suppliers.Add(new Supplier()
                    {
                        IdSupplier = idSupplier,
                        Name = productInfo.Supplier.Name
                    });
                }

                string? imageFileName = null;

                if (!string.IsNullOrEmpty(productInfo.Image) && !productInfo.Image.EndsWith("picture.png"))
                {
                    imageFileName = System.IO.Path.GetFileName(productInfo.Image);
                }


                Product product = new()
                {
                    IdProduct = newProductId,
                    Name = productInfo.Name,
                    Artikul = productInfo.Artikul,
                    UnitOfMeasurement = productInfo.UnitOfMeasurement,
                    Price = productInfo.OldPrice ?? 0,
                    IdSupplier = idSupplier,
                    IdManufacturer = productInfo.Manufacturer.Id,
                    IdCategory = productInfo.Category.Id,
                    CurrentDiscount = productInfo.CurrentDiscount ?? 0,
                    CountInStock = productInfo.CountInStock,
                    Description = productInfo.Description,
                    Image = imageFileName
                };


                _context.Products.Add(product);
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool UpdateProduct(ProductCard productInfo)
        {
            try
            {
                var existingProduct = _context.Products.FirstOrDefault(p => p.IdProduct == productInfo.Id);

                if (existingProduct == null) return false;

                int newProductId = _context.Products.Max(p => p.IdProduct) + 1;

                int idSupplier = FindSupplier(productInfo.Supplier.Name);

                if (idSupplier == 0)
                {
                    idSupplier = _context.Suppliers.Max(s => s.IdSupplier) + 1;

                    _context.Suppliers.Add(new Supplier()
                    {
                        IdSupplier = idSupplier,
                        Name = productInfo.Supplier.Name
                    });
                }

                string? imageFileName = null;

                if (!string.IsNullOrEmpty(productInfo.Image) && !productInfo.Image.EndsWith("picture.png"))
                {
                    imageFileName = Path.GetFileName(productInfo.Image);
                }

                existingProduct.Name = productInfo.Name;
                existingProduct.Artikul = productInfo.Artikul;
                existingProduct.UnitOfMeasurement = productInfo.UnitOfMeasurement;
                existingProduct.Price = productInfo.OldPrice ?? 0;
                existingProduct.IdSupplier = idSupplier;
                existingProduct.IdManufacturer = productInfo.Manufacturer.Id;
                existingProduct.IdCategory = productInfo.Category.Id;
                existingProduct.CurrentDiscount = productInfo.CurrentDiscount ?? 0;
                existingProduct.CountInStock = productInfo.CountInStock;
                existingProduct.Description = productInfo.Description;
                existingProduct.Image = imageFileName;

                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
