﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodOrder_API.Models.Dto
{
    public class AddItemDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public double Price { get; set; }

        public int CategoryId { get; set; }
    }
}
