namespace PhotoShare.Client.Core.Commands
{
    using Microsoft.EntityFrameworkCore;
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class AcceptFriendCommand
    {
        // AcceptFriend <username1> <username2>
        public static string Execute(string[] data)
        {
            var acceptingUserName = data[1];
            var requestingUserName = data[2];

            if (data.Length < 3)
            {
                throw new IndexOutOfRangeException($"Command {data[0]} not valid!");
            }

            using (var db = new PhotoShareContext())
            {
                var acceptingUser = db.Users.Include(e => e.FriendsAdded).ThenInclude(fa => fa.Friend).FirstOrDefault(e => e.Username == acceptingUserName);

                if (Session.User.Username != acceptingUserName)
                {
                    throw new ArgumentException("Invalid credentials!");
                }

                if (acceptingUser == null)
                {
                    throw new ArgumentException($"{acceptingUserName} not found!");
                }

                var requestingUser = db.Users.Include(e => e.FriendsAdded).ThenInclude(fa => fa.Friend).FirstOrDefault(e => e.Username == requestingUserName);

                if (requestingUser == null)
                {
                    throw new ArgumentException($"{requestingUserName} not found!");
                }

                bool alreadyAccepted = acceptingUser.FriendsAdded.Any(u => u.Friend == requestingUser);

                bool Added = requestingUser.FriendsAdded.Any(u => u.Friend == acceptingUser);

                if (!Added)
                {
                    throw new InvalidOperationException($"{requestingUserName} has not added {acceptingUserName} as a friend");
                }

                if (alreadyAccepted && Added)
                {
                    throw new InvalidOperationException($"{requestingUserName} is already a friend to {acceptingUserName}");
                }

                acceptingUser.FriendsAdded.Add(new Friendship
                {
                    User = acceptingUser,
                    Friend = requestingUser
                });

                db.SaveChanges();

                return $"{acceptingUserName} accepted {requestingUserName} as a friend";
            }
        }
    }
}

