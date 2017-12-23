using System;

using Instagraph.Data;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Instagraph.DataProcessor
{
    public class Serializer
    {
        public static string ExportUncommentedPosts(InstagraphContext context)
        {
            var uncommentedPosts = context.Posts.Include(p=>p.Picture).Include(u=>u.User)
                .Where(e => e.Comments.Count == 0).OrderBy(e => e.Id).ToList();
            var posts = new List<PostDto>();

            foreach (var up in uncommentedPosts)
            {
                var postDto = new PostDto
                {
                    Id = up.Id,
                    Picture = up.Picture.Path,
                    User = up.User.Username
                };
                posts.Add(postDto);
            }

            var jsonPosts = JsonConvert.SerializeObject(posts, Formatting.Indented);

            return jsonPosts;
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
            var users = context
              .Users
              .Include(u => u.Posts)
              .ThenInclude(p => p.Comments)
              .ToList();

            var selectedUsers = new List<UserPostDto>();

            var xmlDoc = new XDocument(new XElement("users"));

            foreach (var user in users)
            {
                var mostCommentedPostsCount = user.Posts.Select(p => p.Comments.Count).ToArray();

                int mostComments = 0;

                if (mostCommentedPostsCount.Any())
                {
                    mostComments = mostCommentedPostsCount.OrderByDescending(c => c).First();
                }

                var currentUserPostDto = new UserPostDto()
                {
                    Username = user.Username,
                    MostComments = mostComments
                };

                selectedUsers.Add(currentUserPostDto);
            }

            foreach (var user in selectedUsers.OrderByDescending(su => su.MostComments).ThenBy(su => su.Username))
            {
                xmlDoc.Root.Add(new XElement("user",
                                    new XElement("Username", user.Username),
                                    new XElement("MostComments", user.MostComments)));
            }

            string xmlResult = xmlDoc.ToString();

            return xmlResult;
        }
    }
}
