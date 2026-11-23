using System;
using System.Collections.Generic;

namespace demo2.Models;

public partial class Sity
{
    public int IdSity { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Adress> Adresses { get; set; } = new List<Adress>();
}
