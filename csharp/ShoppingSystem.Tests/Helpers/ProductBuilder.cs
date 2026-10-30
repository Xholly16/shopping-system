using ShoppingSystem.Core;

namespace ShoppingSystem.Tests.Helpers;

public class ProductBuilder
{
    private int _id = 1;
    private string _name = "Mouse";
    private decimal _price = 250m;
    private int _stock = 5;

    public ProductBuilder WithId(int id) { _id = id; return this; }
    public ProductBuilder WithName(string name) { _name = name; return this; }
    public ProductBuilder WithPrice(decimal price) { _price = price; return this; }
    public ProductBuilder WithStock(int stock) { _stock = stock; return this; }

    public Product Build() => new Product(_id, _name, _price, _stock);
}
