namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    // Login <username> <password>
    public class LoginCommand
    {
        public static string Execute(string[] data)
        {
            var userName = data[1];
            var passwod = data[2];

            using (var db = new PhotoShareContext())
            {
                if (data.Length < 2)
                {
                    throw new InvalidOperationException($"Command {data[0]} not valid!");
                }

                if (Session.User !=null)
                {
                    throw new ArgumentException("You should logout first!");
                }

                var user = db.Users.FirstOrDefault(e => e.Username == userName && e.Password == passwod);


                if (user == null)
                {
                    throw new ArgumentException("Invalid username or password!");
                }

                Session.User = user;

                return $"User {userName} successfully logged in!";
            }
        }
    }
}
