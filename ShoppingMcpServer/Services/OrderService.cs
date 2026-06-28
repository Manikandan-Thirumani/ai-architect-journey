using ShoppingMcpServer.Models;

namespace ShoppingMcpServer.Services;

public class OrderService
{
    private readonly List<Order>
        _orders = [];

    private int _nextOrderId = 1;

    /*
     * Place order
     */
    public Order PlaceOrder(
        List<CartItem> items,
        decimal total)
    {
        var order =
            new Order
            {
                OrderId =
                    _nextOrderId++,

                OrderDate =
                    DateTime.UtcNow,

                Items =
                    items,

                Total =
                    total,

                Status =
                    "Placed"
            };

        _orders.Add(
            order);

        return order;
    }

    /*
     * Get all orders
     */
    public List<Order>
        GetOrders()
    {
        return _orders;
    }

    /*
     * Get single order
     */
    public Order?
        GetOrder(
            int orderId)
    {
        return _orders
            .FirstOrDefault(
                x =>
                    x.OrderId
                        == orderId);
    }

    /*
     * Cancel order
     */
    public bool
        CancelOrder(
            int orderId)
    {
        var order =
            _orders
                .FirstOrDefault(
                    x =>
                        x.OrderId
                            == orderId);

        if (order == null)
        {
            return false;
        }

        order.Status =
            "Cancelled";

        return true;
    }
}