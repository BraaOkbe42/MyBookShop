using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }

        [ForeignKey("User")]
        public string UserID { get; set; } 
        public User User { get; set; }

        public List<CartItems> CartItems { get; set; }
    }

}
