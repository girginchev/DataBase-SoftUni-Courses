namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Data;
    using System;
    using System.Linq;

    public class ModifyUserCommand
    {
        // ModifyUser <username> <property> <new value>
        // For example:
        // ModifyUser <username> Password <NewPassword>
        // ModifyUser <username> BornTown <newBornTownName>
        // ModifyUser <username> CurrentTown <newCurrentTownName>
        // !!! Cannot change username
        public static string Execute(string[] data)
        {
            var userName = data[1];
            var property = data[2].ToLower();
            var newValue = data[3];

            if (data.Length < 4)
            {
                throw new IndexOutOfRangeException($"Command {data[0]} not valid!");
            }

            using (var db = new PhotoShareContext())
            {
                var user = db.Users.Where(e => e.Username == userName).FirstOrDefault();

                if (Session.User.Username != userName)
                {
                    throw new ArgumentException("Invalid credentials!");
                }

                if (user == null)
                {
                    throw new ArgumentException($"User {userName} not found!");
                }

                string valueError = $"Value {newValue} not valid." + Environment.NewLine;

                switch (property)
                {
                    case "password":
                        if (!newValue.Any(e=>Char.IsLower(e)) || !newValue.Any(e=>Char.IsDigit(e)))
                        {
                            throw new ArgumentException($"{valueError}" + "Invalid Password");
                        }
                        user.Password = newValue;
                        break;

                    case "borntown":
                        var bornTown = db.Towns.Where(e => e.Name == newValue).FirstOrDefault();
                        if (bornTown == null)
                        {
                            throw new ArgumentException($"{valueError}" + $"Town {newValue} not found!");
                        }
                        user.BornTown = bornTown;
                        break;

                    case "currenttown":
                        var currentTown = db.Towns.Where(e => e.Name == newValue).FirstOrDefault();
                        if (currentTown == null)
                        {
                            throw new ArgumentException($"{valueError}" + $"Town {newValue} not found!");
                        }
                        user.CurrentTown = currentTown;
                        break;

                    default: throw new ArgumentException($"Property {property} not supported!");
                }
                db.SaveChanges();


            }
            return $"User {userName} {property} is {newValue}.";
        }
    }
}
