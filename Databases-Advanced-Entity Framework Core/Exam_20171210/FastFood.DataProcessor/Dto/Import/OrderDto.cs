using FastFood.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace FastFood.DataProcessor.Dto.Import
{
    [XmlType("Order")]
    public class OrderDto
    {
        [Required]
        public string Customer { get; set; }

        [Required]
        public string DateTime { get; set; }

        [Required]
        public OrderType Type { get; set; } = OrderType.ForHere;


        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Employee { get; set; }

        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
