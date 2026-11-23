using System;
using System.Collections.Generic;

namespace demo2.Models;

public partial class Adress
{
    public int IdAdress { get; set; }

    public int Index { get; set; }

    public int IdSity { get; set; }

    public string Street { get; set; } = null!;

    public int? House { get; set; }

    public virtual Sity IdSityNavigation { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
