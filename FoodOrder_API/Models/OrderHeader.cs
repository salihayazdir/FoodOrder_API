using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodOrder_API.Models
{
    public class OrderHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Notes { get; set; }

        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Address")]
        [Required]
        public int AddressId { get; set; }
        public Address Address { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public int Status { get; set; }

        // 0 - İptal
        // 1 - Yeni Sipariş
        // 2 - Onaylandı
        // 3 - Yolda
        // 4 - Teslim Edildi


    }
}
