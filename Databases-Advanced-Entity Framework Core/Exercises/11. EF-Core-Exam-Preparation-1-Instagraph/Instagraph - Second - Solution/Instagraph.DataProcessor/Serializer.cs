using System;

using Instagraph.Data;
using System.Linq;
using Instagraph.DataProcessor.Dto;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Instagraph.DataProcessor
{
    public class Serializer
    {
        public static string ExportUncommentedPosts(InstagraphContext context)
        {
            var uncommentedPosts = context.Posts.Where(e => e.Comments.Count == 0)
                .Select(e => new PostDtoExport
                {
                    Id = e.Id,
                    User = e.User.Username,
                    Picture = e.Picture.Path
                }).OrderBy(e => e.Id).ToArray();

            var result = JsonConvert.SerializeObject(uncommentedPosts, Formatting.Indented);
            return result;
        }

        public static string ExportPopularUsers(InstagraphContext context)
        {
            var popularUsers = context
                     .Users
                     .Where(u => u.Posts
                         .Any(p => p.Comments
                             .Any(c => u.Followers
                                 .Any(uf => uf.FollowerId == c.UserId))))
                     .OrderBy(u => u.Id)
                     .Select(u => new
                     {
                         Username = u.Username,
                         Followers = u.Followers.Count
                     })
                     .ToArray();

            var result = JsonConvert.SerializeObject(popularUsers, Formatting.Indented);
            return result;
        }

        public static string ExportCommentsOnPosts(InstagraphContext context)
        {
            var users = context.Users.Include(e => e.Posts).ThenInclude(e => e.Comments).ToArray();

            var validUsers = new List<CommentsOnPostDto>();

            foreach (var user in users)
            {
                int  mostCommentsCount = user.Posts.Select(e => e.Comments.Count).ToArray().OrderByDescending(e => e).FirstOrDefault();

                var validUser = new CommentsOnPostDto()
                {
                    Username = user.Username,
                    MostComments = mostCommentsCount
                };
                validUsers.Add(validUser);
            }
            var result = validUsers.OrderByDescending(e => e.MostComments).ThenBy(e => e.Username);

            var xmlDoc = new XDocument(new XElement("users"));

            foreach (var user in result)
            {
                xmlDoc.Root.Add(new XElement("user",new XElement("Username", user.Username), new XElement("MostComments", user.MostComments)));
            }
            return xmlDoc.ToString();
        }
    }
}
