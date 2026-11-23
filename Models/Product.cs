using System;
using System.Collections.Generic;

namespace demo2.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public string Artikul { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string UnitOfMeasurement { get; set; } = null!;

    public decimal Price { get; set; }

    public int IdSupplier { get; set; }

    public int IdManufacturer { get; set; }

    public int IdCategory { get; set; }

    public decimal? CurrentDiscount { get; set; }

    public int CountInStock { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public virtual Caterory IdCategoryNavigation { get; set; } = null!;

    public virtual Manufacturer IdManufacturerNavigation { get; set; } = null!;

    public virtual Supplier IdSupplierNavigation { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
