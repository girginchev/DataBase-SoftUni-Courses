namespace PhotoShare.Client.Core.Commands
{
    using Models;
    using Data;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using System;

    public class AddTownCommand
    {
        // AddTown <townName> <countryName>
        public static string Execute(string[] data)
        {
            string townName = data[1];
            string country = data[2];

            if (data.Length < 3)
            {
                throw new IndexOutOfRangeException($"Command {data[0]} not valid!");
            }
            if (Session.User == null)
            {
                throw new ArgumentException("Invalid credentials!");
            }

            using (PhotoShareContext context = new PhotoShareContext())
            {
                //var checkTown = context.Towns
                //    .AsNoTracking()
                //    .Where(e => e.Name == townName)
                //    .FirstOrDefault();
                //if (checkTown != null)
                //{
                //    throw new ArgumentException($"Town {townName} was already added!");
                //}

                if (context.Towns.Any(e=>e.Name == townName))
                {
                    throw new ArgumentException($"Town {townName} was already added!");
                }

                Town town = new Town
                {
                    Name = townName,
                    Country = country
                };

                context.Towns.Add(town);
                context.SaveChanges();

                return "Town " + townName + " was added successfully!";
            }
        }
    }
}
