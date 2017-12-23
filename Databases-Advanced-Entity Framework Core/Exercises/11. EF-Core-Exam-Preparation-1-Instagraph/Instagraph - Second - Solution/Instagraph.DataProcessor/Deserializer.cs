namespace Instagraph.DataProcessor
{
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
    using System.ComponentModel.DataAnnotations;
    using Instagraph.DataProcessor.Dto;
    using System.Xml.Serialization;
    using System.IO;

    public class Deserializer
    {
        public static string ImportPictures(InstagraphContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var deserializedPictures = JsonConvert.DeserializeObject<Picture[]>(jsonString);

            var validPictures = new List<Picture>();
            foreach (var picture in deserializedPictures)
            {
                if (String.IsNullOrWhiteSpace(picture.Path)  || validPictures.Any(e => e.Path == picture.Path) 
                    || picture.Size <= 0 || context.Pictures.Any(e=>e.Path == picture.Path))
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }
                validPictures.Add(picture);
                sb.AppendLine($"Successfully imported Picture {picture.Path}.");
            }
            context.AddRange(validPictures);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportUsers(InstagraphContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var deserializedUsers = JsonConvert.DeserializeObject<UserDto[]>(jsonString);
            var validUsers = new List<User>();
            foreach (var userDto in deserializedUsers)
            {
                if (!IsValid(userDto))
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }
                var picture = context.Pictures.SingleOrDefault(e => e.Path == userDto.ProfilePicture);
                if (picture == null)
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }
                var user = new User()
                {
                    Username = userDto.Username,
                    Password = userDto.Password,
                    ProfilePicture = picture
                };
                validUsers.Add(user);
                sb.AppendLine($"Successfully imported User {user.Username}.");
            }
            context.Users.AddRange(validUsers);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportFollowers(InstagraphContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var deserializedFollowers = JsonConvert.DeserializeObject<UserFollowerDto[]>(jsonString);
            var validFollowers = new List<UserFollower>();
            foreach (var ufDto in deserializedFollowers)
            {
                var user = context.Users.FirstOrDefault(e => e.Username == ufDto.User);
                var follower = context.Users.FirstOrDefault(e => e.Username == ufDto.Follower);

                if (user == null || follower == null)
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }
                var isExists = validFollowers.Any(e => e.User.Username == ufDto.User && e.Follower.Username == ufDto.Follower);
                if (isExists)
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }
                var userFollower = new UserFollower()
                {
                    User = user,
                    Follower = follower
                };
                validFollowers.Add(userFollower);
                sb.AppendLine($"Successfully imported Follower {userFollower.Follower.Username} to User {userFollower.User.Username}.");
            }
            context.UsersFollowers.AddRange(validFollowers);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportPosts(InstagraphContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(PostDto[]), new XmlRootAttribute("posts"));
            var deserializedPosts = (PostDto[])serializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(xmlString)));
            var validPosts = new List<Post>();
            foreach (var postDto in deserializedPosts)
            {
                var user = context.Users.FirstOrDefault(e => e.Username == postDto.User);
                var picture = context.Pictures.FirstOrDefault(e => e.Path == postDto.Picture);

                if (string.IsNullOrWhiteSpace(postDto.Caption) || string.IsNullOrWhiteSpace(postDto.User) || string.IsNullOrWhiteSpace(postDto.Picture)
                    || user == null || picture == null)
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }
                var post = new Post()
                {
                    Caption = postDto.Caption,
                    Picture = picture,
                    User = user
                };
                validPosts.Add(post);
                sb.AppendLine($"Successfully imported Post {post.Caption}.");
            }
            context.Posts.AddRange(validPosts);
            context.SaveChanges();

            return sb.ToString().Trim(); 
        }

        public static string ImportComments(InstagraphContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var xdoc = XDocument.Parse(xmlString);
            var elements = xdoc.Root.Elements();
            var validComments = new List<Comment>();

            foreach (var element in elements)
            {
                var content = element.Element("content")?.Value;
                var userName = element.Element("user")?.Value;
                var postIdAsString = element.Element("post")?.Attribute("id")?.Value;

                if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(postIdAsString))
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }
                var user = context.Users.FirstOrDefault(e => e.Username == userName);
                var postId = int.Parse(postIdAsString);
                var post = context.Posts.FirstOrDefault(e => e.Id == postId);
                if (user == null || post == null)
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }
                var comment = new Comment()
                {
                    Content = content,
                    Post = post,
                    User = user
                };
                validComments.Add(comment);
                sb.AppendLine($"Successfully imported Comment {comment.Content}.");
            }
            context.AddRange(validComments);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

            return isValid;
        }
    }
}
