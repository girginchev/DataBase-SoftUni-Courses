namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;

    using Data;

    public class DeleteUser
    {
        // DeleteUser <username>
        public static string Execute(string[] data)
        {
            string username = data[1];

            if (data.Length < 2)
            {
                throw new IndexOutOfRangeException($"Command {data[0]} not valid!");
            }

            using (PhotoShareContext context = new PhotoShareContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == username);

                if (Session.User.Username != username)
                {
                    throw new ArgumentException("Invalid credentials!");
                }

                if (user == null)
                {
                    throw new InvalidOperationException($"User with {username} was not found!");
                }

                if (user.IsDeleted == true)
                {
                    throw new InvalidOperationException($"User {username} is already deleted!");
                }

                user.IsDeleted = true;

                context.SaveChanges();

                return $"User {username} was deleted from the database!";
            }
        }
    }
}
