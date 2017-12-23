namespace PhotoShare.Client.Core.Commands
{
    using Microsoft.EntityFrameworkCore;
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class UploadPictureCommand
    {
        // UploadPicture <albumName> <pictureTitle> <pictureFilePath>
        public static string Execute(string[] data)
        {
            var albumName = data[1];
            var pictureTitle = data[2];
            var pictureFilePath = data[3];

            if (data.Length < 4)
            {
                throw new IndexOutOfRangeException($"Command {data[0]} not valid!");
            }

            if (Session.User == null)
            {
                throw new ArgumentException("You should login first!");
            }

            using (var db = new PhotoShareContext())
            {
                var album = db.Albums.Include(a => a.AlbumRoles).SingleOrDefault(a => a.Name == albumName);

                if (album == null)
                {
                    throw new ArgumentException($"Album {albumName} not found!");
                }

                var picture = new Picture()
                {
                    Album = album,
                    Title = pictureTitle,
                    Path = pictureFilePath
                };

                db.Pictures.Add(picture);
                db.SaveChanges();
                return $"Picture {pictureTitle} added to {albumName}!";
            }
        }
    }
}
