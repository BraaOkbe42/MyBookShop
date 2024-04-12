using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Book
    {
        [Key]
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public List<CartItems> CartItems { get; set; }

        public string ImgUrl { get; set; }

        public string description { get; set; }

    }
}
