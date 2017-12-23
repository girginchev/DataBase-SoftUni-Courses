namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class AddTagToCommand 
    {
        // AddTagTo <albumName> <tag>
        public static string Execute(string[] data)
        {
            var albumName = data[1];
            var tagName = data[2];

            if (data.Length < 3)
            {
                throw new IndexOutOfRangeException($"Command {data[0]} not valid!");
            }

            if (Session.User == null)
            {
                throw new ArgumentException("Invalid credentials!");
            }

            using (var db = new PhotoShareContext())
            {
                var album = db.Albums.FirstOrDefault(e => e.Name == albumName);
                var tag = db.Tags.FirstOrDefault(e => e.Name == "#" + tagName);

                if (album == null || tag == null)
                {
                    throw new ArgumentException("Either tag or album do not exist!");
                }

                var albumTag = new AlbumTag()
                {
                    Album = album,
                    Tag = tag
                };
                db.AlbumTags.Add(albumTag);
                db.SaveChanges();
                return $"Tag {tagName} added to {albumName}!";
            }
        }
    }
}
