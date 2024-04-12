using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookShop.Models
{
    public class CreditCard
    {
        [Key]
        public int CiD { get; set; }

        public User User { get; set; }

        public string CardNumber { get; set; }

        public DateTime date { get; set; }

        public string cvv { get; set; }
    }
}
