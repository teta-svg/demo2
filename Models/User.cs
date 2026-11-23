using System;
using System.Collections.Generic;

namespace demo2.Models;

public partial class User
{
    public int IdUser { get; set; }

    public int IdUserRole { get; set; }

    public int IdPerson { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual Person IdPersonNavigation { get; set; } = null!;

    public virtual UserRole IdUserRoleNavigation { get; set; } = null!;
}
