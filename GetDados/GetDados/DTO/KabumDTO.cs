namespace GetDados.DTO;

public class KabumDTO
{
    public int id { get; set; }
    public string title { get; set; } = string.Empty;
    public decimal price { get; set; }
    public decimal price_with_discount { get; set; }
}
