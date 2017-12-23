using System;
using FastFood.Data;
using System.Linq;

namespace FastFood.DataProcessor
{
    public static class Bonus
    {
	    public static string UpdatePrice(FastFoodDbContext context, string itemName, decimal newPrice)
	    {
            string result;
            var item = context.Items.FirstOrDefault(e => e.Name == itemName);
            var oldPrice = 0m;

            if (item == null)
            {
                result = $"Item {itemName} not found!";
            }
            else
            {
                oldPrice = item.Price;
                item.Price = newPrice;
                context.SaveChanges();
                result = $"{itemName} Price updated from ${oldPrice:F2} to ${newPrice:F2}";
            }
            return result;
	    }
    }
}
