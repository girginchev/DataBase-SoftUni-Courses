namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class ShareAlbumCommand
    {
        // ShareAlbum <albumId> <username> <permission>
        // For example:
        // ShareAlbum 4 dragon321 Owner
        // ShareAlbum 4 dragon11 Viewer
        public static string Execute(string[] data)
        {
            var albumId = int.Parse(data[1]);
            var userName = data[2];
            var permission = data[3].ToLower();

            if (data.Length < 4)
            {
                throw new IndexOutOfRangeException($"Command {data[0]} not valid!");
            }

            using (var db = new PhotoShareContext())
            {
                var album = db.Albums.FirstOrDefault(e => e.Id == albumId);

                if (album == null)
                {
                    throw new ArgumentException($"Album {albumId} not found!");
                }

                var user = db.Users.FirstOrDefault(e => e.Username == userName);

                if (Session.User.Username != userName)
                {
                    throw new ArgumentException("Invalid credentials!");
                }

                if (user == null)
                {
                    throw new ArgumentException($"User {userName} not found!");
                }

                var albumRole = new AlbumRole();

                switch (permission)
                {
                    case "owner":
                        albumRole.Album = album;
                        albumRole.User = user;
                        albumRole.Role = Role.Owner;
                        break;
                    case "viewer":
                        albumRole.Album = album;
                        albumRole.User = user;
                        albumRole.Role = Role.Viewer;
                        break;
                    default:
                        throw new ArgumentException("Permission must be either “Owner” or “Viewer”!");
                }
                db.AlbumRoles.Add(albumRole);
                db.SaveChanges();

                return $"Username {userName} added to album {album.Name} ({permission})";
            }

        }
    }
}
