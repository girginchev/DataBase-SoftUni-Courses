namespace PhotoShare.Client.Core.Commands
{
    using Models;
    using Data;
    using Utilities;
    using System;

    public class AddTagCommand
    {
        // AddTag <tag>
        public static string Execute(string[] data)
        {
            string tag = data[1].ValidateOrTransform();

            if (data.Length < 2)
            {
                throw new IndexOutOfRangeException($"Command {data[0]} not valid!");
            }

            if (Session.User == null)
            {
                throw new ArgumentException("Invalid credentials!");
            }

            using (PhotoShareContext context = new PhotoShareContext())
            {
                context.Tags.Add(new Tag
                {
                    Name = tag
                });

                context.SaveChanges();
            }

            return $"Tag {tag} was added successfully!";
        }
    }
}
