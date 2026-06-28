using ShoppingMcpServer.Models;

namespace ShoppingMcpServer.Services;

public class CartService
{
    private readonly List<CartItem>
        _items = [];

    public void Add(
        Product product,
        int quantity)
    {
        var existing =
            _items.FirstOrDefault(
                x => x.ProductId ==
                     product.ProductId);

        if (existing != null)
        {
            existing.Quantity +=
                quantity;
        }
        else
        {
            _items.Add(
                new CartItem
                {
                    ProductId =
                        product.ProductId,

                    ProductName =
                        product.Name,

                    Price =
                        product.Price,

                    Quantity =
                        quantity
                });
        }
    }

    /*
     * NEW FOR DAY 5
     */
    public void Remove(
        int productId)
    {
        var item =
            _items.FirstOrDefault(
                x => x.ProductId ==
                     productId);

        if (item != null)
        {
            _items.Remove(
                item);
        }
    }

    public List<CartItem>
        GetItems()
    {
        return _items;
    }

    public decimal
        GetTotal()
    {
        return _items.Sum(
            x => x.Price *
                 x.Quantity);
    }

    public void Clear()
    {
        _items.Clear();
    }
}