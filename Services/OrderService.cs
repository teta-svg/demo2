using demo2.Helpers;
using demo2.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace demo2.Services
{
    public class OrderService
    {
        private readonly DemoDb2Context _context;
        private readonly ProductService _productService;

        public OrderService(DemoDb2Context context)
        {
            _context = context;
            _productService = new(_context);
        }

        private OrderCard FromOrderToOrderCard(Order o)
        {
            ObservableCollection<OrderItemCard> orderItems = [];

            decimal? totalPrice = 0;

            foreach (var item in o.OrderItems)
            {
                decimal? currentOrderPrice = item.IdProductNavigation.CurrentDiscount > 0 ?
                    item.IdProductNavigation.Price - item.IdProductNavigation.Price * item.IdProductNavigation.CurrentDiscount / 100m :
                    item.IdProductNavigation.Price;

                totalPrice += item.Count * currentOrderPrice;

                orderItems.Add(new OrderItemCard()
                {
                    IdProduct = item.IdProduct,
                    Count = item.Count,
                    Artikul = item.IdProductNavigation.Artikul,
                    Name = item.IdProductNavigation.Name,
                    Price = currentOrderPrice,
                    CountInStock = item.IdProductNavigation.CountInStock
                });
            }

            return new OrderCard()
            {
                IdOrder = o.IdOrder,
                OrderDate = o.OrderDate.ToDateTime(TimeOnly.MinValue),
                DeliveryDate = o.DeliveryDate?.ToDateTime(TimeOnly.MinValue),
                TotalPrice = totalPrice,
                Adress = new OrderAdress()
                {
                    IdAdress = o.IdAdressNavigation.IdAdress,
                    Index = o.IdAdressNavigation.Index,
                    Sity = new OrderSity()
                    {
                        IdSity = o.IdAdressNavigation.IdSityNavigation.IdSity,
                        Name = o.IdAdressNavigation.IdSityNavigation.Name
                    },
                    Street = o.IdAdressNavigation.Street,
                    House = o.IdAdressNavigation.House
                },
                Person = new OrderPerson()
                {
                    Id = o.IdPersonNavigation.IdPerson,
                    Surname = o.IdPersonNavigation.Surname,
                    FirstName = o.IdPersonNavigation.FirstName,
                    Patronymic = o.IdPersonNavigation.Patronymic ?? ""
                },
                Kod = o.Kod,
                OrderStatus = o.OrderStatus,
                OrderItems = orderItems
            };
        }

        private List<Order> GetAllOrdersFromDb()
        {
            return _context.Orders
                .Include(o => o.IdAdressNavigation)
                    .ThenInclude(a => a.IdSityNavigation)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.IdProductNavigation)
                .Include(o => o.IdPersonNavigation)
                .ToList();
        }

        private Order? GetOrderByIdFromDb(int? id)
        {
            return _context.Orders
                .Include(o => o.IdAdressNavigation)
                    .ThenInclude(a => a.IdSityNavigation)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.IdProductNavigation)
                .Include(o => o.IdPersonNavigation)
                .FirstOrDefault(o => o.IdOrder == id);
        }

        public ObservableCollection<OrderCard> GetAllOrders()
        {
            var ordersFromDb = GetAllOrdersFromDb();

            ObservableCollection<OrderCard> orders = [];

            foreach(var o in ordersFromDb)
            {
                orders.Add(FromOrderToOrderCard(o));
            }

            return orders;
        }

        public bool DeleteOrderById(int id)
        {
            var order = GetOrderByIdFromDb(id);

            if (order == null) return false;

            try
            {
                foreach (var o in order.OrderItems)
                {
                    _context.OrderItems.Remove(o);
                }

                _context.Orders.Remove(order);
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public OrderCard? GetOrderById(int? id)
        {
            var orderFromDb = GetOrderByIdFromDb(id);

            if (orderFromDb == null) return null;

            return FromOrderToOrderCard(orderFromDb);
        }

        private int FindSity(OrderSity orderSity)
        {
            var sity = _context.Sities.FirstOrDefault(s => s.Name == orderSity.Name);

            if (sity == null) return 0;

            return sity.IdSity;
        }

        private int FindAdress(OrderAdress orderAdress)
        {
            int sityId = FindSity(orderAdress.Sity);

            if (sityId == 0) return 0;

            var adress = _context.Adresses
                .FirstOrDefault(a => a.House ==  orderAdress.House &&
                a.Index == orderAdress.Index && a.Street == orderAdress.Street && a.IdSity == sityId);

            if (adress == null) return 0;

            return adress.IdAdress;

        }

        private int FindPerson(OrderPerson orderPerson)
        {
            var person = _context.People.FirstOrDefault(p => p.FirstName ==  orderPerson.FirstName
            && p.Patronymic == orderPerson.Patronymic && orderPerson.Surname == p.Surname);

            if (person == null) return 0;

            return person.IdPerson;
        }

        public bool AddOrder(OrderCard orderCard)
        {
            try
            {
                int newIdOrder = _context.Orders.Max(o => o.IdOrder) + 1;

                int idAdress = FindAdress(orderCard.Adress);
                int idSity = FindSity(orderCard.Adress.Sity);
                int idPerson = FindPerson(orderCard.Person);
                int idOrderItems = _context.OrderItems.Max(oi => oi.IdOrderItem);

                if(idSity == 0)
                {
                    idSity = _context.Sities.Max(s => s.IdSity) + 1;

                    _context.Sities.Add(new Sity()
                    {
                        IdSity = idSity,
                        Name = orderCard.Adress.Sity.Name
                    });
                }

                if (idAdress == 0)
                {
                    idAdress = _context.Adresses.Max(a => a.IdAdress) + 1;

                    _context.Adresses.Add(new Adress()
                    {
                        IdAdress = idAdress,
                        Index = orderCard.Adress.Index,
                        IdSity = idSity,
                        Street = orderCard.Adress.Street,
                        House = orderCard.Adress.House ?? null
                    });
                }

                if (idPerson == 0)
                {
                    idPerson = _context.People.Max(a => a.IdPerson) + 1;

                    _context.People.Add(new Person()
                    {
                        IdPerson = idPerson,
                        Surname = orderCard.Person.Surname,
                        FirstName = orderCard.Person.FirstName,
                        Patronymic = orderCard.Person.Patronymic ?? null
                    });
                }

                List<OrderItem> orderItems = [];
                Product product;
                
                foreach (var items in orderCard.OrderItems)
                {
                    orderItems.Add(new OrderItem()
                    {
                        IdOrder = newIdOrder,
                        IdProduct = items.IdProduct,
                        IdOrderItem = ++idOrderItems,
                        Count = items.Count,
                    });

                    product = _productService.GetProductByIdFromDb(items.IdProduct)!;
                    product.CountInStock -= items.Count;
                }

                Order order = new()
                {
                    IdOrder = newIdOrder,
                    OrderDate = DateOnly.FromDateTime(orderCard.OrderDate),
                    DeliveryDate = orderCard.DeliveryDate.HasValue ? 
                        DateOnly.FromDateTime(orderCard.DeliveryDate.Value) : 
                        null,
                    IdAdress = idAdress,
                    IdPerson = idPerson,
                    Kod = orderCard.Kod,
                    OrderStatus = orderCard.OrderStatus,
                    OrderItems = orderItems
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public bool UpdateOrder(OrderCard orderCard)
        {
            try
            {

                var existingOrder = _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefault(o => o.IdOrder == orderCard.IdOrder);

                if (existingOrder == null)
                {
                    return false;
                }

                int idAdress = FindAdress(orderCard.Adress);
                int idSity = FindSity(orderCard.Adress.Sity);
                int idPerson = FindPerson(orderCard.Person);
                int idOrderItems = _context.OrderItems.Max(oi => oi.IdOrderItem);

                if(idSity == 0)
                {
                    idSity = _context.Sities.Max(s => s.IdSity) + 1;

                    _context.Sities.Add(new Sity()
                    {
                        IdSity = idSity,
                        Name = orderCard.Adress.Sity.Name
                    });
                }

                if (idAdress == 0)
                {
                    idAdress = _context.Adresses.Max(a => a.IdAdress) + 1;

                    _context.Adresses.Add(new Adress()
                    {
                        IdAdress = idAdress,
                        Index = orderCard.Adress.Index,
                        IdSity = idSity,
                        Street = orderCard.Adress.Street,
                        House = orderCard.Adress.House ?? null
                    });
                }

                if (idPerson == 0)
                {
                    idPerson = _context.People.Max(a => a.IdPerson) + 1;

                    _context.People.Add(new Person()
                    {
                        IdPerson = idPerson,
                        Surname = orderCard.Person.Surname,
                        FirstName = orderCard.Person.FirstName,
                        Patronymic = orderCard.Person.Patronymic ?? null
                    });
                }

                foreach (var oldItem in existingOrder.OrderItems)
                {
                    var productFromDb = _productService.GetProductByIdFromDb(oldItem.IdProduct);
                    if (productFromDb != null)
                    {
                        productFromDb.CountInStock += oldItem.Count;
                    }
                }

                _context.OrderItems.RemoveRange(existingOrder.OrderItems);

                Product product;

                foreach (var items in orderCard.OrderItems)
                {
                    _context.OrderItems.Add(new OrderItem()
                    {
                        IdOrder = orderCard.IdOrder,
                        IdProduct = items.IdProduct,
                        IdOrderItem = ++idOrderItems,
                        Count = items.Count,
                    });

                    product = _productService.GetProductByIdFromDb(items.IdProduct)!;
                    product.CountInStock -= items.Count;
                }

                existingOrder.OrderDate = DateOnly.FromDateTime(orderCard.OrderDate);
                existingOrder.DeliveryDate = orderCard.DeliveryDate.HasValue
                    ? DateOnly.FromDateTime(orderCard.DeliveryDate.Value)
                    : null;
                existingOrder.IdAdress = idAdress;
                existingOrder.IdPerson = idPerson;
                existingOrder.Kod = orderCard.Kod;
                existingOrder.OrderStatus = orderCard.OrderStatus;

                _context.SaveChanges();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
