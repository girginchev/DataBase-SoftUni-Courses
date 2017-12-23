using FastFood.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FastFood.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string Customer { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public OrderType Type { get; set; } /*= OrderType.ForHere;*/

        [Required]
        public decimal TotalPrice { get; set; } /*– decimal value(calculated property, (not mapped to database), required)*/

        [Required]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();

    }
}