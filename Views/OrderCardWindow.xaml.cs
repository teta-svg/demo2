using demo2.Helpers;
using demo2.Models;
using demo2.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace demo2.Views
{
    public partial class OrderCardWindow : Window
    {
        private readonly DemoDb2Context _context;
        private readonly OrderService _orderService;
        private readonly ProductService _productService;
        public OrderCardWindow(string windowType, UserContext user, int? orderId = null)
        {
            InitializeComponent();

            User = user;

            _context = new();
            _orderService = new(_context);
            _productService = new(_context);

            Products = _productService.GetProducts();

            if (windowType.Contains("edit"))
            {
                ButtonType = "Изменить";

                Order = _orderService.GetOrderById(orderId);             

                if (Order == null)
                {
                    MessageBox.Show("Заказ не найден", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _totalPrice = Order.TotalPrice;

            }
            else
            {
                ButtonType = "Добавить";
                OrderIdBlock.Visibility = Visibility.Collapsed;
                Order = new();
                _totalPrice = 0;
            }

            DataContext = this;
        }
        private decimal? _totalPrice ;
        public UserContext User { get; set; }
        public OrderCard? Order { get; set; }
        public string ButtonType { get; set; }
        public ObservableCollection<OrderItemCard> Products { get; set; }
        public OrderItemCard? SelectedProduct { get; set; }
        public OrderItemCard? OrderItem { get; set; }
        public int ProductCount { get; set; } = 0;

        private bool ValidateOrder()
        {
            if (Order == null)
            {
                MessageBox.Show("Данные заказа не заполнены", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (Order.DeliveryDate.HasValue && Order.DeliveryDate.Value.Date < Order.OrderDate.Date)
            {
                MessageBox.Show("Дата доставки не может быть раньше даты заказа", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Order.Person.Surname))
            {
                MessageBox.Show("Введите фамилию заказчика", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Order.Person.FirstName))
            {
                MessageBox.Show("Введите имя заказчика", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (Order.Adress.Index <= 0)
            {
                MessageBox.Show("Введите корректный индекс", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Order.Adress.Sity.Name))
            {
                MessageBox.Show("Введите город", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Order.Adress.Street))
            {
                MessageBox.Show("Введите улицу", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }


            if (Order.Kod.HasValue && Order.Kod <= 0)
            {
                MessageBox.Show("Код получения должен быть больше 0", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Order.OrderStatus))
            {
                MessageBox.Show("Введите статус заказа", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (Order.OrderItems.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один товар в заказ", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {

            if (!ValidateOrder())
            {
                return;
            }

            if (ButtonType.Contains("Изменить"))
            {
                if (!_orderService.UpdateOrder(Order!))
                {
                    MessageBox.Show("Ошибка обновления", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBox.Show("Заказ обновлен", "успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (!_orderService.AddOrder(Order!))
                {
                    MessageBox.Show("Ошибка добавления", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBox.Show("Заказ добавлен", "успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            this.Close();

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddProductBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProduct == null)
            {
                MessageBox.Show("Выберите товар", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(ProductCount <= 0)
            {
                MessageBox.Show("Количество должно быть больше 0", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(SelectedProduct.CountInStock <  ProductCount)
            {
                MessageBox.Show("Недостаточно товара в наличии", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Order.OrderItems.Any(oi => oi.IdProduct == SelectedProduct.IdProduct))
            {
                MessageBox.Show("Товар уже есть в списке", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Order.OrderItems.Add(new OrderItemCard()
            {
                IdProduct = SelectedProduct.IdProduct,
                Name = SelectedProduct.Name,
                Price = SelectedProduct.Price,
                Count = ProductCount,
                Artikul = SelectedProduct.Artikul
            });

            SelectedProduct.CountInStock -= ProductCount;

            _totalPrice += (SelectedProduct.Price ?? 0) * ProductCount;
            PriceBox.Text = $"{_totalPrice:0.##} руб.";
        }

        private void DeleteProductBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OrderItem == null)
            {
                MessageBox.Show("Выберите товар", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить товар из заказа?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            var productDelete = Order.OrderItems.FirstOrDefault(oi => oi.IdProduct == OrderItem.IdProduct);
            var productCountUpdate = Products.FirstOrDefault(p => p.IdProduct == productDelete.IdProduct);

            productCountUpdate.CountInStock += productDelete.Count;

            _totalPrice -= (productDelete.Price ?? 0) * productDelete.Count;
            PriceBox.Text = $"{_totalPrice:0.##} руб.";

            Order.OrderItems.Remove(productDelete);
        }
    }
}
