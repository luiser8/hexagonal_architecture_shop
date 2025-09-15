using HexagonalShop.Application.DTOs;
using HexagonalShop.Domain.Entities;
using HexagonalShop.Domain.Interfaces;

namespace HexagonalShop.Application.UseCases;

public class OrderHandler
{
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;
    private readonly IInvoiceService _invoiceService;

    public OrderHandler(IOrderService orderService, IProductService productService, IInvoiceService invoiceService)
    {
        _orderService = orderService;
        _productService = productService;
        _invoiceService = invoiceService;
    }

    public async Task<Order?>? ExecuteById(int order) => await _orderService.GetById(order);
    public async Task<List<Order>>? ExecuteAll() => await _orderService.GetAll();
    public async Task<List<Order>>? ExecuteByUser(int idUser) => await _orderService.GetByUser(idUser);
    public async Task ExecuteSave(OrderDto order)
    {
        var idempotencyKey = Guid.NewGuid().ToString();
        order.IdempotencyKey = idempotencyKey;

        foreach (var orderDetail in order.OrderDetails)
        {
            var product = await _productService.GetById(orderDetail.IdProduct);
            if (product == null)
                throw new Exception($"Product {orderDetail.IdProduct} not found");

            if (product.Stock < orderDetail.Quantity)
                throw new Exception($"Insufficient stock for product {product.Id}");

            product.Stock -= orderDetail.Quantity;
            await _productService.Update(orderDetail.IdProduct, product);
        }

        var orderWithIdempotency = await _orderService.GetByIdempotencyKey(idempotencyKey);
        if (orderWithIdempotency == null)
        {
            var newOrder = new Order
            {
                IdUser = order.IdUser,
                IdempotencyKey = idempotencyKey,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetails
                {
                    IdProduct = od.IdProduct,
                    Quantity = od.Quantity
                }).ToList()
            };

            var orderCreated = await _orderService.Create(newOrder);

            decimal subTotal = 0;
            foreach (var od in order.OrderDetails)
            {
                var product = await _productService.GetById(od.IdProduct);
                if (product != null) subTotal += product.Price * od.Quantity;
            }

            var iva = subTotal * 0.21m;
            var total = subTotal + iva;

            var invoice = new Invoice
            {
                IdOrder = orderCreated,
                SubTotal = subTotal,
                Iva = iva,
                Total = total
            };

            await _invoiceService.Create(invoice);
        }
    }

    public async Task ExecuteDelete(int id) => await _orderService.Delete(id);
}