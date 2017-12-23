namespace PhotoShare.Client.Core.Commands
{
    using Microsoft.EntityFrameworkCore;
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class AddFriendCommand
    {
        // AddFriend <username1> <username2>
        public static string Execute(string[] data)
        {
            var requestingUserName = data[1];
            var addedUserName = data[2];

            if (data.Length < 3)
            {
                throw new IndexOutOfRangeException($"Command {data[0]} not valid!");
            }

            using (var db = new PhotoShareContext())
            {
                var requestingUser = db.Users.Include(e=>e.FriendsAdded).ThenInclude(fa=>fa.Friend).FirstOrDefault(e => e.Username == requestingUserName);

                if (Session.User.Username != requestingUserName)
                {
                    throw new ArgumentException("Invalid credentials!");
                }

                if (requestingUser == null)
                {
                    throw new ArgumentException($"{requestingUserName} not found!");
                }

                var addedUser = db.Users.Include(e=>e.FriendsAdded).ThenInclude(fa=>fa.Friend).FirstOrDefault(e => e.Username == addedUserName);

                if (addedUser == null)
                {
                    throw new ArgumentException($"{addedUserName} not found!");
                }

                bool alreadyAdded = requestingUser.FriendsAdded.Any(u => u.Friend == addedUser);

                bool accepted = addedUser.FriendsAdded.Any(u => u.Friend == requestingUser);

                if (alreadyAdded && !accepted)
                {
                    throw new InvalidOperationException("Friend request already sent!");
                }
                if (!alreadyAdded && accepted)
                {
                    throw new InvalidCastException($"{requestingUserName} has already received a friend request from {addedUserName}");
                }
                if (alreadyAdded && accepted)
                {
                    throw new InvalidOperationException($"{addedUserName} is already a friend to {requestingUserName}");
                }

                requestingUser.FriendsAdded.Add(new Friendship
                {
                    User = requestingUser,
                    Friend = addedUser
                });

                db.SaveChanges();

                return $"Friend {addedUserName} added to {requestingUserName}";
            }
        }
    }
}
