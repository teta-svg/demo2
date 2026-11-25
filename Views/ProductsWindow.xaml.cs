using demo2.Helpers;
using demo2.Models;
using demo2.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace demo2.Views
{
    public partial class ProductsWindow : Window
    {
        private readonly DemoDb2Context _context;
        private readonly ProductService _productService;
        
        public ProductsWindow(UserContext user)
        {
            InitializeComponent();

            _context = new();
            _productService = new(_context);

            User = user;
            Products = [];

            Suppliers = _productService.GetAllSuppliers();
            Supplier = Suppliers.FirstOrDefault();

            var products = _productService.GetProducts(Supplier?.Id ?? 0);

            foreach(var product in products)
            {
                Products.Add(product);
            }

            if(user.Role.Contains("Авторизированный клиент") || user.Role.Contains("Гость"))
            {
                ControlBtns.Visibility = Visibility.Collapsed;
                ProductsSearch.Visibility = Visibility.Collapsed;
                OpenOrderBtn.Visibility = Visibility.Collapsed;
            }
            else if (user.Role.Contains("Менеджер"))
            {
                ControlBtns.Visibility = Visibility.Collapsed;
            }

            DataContext = this;
        }

        public ObservableCollection<ProductCard>? Products { get; set; }
        public ProductCard? Product { get; set; }
        public UserContext User { get; set; }
        public ObservableCollection<SupplierInfo> Suppliers { get; set; }
        public SupplierInfo? Supplier { get; set; }
        public string? SearchText { get; set; }
        public int? SelectedSort { get; set; }

        private void RefreshList()
        {
            if (Supplier == null)
            {
                MessageBox.Show("Поставщик не выбран", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Products == null)
            {
                MessageBox.Show("Список товаров пуст", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            };

            ObservableCollection<ProductCard> updateList;

            if (!string.IsNullOrEmpty(SearchText))
            {
                updateList = _productService.GetProducts(Supplier.Id, SearchText);
            }
            else
            {
                updateList = _productService.GetProducts(Supplier.Id);
            }
            if (SelectedSort != null)
            {
                if (SelectedSort == 0)
                {
                    updateList = new ObservableCollection<ProductCard>(updateList.OrderBy(p => p.CountInStock));
                }
                else if (SelectedSort == 1)
                {
                    updateList = new ObservableCollection<ProductCard>(updateList.OrderByDescending(p => p.CountInStock));
                };
            }

            Products.Clear();

            if (updateList.Count == 0)
            {
                MessageBox.Show("Товары не найдены", "сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            foreach (var item in updateList)
            {
                Products.Add(item);
            }
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            Application.Current.MainWindow = new LoginWindow();
            Application.Current.MainWindow.Show();

            this.Close();
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!User.Role.Contains("Администратор"))
            {
                return;
            }

            if (Product == null)
            {
                MessageBox.Show("Товар не выбран", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Application.Current.MainWindow = new ProductCardWindow("edit", Product.Id);
            Application.Current.MainWindow.ShowDialog();

            RefreshList();
        }

        private void AddProductBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow = new ProductCardWindow("add");
            Application.Current.MainWindow.ShowDialog();

            RefreshList();
        }

        private void DeleteProductBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Product == null)
            {
                MessageBox.Show("Товар не выбран", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить товар?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            string message = _productService.DeleteProductById(Product.Id);

            MessageBox.Show(message, "сообщение", MessageBoxButton.OK, MessageBoxImage.Information);

            RefreshList();
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RefreshList();
        }

        private void FilterBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RefreshList();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            RefreshList();
        }

        private void ProductImage_Loaded(object sender, RoutedEventArgs e)
        {
            var image = (System.Windows.Controls.Image)sender;

            if (image.DataContext is ProductCard p && !string.IsNullOrEmpty(p.Image) && System.IO.File.Exists(p.Image))
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = new Uri(p.Image);
                bmp.EndInit();

                image.Source = bmp;
            }
        }

        private void OpenOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow = new OrderWindow(User);
            Application.Current.MainWindow.Show();

            this.Close();
        }
    }
}
