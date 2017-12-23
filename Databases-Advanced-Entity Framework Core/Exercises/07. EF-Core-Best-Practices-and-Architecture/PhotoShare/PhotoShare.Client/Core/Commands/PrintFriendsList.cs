namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Data;
    using System;
    using System.Linq;
    using System.Text;

    public class PrintFriendsListCommand 
    {
        // PrintFriendsList <username>
        public static string Execute(string[] data)
        {
            var userName = data[1];

            if (data.Length < 2)
            {
                throw new IndexOutOfRangeException($"Command {data[0]} not valid!");
            }

            using (var db = new PhotoShareContext())
            {
                var user = db.Users.FirstOrDefault(e => e.Username == userName);

                if (user == null)
                {
                    throw new ArgumentException($"User {userName} not found!");
                }

                var userFriends = db.Friendships.Where(e => e.User == user).Select(e=>e.Friend).ToList();

                if (userFriends.Count == 0)
                {
                    throw new ArgumentException("No friends for this user. :(");
                }

                var result = new StringBuilder();
                result.AppendLine("Friends:");
                foreach (var uf in userFriends)
                {
                    result.AppendLine("-" + uf.Username);
                }

                return result.ToString();
            }
        }
    }
}
