using demo2.Helpers;
using demo2.Models;
using demo2.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;


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

            var products = _productService.GetProducts();

            foreach(var product in products)
            {
                Products.Add(product);
            }

            if(user.Role.Contains("Авторизированный клиент") || user.Role.Contains("Гость"))
            {
                ControlBtns.Visibility = Visibility.Collapsed;
                ProductsSearch.Visibility = Visibility.Collapsed;
            }
            else if (user.Role.Contains("Менеджер"))
            {
                ControlBtns.Visibility = Visibility.Collapsed;
            }

            DataContext = this;
        }

        public ObservableCollection<ProductCard> Products { get; set; }
        public ProductCard? Product { get; set; }

        public UserContext User { get; set;  }

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

        }

        private void AddProductBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateProductBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteProductBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void FilterBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
