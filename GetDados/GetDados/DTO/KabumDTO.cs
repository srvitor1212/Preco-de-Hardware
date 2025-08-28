namespace GetDados.DTO;

public class KabumDTO
{
    public string Name { get; set; } = string.Empty;
    public string FriendlyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ManufacturerName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal PrimePrice { get; set; }
    public decimal PrimePriceWithDiscount { get; set; }
    public decimal OldPrice { get; set; }
    public decimal OldPrimePrice { get; set; }
    public decimal PriceWithDiscount { get; set; }
    public decimal PriceMarketplace { get; set; }
    public bool Available { get; set; }

}