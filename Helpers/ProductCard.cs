using System.Windows;

namespace demo2.Helpers
{
    public class ProductCard
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string? Description { get; set; }
        public string ManufacturerName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public decimal? OldPrice { get; set; }
        public decimal? Price {  get; set; }
        public string UnitOfMeasurement { get; set; } = null!;
        public int CountInStock { get; set; }
        public decimal? CurrentDiscount { get; set; }
        public string BackgroundColor { get; set; } = string.Empty;
        public Visibility OldPriceVisibility { get; set; }
    }
}
