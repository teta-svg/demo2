using demo2.Helpers;
using demo2.Models;
using demo2.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace demo2.Views
{
    public partial class ProductCardWindow : Window
    {
        private readonly DemoDb2Context _context;
        private readonly ProductService _productService;

        public ProductCardWindow(string windowType, int? productId = null)
        {
            InitializeComponent();

            _context = new();
            _productService = new(_context);

            Categories = _productService.GetAllCategories();
            Manufacturers = _productService.GetAllManufacturers();

            if (windowType.Contains("edit"))
            {
                ButtonType = "Изменить";
                ImageButtonType = "Изменить картинку";

                Product = _productService.GetProductById(productId ?? 0);

                if (Product == null)
                {
                    MessageBox.Show("Товар не найден", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                SelectedCategory = Categories.FirstOrDefault(c => c.Id == Product.Category.Id);
                SelectedManufacturer = Manufacturers.FirstOrDefault(m => m.Id == Product.Manufacturer.Id);
                SupplierName = Product.Supplier.Name ?? "";
                PriceBox.Text = Product.OldPrice?.ToString("0.##") ?? "";
                DiscountBox.Text = Product.CurrentDiscount?.ToString("0.##") ?? "";
                _oldImagePath = Product.Image;
            }
            else
            {
                ButtonType = "Добавить";
                ImageButtonType = "Добавить картинку";
                ProductIdBlock.Visibility = Visibility.Collapsed;

                Product = new();
            }

            string imagePath = string.IsNullOrEmpty(Product?.Image) ? 
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "picture.png")
                : Product.Image;

            if (File.Exists(imagePath))
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = new Uri(imagePath);
                bmp.EndInit();
                ProductImage.Source = bmp;
            }

            DataContext = this;
        }

        public ProductCard? Product { get; set; }
        public string ButtonType { get; set; }
        public string ImageButtonType { get; set; }
        public ObservableCollection<CategotyInfo> Categories { get; set; }
        public CategotyInfo? SelectedCategory { get; set; }
        public ObservableCollection<ManufacturerInfo> Manufacturers { get; set; }
        public ManufacturerInfo? SelectedManufacturer { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        private string _oldImagePath;
        private string? _newImageSourcePath;


        private void ChangeImageBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "Изображения (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
                };

                if (openFileDialog.ShowDialog() != true) return;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(openFileDialog.FileName);
                bitmap.EndInit();

                if (bitmap.PixelWidth > 300 || bitmap.PixelHeight > 200)
                {
                    MessageBox.Show("Фото должно быть не больше 300x200 пикселей!");
                    return;
                }

                _newImageSourcePath = openFileDialog.FileName;
                ProductImage.Source = bitmap;
                Product.Image = openFileDialog.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private bool Validate()
        {
            if (SelectedManufacturer == null)
            {
                MessageBox.Show("Выберите производителя", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            Product.Manufacturer = SelectedManufacturer;

            if (SelectedCategory == null)
            {
                MessageBox.Show("Выберите категорию", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            Product.Category = SelectedCategory;

            if (string.IsNullOrEmpty(Product.Name))
            {
                MessageBox.Show("Введите название", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(Product.Artikul))
            {
                MessageBox.Show("Введите артикул", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(SupplierName))
            {
                MessageBox.Show("Введите поставщика", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(Product.UnitOfMeasurement))
            {
                MessageBox.Show("Введите единицу измерения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            Product.Supplier.Name = SupplierName;

            string priceText = PriceBox.Text.Replace('.', ',');
            string discountText = DiscountBox.Text.Replace('.', ',');
            if (!decimal.TryParse(priceText, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену больше нуля", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            Product.OldPrice = price;

            if (!int.TryParse(CountBox.Text, out int count) || count < 0)
            {
                MessageBox.Show("Количество должно быть целым неотрицательным числом", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            Product.CountInStock = count;

            if (!decimal.TryParse(discountText, out decimal discount) || discount < 0 || discount > 100)
            {
                MessageBox.Show("Скидка должна быть от 0 до 100%", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            Product.CurrentDiscount = discount;

            return true;
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;

            if (ButtonType.Contains("Изменить"))
            {
                if (!_productService.UpdateProduct(Product))
                {
                    MessageBox.Show("Ошибка обновления", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBox.Show("Товар обновлен", "успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (!_productService.AddProduct(Product))
                {
                    MessageBox.Show("Ошибка добавления", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBox.Show("Товар добавлен", "успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            if (_newImageSourcePath != null)
            {
                try
                {
                    string newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", Path.GetFileName(_newImageSourcePath));

                    if (!File.Exists(newPath))
                    {
                        File.Copy(_newImageSourcePath, newPath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка копирования картинки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (_oldImagePath != null && !_oldImagePath.EndsWith("picture.png"))
            {
                string oldFileName = Path.GetFileName(_oldImagePath);
                string newFileName = Path.GetFileName(Product.Image ?? "");

                if (oldFileName != newFileName && !_context.Products.Any(p => p.Image == oldFileName))
                {
                    try
                    {
                        File.Delete(_oldImagePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "ошибка удаления");
                    }
                }
            }

            this.Close();

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
