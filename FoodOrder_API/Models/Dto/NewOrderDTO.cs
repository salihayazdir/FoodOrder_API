using Microsoft.OpenApi.Any;

namespace FoodOrder_API.Models.Dto
{
    public class OrderItemRequestDTO
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }

    }
    public class NewOrderDTO
    {
        public string Notes { get; set; }
        public int AddressId { get; set; }
        public OrderItemRequestDTO[] OrderItems { get; set; }
    }
}
