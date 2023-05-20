using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodOrder_API.Models
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("OrderHeader")]
        [Required]
        public int OrderHeaderId { get; set; }
        public OrderHeader OrderHeader { get; set; }

        [ForeignKey("Item")]
        [Required]
        public int ItemId { get; set; }
        public Item Item { get; set; }
        [Required]
        public int Quantity { get; set; }

    }
}
