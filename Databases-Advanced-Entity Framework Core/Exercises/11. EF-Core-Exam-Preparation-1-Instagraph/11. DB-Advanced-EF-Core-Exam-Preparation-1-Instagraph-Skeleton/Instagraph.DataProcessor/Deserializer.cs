using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

using Newtonsoft.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

using Instagraph.Data;
using Instagraph.Models;

namespace Instagraph.DataProcessor
{
    public class Deserializer
    {
        public static string ImportPictures(InstagraphContext context, string jsonString)
        {
            var pictures = JsonConvert.DeserializeObject<Picture[]>(jsonString);
            var sb = new StringBuilder();
            var resPictures = new List<Picture>();
            foreach (var p in pictures)
            {
                var isValid = !String.IsNullOrWhiteSpace(p.Path) && p.Size > 0;
                var isUnique = resPictures.Any(x=>x.Path == p.Path) || context.Pictures.Any(e => e.Path == p.Path);
                if (isValid && !isUnique)
                {
                    var picture = new Picture
                    {
                        Path = p.Path,
                        Size = p.Size
                    };
                    sb.AppendLine($"Successfully imported Picture {p.Path}.");
                    resPictures.Add(picture);
                }
                else
                {
                    sb.AppendLine("Error: Invalid data.");
                }
            }
            context.Pictures.AddRange(resPictures);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportUsers(InstagraphContext context, string jsonString)
        {
            var deserializedUsers = JsonConvert.DeserializeObject<UserDto[]>(jsonString);

            var sb = new StringBuilder();
            var users = new List<User>();

            foreach (var u in deserializedUsers)
            {
                bool isValid = !String.IsNullOrWhiteSpace(u.Username) 
                    && u.Username.Length <=30
                    && !String.IsNullOrWhiteSpace(u.Password) 
                    && u.Password.Length <=20
                    && !String.IsNullOrWhiteSpace(u.ProfilePicture);

                var picture = context.Pictures.FirstOrDefault(e => e.Path == u.ProfilePicture);

                var isUserExists = users.Any(e => e.Username == u.Username);

                if (isValid && picture !=null && !isUserExists)
                {
                    var user = new User
                    {
                        Username = u.Username,
                        Password = u.Password,
                        ProfilePicture = picture
                    };
                    users.Add(user);
                    sb.AppendLine($"Successfully imported User {u.Username}.");
                }
                else
                {
                    sb.AppendLine("Error: Invalid data.");
                }
            }
            context.Users.AddRange(users);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportFollowers(InstagraphContext context, string jsonString)
        {
            var userFollowerDto = JsonConvert.DeserializeObject<FollowerDto[]>(jsonString);
            var sb = new StringBuilder();
            var usersFollowers = new List<UserFollower>();
            foreach (var uf in userFollowerDto)
            {
                var user = context.Users.FirstOrDefault(e => e.Username == uf.User);
                var follower = context.Users.FirstOrDefault(e => e.Username == uf.Follower);

                var isImported = usersFollowers.Any(e => e.User == user && e.Follower == follower);

                if (user != null && follower != null && !isImported)
                {
                    var userFollower = new UserFollower
                    {
                        User = user,
                        Follower = follower
                    };
                    usersFollowers.Add(userFollower);
                    sb.AppendLine($"Successfully imported Follower {follower.Username} to User {user.Username}.");
                }
                else
                {
                    sb.AppendLine("Error: Invalid data.");
                }
                    
            }
            context.UsersFollowers.AddRange(usersFollowers);
            context.SaveChanges();
            return sb.ToString();
        }

        public static string ImportPosts(InstagraphContext context, string xmlString)
        {
            var deserializedPosts = XDocument.Parse(xmlString);

            var sb = new StringBuilder();

            var posts = new List<Post>();

            foreach (var p in deserializedPosts.Root.Elements())
            {
                var caption = p.Element("caption")?.Value;
                var username = p.Element("user")?.Value;
                var picturePath = p.Element("picture")?.Value;

                bool isValid = !String.IsNullOrWhiteSpace(caption) && !String.IsNullOrWhiteSpace(username) && !String.IsNullOrWhiteSpace(picturePath);

                var user = context.Users.FirstOrDefault(e => e.Username == username);
                var pic = context.Pictures.FirstOrDefault(e => e.Path == picturePath);


                if (user != null && pic != null && isValid)
                {
                    var post = new Post
                    {
                        Caption = caption,
                        User = user,
                        Picture = pic
                    };
                    posts.Add(post);
                    sb.AppendLine($"Successfully imported Post {caption}.");
                }
                else
                {
                    sb.AppendLine("Error: Invalid data.");
                }
            }
            context.Posts.AddRange(posts);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportComments(InstagraphContext context, string xmlString)
        {
            var xmlDoc = XDocument.Parse(xmlString);

            var sb = new StringBuilder();
            var comments = new List<Comment>();

            foreach (var element in xmlDoc.Root.Elements())
            {
                var content = element.Element("content")?.Value;
                var userName = element.Element("user")?.Value;
                var postIdString = element.Element("post")?.Attribute("id")?.Value;
                var postId = 0;
                if (!String.IsNullOrWhiteSpace(postIdString))
                {
                    postId = int.Parse(postIdString);
                }
                else
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }

                var isValid = !String.IsNullOrWhiteSpace(content) && !String.IsNullOrWhiteSpace(userName) && !String.IsNullOrWhiteSpace(postIdString);
                var user = context.Users.FirstOrDefault(e => e.Username == userName);
                var post = context.Posts.FirstOrDefault(e => e.Id == postId);

                if (isValid && user != null && post != null )
                {
                    var comment = new Comment
                    {
                        Content = content,
                        User = user,
                        PostId = postId
                    };
                    comments.Add(comment);
                    sb.AppendLine($"Successfully imported Comment {content}.");
                }
                else
                {
                    sb.AppendLine("Error: Invalid data.");
                }

            }
            context.Comments.AddRange(comments);
            context.SaveChanges();
            return sb.ToString().Trim();
        }
    }
}
