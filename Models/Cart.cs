using System;
using System.Collections.Generic;

namespace server.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int Cantidad { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
