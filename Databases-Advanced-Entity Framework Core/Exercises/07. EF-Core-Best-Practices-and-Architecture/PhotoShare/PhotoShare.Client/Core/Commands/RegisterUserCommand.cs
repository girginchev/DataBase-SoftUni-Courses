namespace PhotoShare.Client.Core.Commands
{
    using System;

    using Models;
    using Data;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public class RegisterUserCommand
    {
        // RegisterUser <username> <password> <repeat-password> <email>
        public static string Execute(string[] data)
        {
            string username = data[1];
            string password = data[2];
            string repeatPassword = data[3];
            string email = data[4];

            if (data.Length < 5)
            {
                throw new IndexOutOfRangeException($"Command {data[0]} not valid!");
            }

            if (Session.User != null)
            {
                throw new InvalidOperationException("Invalid Credentials!");
            }

            if (password != repeatPassword)
            {
                throw new ArgumentException("Passwords do not match!");
            }

            using (var db = new PhotoShareContext())
            {
                //var checkUser = db.Users
                //    .AsNoTracking()
                //    .Where(e => e.Username == username)
                //    .FirstOrDefault();
                //if (checkUser != null)
                //{
                //    throw new InvalidOperationException($"Username {username} is already taken!");
                //}

                if (db.Users.Any(e=>e.Username == username))
                {
                    throw new InvalidOperationException($"Username {username} is already taken!");
                }
            }

            User user = new User
            {
                Username = username,
                Password = password,
                Email = email,
                IsDeleted = false,
                RegisteredOn = DateTime.Now,
                LastTimeLoggedIn = DateTime.Now
            };

            using (PhotoShareContext context = new PhotoShareContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            return "User " + user.Username + " was registered successfully!";
        }
    }
}
