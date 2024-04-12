


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public float TotalPrice { get; set; }

        public DateTime OrderDate { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<OrderBooks> OrderBooks { get; set; } = new List<OrderBooks>();
    }
}
