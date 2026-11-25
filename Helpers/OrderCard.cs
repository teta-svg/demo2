using System.Collections.ObjectModel;

namespace demo2.Helpers
{
    public class OrderCard
    {
        public int IdOrder { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Today;
        public DateTime? DeliveryDate { get; set; }
        public OrderAdress Adress { get; set; } = new();
        public OrderPerson Person { get; set; } = new();
        public int? Kod { get; set; }
        public string OrderStatus { get; set; } = null!;
        public decimal? TotalPrice { get; set; }
        public ObservableCollection<OrderItemCard> OrderItems { get; set; } = new();
    }

    public class OrderPerson
    {
        public string Surname { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? Patronymic { get; set; }
        public int Id { get; set; }
    }

    public class OrderItemCard
    {
        public int IdProduct { get; set; }
        public string Name { get; set; } = null!;
        public string Artikul { get; set; } = null!;
        public int Count { get; set; }
        public decimal? Price { get; set; }
        public int CountInStock { get; set; }
    }
    public class OrderAdress
    {
        public int IdAdress { get; set; }
        public int Index { get; set; }
        public OrderSity Sity { get; set; } = new();
        public string Street { get; set; } = null!;
        public int? House { get; set; }
    }
    public class OrderSity
    {
        public int IdSity { get; set; }
        public string Name { get; set; } = null!;
    }
}

