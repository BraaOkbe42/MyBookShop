using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Models
{
    public class OrderBooks
    {
        [Key]
        public int OrderBookId { get; set; }

        public int OrderID { get; set; }
        public Order Order { get; set; }
        public int BookID { get; set; }
        public Book Book { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public float UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
