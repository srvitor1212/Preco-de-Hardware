namespace Domain.Models;

public class Product
    (string name, decimal price)
    : BaseModel
{
    public string Name { get; private set; } = name;
    public decimal Price { get; private set; } = price;
}
