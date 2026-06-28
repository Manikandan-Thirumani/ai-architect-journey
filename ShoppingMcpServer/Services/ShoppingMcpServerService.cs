using ShoppingMcpServer.Services;
using System.Text.Json;

namespace ShoppingMcpServer.MCP;

public class ShoppingMcpServerService
{
    private readonly ProductRepository
        _repository;

    private readonly CartService
        _cart;

    private readonly OrderService
        _orders;

    public ShoppingMcpServerService(
        ProductRepository repository,
        CartService cart,
        OrderService orders)
    {
        _repository = repository;
        _cart = cart;
        _orders = orders;
    }

    /*
     * MCP Tool Discovery
     */
    public List<McpToolDefinition>
        GetTools()
    {
        return
        [
            /*
             * search_products
             */
            new()
            {
                Name = "search_products",

                Description =
                    "Search products by keyword.",

                InputSchema = new
                {
                    type = "object",

                    properties = new
                    {
                        keyword = new
                        {
                            type = "string"
                        }
                    },

                    required =
                        new[]
                        {
                            "keyword"
                        }
                }
            },

            /*
             * get_product
             */
            new()
            {
                Name = "get_product",

                Description =
                    "Get product details by product id.",

                InputSchema = new
                {
                    type = "object",

                    properties = new
                    {
                        productId = new
                        {
                            type = "integer"
                        }
                    },

                    required =
                        new[]
                        {
                            "productId"
                        }
                }
            },

            /*
             * add_to_cart
             */
            new()
            {
                Name = "add_to_cart",

                Description =
                    "Adds a product to shopping cart.",

                InputSchema = new
                {
                    type = "object",

                    properties = new
                    {
                        productId = new
                        {
                            type = "integer"
                        },

                        quantity = new
                        {
                            type = "integer"
                        }
                    },

                    required =
                        new[]
                        {
                            "productId",
                            "quantity"
                        }
                }
            },

            /*
             * view_cart
             */
            new()
            {
                Name = "view_cart",

                Description =
                    "Returns current shopping cart.",

                InputSchema = new
                {
                    type = "object",

                    properties = new { }
                }
            },

            /*
             * remove_from_cart
             */
            new()
            {
                Name = "remove_from_cart",

                Description =
                    "Removes product from shopping cart.",

                InputSchema = new
                {
                    type = "object",

                    properties = new
                    {
                        productId = new
                        {
                            type = "integer"
                        }
                    },

                    required =
                        new[]
                        {
                            "productId"
                        }
                }
            },

            /*
             * place_order
             */
            new()
            {
                Name = "place_order",

                Description =
                    "Places an order.",

                InputSchema = new
                {
                    type = "object",

                    properties = new { }
                }
            },

            /*
             * get_orders
             */
            new()
            {
                Name = "get_orders",

                Description =
                    "Returns all orders.",

                InputSchema = new
                {
                    type = "object",

                    properties = new { }
                }
            },

            /*
             * get_order
             */
            new()
            {
                Name = "get_order",

                Description =
                    "Returns one order.",

                InputSchema = new
                {
                    type = "object",

                    properties = new
                    {
                        orderId = new
                        {
                            type = "integer"
                        }
                    },

                    required =
                        new[]
                        {
                            "orderId"
                        }
                }
            },

            /*
             * cancel_order
             */
            new()
            {
                Name = "cancel_order",

                Description =
                    "Cancels an order.",

                InputSchema = new
                {
                    type = "object",

                    properties = new
                    {
                        orderId = new
                        {
                            type = "integer"
                        }
                    },

                    required =
                        new[]
                        {
                            "orderId"
                        }
                }
            }
        ];
    }

    /*
     * MCP Tool Execution
     */
    public async Task<object>
        CallToolAsync(
            string toolName,
            Dictionary<string, object>
                arguments)
    {
        switch (toolName)
        {
            /*
             * search_products
             */
            case "search_products":
                {
                    var keyword =
                        arguments["keyword"]
                            ?.ToString();

                    var products =
                        _repository
                            .Search(
                                keyword!);

                    return await Task
                        .FromResult(
                            products);
                }

            /*
             * get_product
             */
            case "get_product":
                {
                    int productId =
                        arguments["productId"]
                            switch
                        {
                            JsonElement j =>
                            j.GetInt32(),

                            _ =>
                            Convert.ToInt32(
                                arguments["productId"])
                        };

                    var product =
                        _repository
                            .GetById(
                                productId);

                    if (product == null)
                    {
                        throw new Exception(
                            $"Product {productId} not found");
                    }

                    return await Task
                        .FromResult(
                            product);
                }

            /*
             * add_to_cart
             */
            case "add_to_cart":
                {
                    int productId =
                        arguments["productId"]
                            switch
                        {
                            JsonElement j =>
                            j.GetInt32(),

                            _ =>
                            Convert.ToInt32(
                                arguments["productId"])
                        };

                    int quantity =
                        arguments["quantity"]
                            switch
                        {
                            JsonElement j =>
                            j.GetInt32(),

                            _ =>
                            Convert.ToInt32(
                                arguments["quantity"])
                        };

                    var product =
                        _repository
                            .GetById(
                                productId);

                    if (product == null)
                    {
                        throw new Exception(
                            "Product not found");
                    }

                    _cart.Add(
                        product,
                        quantity);

                    return await Task
                        .FromResult(
                            new
                            {
                                Message =
                                    "Added to cart",

                                Cart =
                                    _cart.GetItems(),

                                Total =
                                    _cart.GetTotal()
                            });
                }

            /*
             * view_cart
             */
            case "view_cart":
                {
                    return await Task
                        .FromResult(
                            new
                            {
                                Cart =
                                    _cart.GetItems(),

                                Total =
                                    _cart.GetTotal()
                            });
                }

            /*
             * remove_from_cart
             */
            case "remove_from_cart":
                {
                    int productId =
                        arguments["productId"]
                            switch
                        {
                            JsonElement j =>
                            j.GetInt32(),

                            _ =>
                            Convert.ToInt32(
                                arguments["productId"])
                        };

                    _cart.Remove(
                        productId);

                    return await Task
                        .FromResult(
                            new
                            {
                                Message =
                                    "Removed from cart",

                                Cart =
                                    _cart.GetItems(),

                                Total =
                                    _cart.GetTotal()
                            });
                }

            /*
             * place_order
             */
            case "place_order":
                {
                    var items =
                        _cart.GetItems();

                    if (!items.Any())
                    {
                        throw new Exception(
                            "Cart is empty");
                    }

                    var total =
                        _cart.GetTotal();

                    var order =
                        _orders.PlaceOrder(
                            items,
                            total);

                    _cart.Clear();

                    return await Task
                        .FromResult(
                            new
                            {
                                Message =
                                    "Order placed successfully",

                                Order =
                                    order
                            });
                }

            /*
             * get_orders
             */
            case "get_orders":
                {
                    return await Task
                        .FromResult(
                            _orders.GetOrders());
                }

            /*
             * get_order
             */
            case "get_order":
                {
                    int orderId =
                        arguments["orderId"]
                            switch
                        {
                            JsonElement j =>
                            j.GetInt32(),

                            _ =>
                            Convert.ToInt32(
                                arguments["orderId"])
                        };

                    var order =
                        _orders.GetOrder(
                            orderId);

                    if (order == null)
                    {
                        throw new Exception(
                            $"Order {orderId} not found");
                    }

                    return await Task
                        .FromResult(
                            order);
                }

            /*
             * cancel_order
             */
            case "cancel_order":
                {
                    int orderId =
                        arguments["orderId"]
                            switch
                        {
                            JsonElement j =>
                            j.GetInt32(),

                            _ =>
                            Convert.ToInt32(
                                arguments["orderId"])
                        };

                    var success =
                        _orders.CancelOrder(
                            orderId);

                    return await Task
                        .FromResult(
                            new
                            {
                                Success =
                                    success,

                                Message =
                                    success
                                        ? "Order cancelled"
                                        : "Order not found"
                            });
                }

            default:
                {
                    throw new Exception(
                        $"Unknown tool: {toolName}");
                }
        }
    }
}