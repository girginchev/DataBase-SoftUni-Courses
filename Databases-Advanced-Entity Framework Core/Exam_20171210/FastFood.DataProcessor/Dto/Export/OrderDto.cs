using System;
using System.Collections.Generic;
using System.Text;

namespace FastFood.DataProcessor.Dto.Export
{
    public class OrderDto
    {
        public string Customer { get; set; }

        public List<ItemDto> Items { get; set; } = new List<ItemDto>();

        public decimal TotalPrice { get; set; }
    }
}
