using BookShop.Data;
using BookShop.Models;
using BookShop.ViewModels;
using BookStore.Data;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly BookStoreContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;


        public ShoppingCartController(BookStoreContext _context, Microsoft.AspNetCore.Identity.UserManager<User> userManager)
        {
            this._context = _context;
            this._userManager = userManager;
        }
        public IActionResult ShoppingCart()
        {
            var cartModel = new TheCart();
            return View(cartModel);
        }



        public async Task<IActionResult> Cart()
        {
            var user = await _userManager.GetUserAsync(User);
            

            TheCart cart;
            cart = CartManger.GetCart(HttpContext);
            foreach(var item in cart.CartItems) {
                var prod = _context.Books.Find(item.BookID);
                if (prod != null)
                {
                    item.Book = prod;
                }
            }
            return View("Cart",cart);
        }


        [HttpPost]
        public IActionResult Remove(int bookId)
        {
            var cart = CartManger.GetCart(HttpContext);
            CartManger.RemoveFromCart(cart, bookId);
            return RedirectToAction("Cart", "ShoppingCart");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCartItemQuantity(int productId, int currentQuantity, string change)
        {

            var mainCart = CartManger.GetCart(HttpContext);

            var specificProduct = await _context.Books.FirstOrDefaultAsync(p => p.BookID == productId);
            if (specificProduct != null)
            {
                int newQuantity = currentQuantity + (change == "increase" ? 1 : -1);
                newQuantity = Math.Max(newQuantity, 1);
                newQuantity = Math.Min(newQuantity, specificProduct.StockQuantity);

                CartManger.UpdateCartItemQuantity(mainCart, productId, newQuantity);
            }

            return RedirectToAction("Cart", "ShoppingCart");
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentVM model)
        {
            if (!ModelState.IsValid)
            {
       
                return View("Cart", model); 
            }

     
            var encryptedCardNumber = model.CardNumber; 
            var paymentRecord = new CreditCard
            {
                  CardNumber = encryptedCardNumber,
                  date = model.ExpiryDate 
            };
            _context.CreditCards.Add(paymentRecord);
            await _context.SaveChangesAsync();

            return View();
        }

        public IActionResult Payment()
        {
            var cart = CartManger.GetCart(HttpContext);
            double total = 0;
            if (cart != null)
            {
                foreach (var item in cart.CartItems)
                {
                    if(item.Book.StockQuantity != 0)
                        total += (double)(item.Book.Price * item.Quantity);
                }
            }
            var paymentViewModel = new PaymentVM() { Total = total };
            return View(paymentViewModel);
        }

    }
}
