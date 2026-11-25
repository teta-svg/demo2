using demo2.Helpers;
using demo2.Models;
using demo2.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace demo2.Views
{
    public partial class OrderWindow : Window
    {
        private readonly DemoDb2Context _context;
        private readonly OrderService _orderService;

        public OrderWindow(UserContext user)
        {
            InitializeComponent();

            User = user;

            _context = new();
            _orderService = new(_context);

            Orders = _orderService.GetAllOrders();

            if (user.Role.Contains("Менеджер"))
            {
                
                DeleteOrderBtn.Visibility = Visibility.Collapsed;
                AddOrderBtn.Visibility = Visibility.Collapsed;
            }

            DataContext = this;
        }

        public UserContext User { get; set; }
        public ObservableCollection<OrderCard> Orders { get; set; }
        public OrderCard? Order { get; set;}

        private void RefreshOrders()
        {
            _context.ChangeTracker.Clear();

            ObservableCollection<OrderCard> updateOrders = _orderService.GetAllOrders();

            Orders.Clear();

            foreach(var order in updateOrders)
            {
                Orders.Add(order);
            }
        }

        private void OpenProductsBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow = new ProductsWindow(User);
            Application.Current.MainWindow.Show();

            this.Close();
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

        private void ListBoxItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (User.Role.Contains("Менеджер"))
            {
                return;
            }

            if (Order == null)
            {
                MessageBox.Show("Выберите заказ", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Application.Current.MainWindow = new OrderCardWindow("edit", User, Order.IdOrder);
            Application.Current.MainWindow.ShowDialog();

            RefreshOrders();
        }

        private void AddOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow = new OrderCardWindow("add", User);
            Application.Current.MainWindow.ShowDialog();

            RefreshOrders();
        }

        private void DeleteOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Order == null)
            {
                MessageBox.Show("Выберите заказ", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить заказ?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            if (!_orderService.DeleteOrderById(Order.IdOrder))
            {
                MessageBox.Show("Ошибка удаления", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Заказ удален", "успех", MessageBoxButton.OK, MessageBoxImage.Information);

            RefreshOrders();
        }
    }
}
