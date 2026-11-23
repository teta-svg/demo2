using demo2.Helpers;
using demo2.Models;
using demo2.Services;
using demo2.Views;
using System.Windows;

namespace demo2
{
    public partial class LoginWindow : Window
    {
        private readonly DemoDb2Context _context;
        private UserContext _user;
        private readonly LoginService _loginService;
        public LoginWindow()
        {
            InitializeComponent();

            _context = new();
            _user = new();
            _loginService = new(_context);

            DataContext = this;
        }

        public string Login { get; set; } = string.Empty;

        private void OpenWindow()
        {
            Application.Current.MainWindow = new ProductsWindow(_user);
            Application.Current.MainWindow.Show();

            this.Close();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Password.Password) || string.IsNullOrEmpty(Login))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _user = _loginService.Login(Login, Password.Password, isGuest: false)!;

            if (_user == null)
            {
                MessageBox.Show("Неправильный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            OpenWindow();

        }

        private void LoginAsGuestBtn_Click(object sender, RoutedEventArgs e)
        {
            _user = _loginService.Login(Login, Password.Password, isGuest: true)!;

            OpenWindow();
        }
    }
}