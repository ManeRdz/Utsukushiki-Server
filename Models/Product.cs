using System;
using System.Collections.Generic;

namespace server.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string ProductDescription { get; set; } = null!;

    public string ProductCategory { get; set; } = null!;

    public int ProductPrice { get; set; }

    public int ProductStock { get; set; }

    public string ProductDir { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
