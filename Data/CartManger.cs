using BookShop.Migrations;
using BookShop.Models;
using BookStore.Data;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace BookShop.Data
{
    public static  class CartManger
    {
        private static readonly ConcurrentDictionary<string, Models.TheCart> Carts = new ConcurrentDictionary<string, Models.TheCart>();
        private static readonly BookStoreContext _context = new BookStoreContext();

    
        public static Models.TheCart GetCart(HttpContext httpContext)
        {
            string cartId = GetCartId(httpContext);
            var cart = Carts.GetOrAdd(cartId, _ => new Models.TheCart());

            if (string.IsNullOrEmpty(httpContext.User.Identity.Name))
            {
                cart.SessionId = cartId; 
            }
            else
            {
                cart.UserID = httpContext.User.Identity.Name; 
            }
            return cart;
        }


        

        private static string GetCartId(HttpContext httpContext)
        {
            if (httpContext.Session.GetString("CartId") == null)
            {
                if (!string.IsNullOrEmpty(httpContext.User.Identity.Name))
                {
                    httpContext.Session.SetString("CartId", httpContext.User.Identity.Name);
                }
                else
                {
                    httpContext.Session.SetString("CartId", Guid.NewGuid().ToString());
                }
            }

            return httpContext.Session.GetString("CartId");
        }

        public static Models.TheCart GetCartByUserId(string userId)
        {
            Models.TheCart cart;
            if (!Carts.TryGetValue(userId, out cart))
            {
                cart = new Models.TheCart { UserID = userId };
                Carts.TryAdd(userId, cart);
            }
            return cart;
        }

        public static Models.TheCart GetCartBySessionId(string sessionId)
        {
            Models.TheCart cart;
            if (!Carts.TryGetValue(sessionId, out cart))
            {
                cart = new Models.TheCart { SessionId = sessionId };
                Carts.TryAdd(sessionId, cart);
            }
            return cart;
        }

        public static Models.TheCart GetCart(string customerId)
        {
            return Carts.GetOrAdd(customerId, new Models.TheCart());
        }


      
        public static async Task AddToCartAsync(Models.TheCart cart, int productId, int quantity, BookStoreContext _context)
        {
            var book =_context.Books.FirstOrDefault(b => b.BookID == productId);
            if (book.StockQuantity > 0)
            {
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.BookID == productId);
                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    cart.CartItems.Add(new Models.CartItems { BookID = productId, Quantity = quantity });
                }
            }
        }

    

        public static void RemoveFromCart(Models.TheCart cart, int productId)
        {
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.BookID == productId);

            if (cartItem != null)
            {
                cart.CartItems.Remove(cartItem);
            }
        }



        public static void UpdateCartItemQuantity(Models.TheCart cart, int productId, int newQuantity)
        {
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.BookID == productId);
            if (cartItem != null)
            {
                if (newQuantity > 0)
                {
                    cartItem.Quantity = newQuantity;
                }
                else
                {
                    cart.CartItems.Remove(cartItem);
                }
            }
            else if (newQuantity > 0)
            {
                cart.CartItems.Add(new Models.CartItems { BookID = productId, Quantity = newQuantity });
            }
        }

        public static Models.TheCart GetCartGuest(HttpContext httpContext, BookStoreContext context)
        {
            var session = httpContext.Session;
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();

            session.SetString("CartId", cartId);

            return new Models.TheCart() { UserID = cartId };
        }



    }
}
