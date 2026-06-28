using ShoppingMcpServer.Models;

namespace ShoppingMcpServer.Services;

public class ProductRepository
{
    private readonly List<Product>
        _products =
        [
            new()
            {
                ProductId = 1,
                Name = "iPhone 16",
                Price = 75000,
                Stock = 15,
                PrimeDelivery = true
            },

            new()
            {
                ProductId = 2,
                Name = "Samsung S25",
                Price = 68000,
                Stock = 8,
                PrimeDelivery = true
            },

            new()
            {
                ProductId = 3,
                Name = "MacBook Air M4",
                Price = 125000,
                Stock = 4,
                PrimeDelivery = false
            }
        ];

    public List<Product>
        Search(string keyword)
    {
        return _products
            .Where(x =>
                x.Name.Contains(
                    keyword,
                    StringComparison
                        .OrdinalIgnoreCase))
            .ToList();
    }

    public Product? GetById(
        int id)
    {
        return _products
            .FirstOrDefault(
                x => x.ProductId == id);
    }
}