using FoodOrder_API.Models.Dto;

namespace FoodOrder_API.Data
{
    public static class ItemStore
    {
        public static List<ItemDTO> itemList = new List<ItemDTO>
        {
            new ItemDTO { Id = 1, Name = "item1", Description = "desc1", Price = 50.25},
            new ItemDTO { Id = 2, Name = "item2", Description = "desc2", Price = 70.25},
        };

    }
}
