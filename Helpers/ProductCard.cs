using System.Windows;

namespace demo2.Helpers
{
    public class ProductCard
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Artikul { get; set; } = string.Empty;
        public CategotyInfo Category { get; set; } = new();
        public string? Image { get; set; }
        public string? Description { get; set; }
        public ManufacturerInfo Manufacturer{ get; set; } = new();
        public SupplierInfo Supplier { get; set; } = new();
        public decimal? OldPrice { get; set; }
        public decimal? Price {  get; set; }
        public string UnitOfMeasurement { get; set; } = null!;
        public int CountInStock { get; set; }
        public decimal? CurrentDiscount { get; set; }
        public string BackgroundColor { get; set; } = string.Empty;
        public Visibility OldPriceVisibility { get; set; }
    }

    public class CategotyInfo
    {
        public int Id { get; set; }
        public string Name {  set; get; } = string.Empty;
    }

    public class ManufacturerInfo
    {
        public int Id { get; set; }
        public string Name { set; get; } = string.Empty;
    }

    public class SupplierInfo
    {
        public int Id { get; set; }
        public string Name { set; get; } = string.Empty;
    }
}
