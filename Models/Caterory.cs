using System;
using System.Collections.Generic;

namespace demo2.Models;

public partial class Caterory
{
    public int IdCategory { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
