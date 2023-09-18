using System;
using System.Collections.Generic;

namespace server.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserEmail { get; set; } = null!;

    public string UserUsername { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
