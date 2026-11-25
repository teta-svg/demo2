namespace demo2.Models;

public partial class Order
{
    public int IdOrder { get; set; }

    public DateOnly OrderDate { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public int IdAdress { get; set; }

    public int IdPerson { get; set; }

    public int? Kod { get; set; }

    public string OrderStatus { get; set; } = null!;

    public virtual Adress IdAdressNavigation { get; set; } = null!;

    public virtual Person IdPersonNavigation { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
