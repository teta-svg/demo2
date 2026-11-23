using System;
using System.Collections.Generic;

namespace demo2.Models;

public partial class UserRole
{
    public int IdUserRole { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
