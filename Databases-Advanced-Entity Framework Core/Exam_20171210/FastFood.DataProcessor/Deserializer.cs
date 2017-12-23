using System;
using FastFood.Data;
using System.Text;
using Newtonsoft.Json;
using FastFood.DataProcessor.Dto.Import;
using System.Collections.Generic;
using FastFood.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using System.Globalization;
using FastFood.Models.Enums;
using System.Xml.Serialization;
using System.IO;

namespace FastFood.DataProcessor
{
	public static class Deserializer
	{
		private const string FailureMessage = "Invalid data format.";
		private const string SuccessMessage = "Record {0} successfully imported.";

        public static string ImportEmployees(FastFoodDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var deserializedEmployees = JsonConvert.DeserializeObject<EmployeeDto[]>(jsonString);

            var validEmployees = new List<Employee>();
            foreach (var empDto in deserializedEmployees)
            {
                if (String.IsNullOrWhiteSpace(empDto.Name) || String.IsNullOrWhiteSpace(empDto.Position) || empDto.Age < 15 || empDto.Age > 80)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                if (!IsValid(empDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }
                
                var position = context.Positions.FirstOrDefault(e => e.Name == empDto.Position);
                var employee = new Employee();
                employee.Name = empDto.Name;
                employee.Age = empDto.Age;

                if (position == null)
                {
                    var currentPosition = new Position()
                    {
                        Name = empDto.Position
                    };
                    context.Positions.Add(currentPosition);
                    context.SaveChanges();
                    employee.Position = currentPosition;
                }
                else
                {
                    employee.Position = position;
                }

                validEmployees.Add(employee);
                sb.AppendLine(string.Format(SuccessMessage, employee.Name));
            }
            context.Employees.AddRange(validEmployees);
            context.SaveChanges();


            return sb.ToString().Trim();
        }

		public static string ImportItems(FastFoodDbContext context, string jsonString)
		{
            var sb = new StringBuilder();
            var deserializedItems = JsonConvert.DeserializeObject<ItemDto[]>(jsonString);
            var validItems = new List<Item>();
            foreach (var itemDto in deserializedItems)
            {
                if (!IsValid(itemDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }
                var isItemExists = context.Items.AsNoTracking().Any(e => e.Name == itemDto.Name);
                if (isItemExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var item = new Item()
                {
                    Name = itemDto.Name,
                    Price = itemDto.Price
                };
                var category = context.Categories.FirstOrDefault(e => e.Name == itemDto.Category);
                if (category != null)
                {
                    item.Category = category;
                }
                else
                {
                    var currentCategory = new Category()
                    {
                        Name = itemDto.Category
                    };
                    context.Categories.Add(currentCategory);
                    context.SaveChanges();
                    item.Category = currentCategory;
                }
                validItems.Add(item);
                sb.AppendLine(string.Format(SuccessMessage, item.Name));
                context.Items.Add(item);
                context.SaveChanges();
            }
            //context.Items.AddRange(validItems);
            //context.SaveChanges();

            return sb.ToString().Trim();
		}

		public static string ImportOrders(FastFoodDbContext context, string xmlString)
		{
            var sb = new StringBuilder();
            //var xdoc = XDocument.Parse(xmlString);
            //var elements = xdoc.Root.Elements();
            //var validOrders = new List<Order>();
            //foreach (var element in elements)
            //{
            //    var customerName = element.Element("Customer")?.Value;
            //    var employeeName = element.Element("Employee")?.Value;
            //    var employee = context.Employees.FirstOrDefault(e => e.Name == employeeName);
            //    var date = DateTime.ParseExact(element.Element("DateTime")?.Value, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            //    var typeString = element.Element("Type")?.Value;
            //}

            var serializer = new XmlSerializer(typeof(OrderDto[]), new XmlRootAttribute("Orders"));
            var deserializedOrders = (OrderDto[])serializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(xmlString)));
            var validOrderItems = new List<OrderItem>();
            foreach (var orderDto in deserializedOrders)
            {
                if (!IsValid(orderDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;

                }
                var employee = context.Employees.FirstOrDefault(e => e.Name == orderDto.Employee);
                if (employee == null)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }
                var areItemsValid = context.Items.Any(e => orderDto.Items.Any(x => x.Name == e.Name) && orderDto.Items.Any(x=>x.Quantity > 0));
                if (!areItemsValid)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }


                var order = new Order()
                {
                    Customer = orderDto.Customer,
                    DateTime = DateTime.ParseExact(orderDto.DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    Type = orderDto.Type,
                    Employee = employee
                };



                foreach (var i in orderDto.Items)
                {
                    var validItem = context.Items.FirstOrDefault(e => e.Name == i.Name);
                    var OrderItem = new OrderItem()
                    {
                        Item = validItem,
                        Order = order,
                        Quantity = i.Quantity
                        
                    };
                    validOrderItems.Add(OrderItem);
                    
                }
                sb.AppendLine($"Order for {order.Customer} on {order.DateTime.ToString("dd/MM/yyyy HH:mm")} added");

            }
            context.OrderItems.AddRange(validOrderItems);
            context.SaveChanges();

            return sb.ToString().Trim();
		}

        public static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);
            return isValid;
        }
	}
}