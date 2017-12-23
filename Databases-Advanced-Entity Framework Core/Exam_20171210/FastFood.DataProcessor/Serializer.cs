using System;
using System.IO;
using FastFood.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using FastFood.Models.Enums;
using FastFood.DataProcessor.Dto.Export;
using FastFood.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace FastFood.DataProcessor
{
	public class Serializer
	{
		public static string ExportOrdersByEmployee(FastFoodDbContext context, string employeeName, string orderType)
		{
            var employees = context.Employees.Include(e => e.Orders).ThenInclude(e => e.OrderItems).ThenInclude(e => e.Item);

            var employee = employees.FirstOrDefault(e => e.Name == employeeName);
            var orders = employee.Orders.Where(e => e.Type == Enum.Parse<OrderType>(orderType)).ToList();

            var validOrderds = new List<OrderDto>();

            foreach (var order in orders)
            {
                decimal totalPrice = 0;
                var validItems = new List<ItemDto>();
                foreach (var item in order.OrderItems)
                {
                    var name = item.Item.Name;
                    var price = item.Item.Price;
                    var quantity = item.Quantity;
                    var itemDto = new ItemDto()
                    {
                        Name = name,
                        Price = price,
                        Quantity = quantity
                    };
                    totalPrice += item.Item.Price * item.Quantity;
                    validItems.Add(itemDto);
                }


                var orderDto = new OrderDto()
                {
                    Customer = order.Customer,
                    TotalPrice = totalPrice,
                    Items = validItems
                };
                validOrderds.Add(orderDto);
            }

            var result = new EmployeeOrderDto()
            {
                Name = employee.Name,
                Orders = validOrderds.OrderByDescending(e=>e.TotalPrice).ThenByDescending(e => e.Items.Count).ToList(),
                TotalMade = validOrderds.Sum(e => e.TotalPrice)
            };

            //var orderedResult = result.Orders.OrderByDescending(e => e.TotalPrice).ThenByDescending(e => e.Items.Count).ToList();

            var json = JsonConvert.SerializeObject(result,Formatting.Indented);
            return json;
		}

		public static string ExportCategoryStatistics(FastFoodDbContext context, string categoriesString)
		{
            var categoriesList = categoriesString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var categories = context.Categories.Where(c => categoriesList.Contains(c.Name))
                .Select(c => new
                {
                    Name = c.Name,
                    MostPopularItem = c.Items.Select(i => new
                    {
                        Name = i.Name,
                        TotalMade = i.OrderItems.Sum(x => x.Quantity * x.Item.Price),
                        TimesSold = i.OrderItems.Sum(x => x.Quantity)
                    }).OrderByDescending(x => x.TimesSold).First()
                }).OrderByDescending(r => r.MostPopularItem.TotalMade).ThenByDescending(r => r.MostPopularItem.TimesSold);

            var xdoc = new XDocument(new XElement("Categories"));
            foreach (var c in categories)
            {
                xdoc.Root.Add(new XElement("Category",
                    new XElement("Name", c.Name), new XElement("MostPopularItem",
                    new XElement("Name", c.MostPopularItem.Name),
                    new XElement("TotalMade", c.MostPopularItem.TotalMade),
                    new XElement("TimesSold", c.MostPopularItem.TimesSold)));
            }
            return xdoc.ToString();
		}
	}
}