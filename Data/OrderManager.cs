

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using BookShop.Models;
using BookStore.Data;

namespace BookShop.Data
{
    public static class OrderManager
    {
        private static readonly ConcurrentDictionary<string, Order> Orders = new ConcurrentDictionary<string, Order>();


        public static async Task<Order> CreateOrderAsync(HttpContext httpContext, BookStoreContext context, User user)
        {
            TheCart cart = CartManger.GetCart(httpContext);
            var order = new Order
            {
                OrderDate = DateTime.Now,
                User = user,
                TotalPrice = 0,
                OrderBooks = new List<OrderBooks>(),
                
            };

            foreach (var item in cart.CartItems)
            {
                var book = await context.Books.FirstOrDefaultAsync(b => b.BookID == item.Book.BookID);
                if (book != null)
                {
                    var orderBook = new OrderBooks
                    {
                        BookID = item.Book.BookID,
                        UnitPrice = (float)book.Price,
                        Book = item.Book,
                        Quantity = item.Quantity,
                        OrderID = order.OrderID
                    };
                    order.TotalPrice += (float)book.Price * item.Quantity;
                    order.OrderBooks.Add(orderBook);
                }
            }

            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            Orders.TryAdd(order.OrderID.ToString(), order);

            return order;
        }
        public static Order GetOrderById(string orderId)
        {
            Orders.TryGetValue(orderId, out var order);
            return order;
        }
    }
}
