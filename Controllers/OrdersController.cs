using Microsoft.AspNetCore.Mvc;
using BookShop.Data;
using BookShop.Models;
using BookStore.Data;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly BookStoreContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;

        public OrdersController(BookStoreContext context, Microsoft.AspNetCore.Identity.UserManager<User> userManager)
        {
            this._context = context;  
            this._userManager = userManager;
        }

        public async Task<IActionResult> OrdersAsync()
        {
     

            var userId = _userManager.GetUserId(User);
            //if (string.IsNullOrEmpty(userId))
            //{
            //    return Challenge(); 
            //}

            var orders = await _context.Orders
                                       .Where(o => o.UserId == userId) 
                                       .Include(o => o.OrderBooks)
                                       .ThenInclude(ob => ob.Book)
                                       .ToListAsync();

            return View(orders); 
        }
    }
}
