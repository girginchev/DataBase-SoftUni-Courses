namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Client.Utilities;
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CreateAlbumCommand
    {
        // CreateAlbum <username> <albumTitle> <BgColor> <tag1> <tag2>...<tagN>
        public static string Execute(string[] data)
        {
            string username = data[1];
            string albumTitle = data[2];
            string bgColor = data[3];
            string[] tags = data.Skip(4).Select(t => t.ValidateOrTransform()).ToArray();

            if (data.Length < 4)
            {
                throw new IndexOutOfRangeException($"Command {data[0]} not valid!");
            }

            using (var context = new PhotoShareContext())
            {
                var user = context.Users
                    .FirstOrDefault(u => u.Username == username);

                if (Session.User.Username != username)
                {
                    throw new ArgumentException("Invalid credentials!");
                }

                if (user == null)
                {
                    throw new ArgumentException($"User {username} not found!");
                }

                if (context.Albums.Any(a => a.Name == albumTitle))
                {
                    throw new ArgumentException($"Album {albumTitle} exists!");
                }

                Color color;
                if (!Enum.TryParse(bgColor, out color))
                {
                    throw new ArgumentException($"Color {bgColor} not found!");
                }

                if (!tags.All(t => context.Tags.Any(ct => ct.Name == t)))
                {
                    throw new ArgumentException($"Invalid tags!");
                }

                Album album = new Album()
                {
                    Name = albumTitle,
                    BackgroundColor = color,
                    AlbumRoles = new List<AlbumRole>()
                    {
                        new AlbumRole()
                        {
                            User = user,
                            Role = Role.Owner
                        }
                    },
                    AlbumTags = tags.Select(t => new AlbumTag()
                    {
                        Tag = context.Tags
                            .FirstOrDefault(ct => ct.Name == t)
                    })
                    .ToArray()
                };

                context.Albums.Add(album);
                context.SaveChanges();
            }

            return $"Album {albumTitle} successfully created!";
        }
    }
}
