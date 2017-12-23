using System;
using System.Collections.Generic;
using System.Text;

namespace FastFood.DataProcessor.Dto.Export
{
    public class EmployeeOrderDto
    {
        public string Name { get; set; }

        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();

        public decimal TotalMade { get; set; }
    }
}
