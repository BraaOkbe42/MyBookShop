using BookShop.Data;
using BookShop.Models;
using BookShop.ViewModels;
using BookStore.Data;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Controllers
{
    public class PaymentController : Controller
    {

        private  readonly BookStoreContext _context ;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;

        public PaymentController(BookStoreContext context, Microsoft.AspNetCore.Identity.UserManager<User> _userManager)
        {
            this._userManager = _userManager;
            this._context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PaymentSuccess()
        {
            return View();

        }


        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentVM model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                var encryptedCardNumber = EncryptionService.EncryptString(model.CardNumber);
                if (user == null)
                {
                    var guest = new Guest();
                    _context.Guests.Add(guest);
                    user = new User
                    {
                        AccessFailedCount = 0,
                        UserName = "Guest",
                        Id = _context.Guests.ToList().Count.ToString()
                    };
                }
                var creditCard = new CreditCard
                {
                    User = user, 
                    CardNumber = encryptedCardNumber,
                    date = model.ExpiryDate,
                    cvv = EncryptionService.EncryptString(model.CVV) 
                };

                _context.CreditCards.Add(creditCard);

                var cart = CartManger.GetCart(HttpContext);
                List<CartItems> itemsToRemove = new List<CartItems>();

                foreach (var item in cart.CartItems)
                {
                    if (item.Book.StockQuantity > 0)
                    {
                        item.Book.StockQuantity -= item.Quantity;
                        _context.Books.Attach(item.Book);
                        _context.Entry(item.Book).State = EntityState.Modified;
                        itemsToRemove.Add(item); 
                    }
                }
                var order = await OrderManager.CreateOrderAsync(HttpContext, _context , user);

                foreach (var item in itemsToRemove)
                {
                    CartManger.RemoveFromCart(cart, item.BookID);
                }


                await _context.SaveChangesAsync();

                return RedirectToAction("PaymentSuccess");
            }
            return RedirectToAction("Payment", "ShoppingCart");
        }

    }
}


