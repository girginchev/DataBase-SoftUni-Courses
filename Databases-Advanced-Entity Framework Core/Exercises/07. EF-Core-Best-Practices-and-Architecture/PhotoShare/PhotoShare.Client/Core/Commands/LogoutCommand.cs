namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Text;


    public class LogoutCommand
    {
        public static string Execute()
        {
            if (Session.User == null)
            {
                throw new InvalidOperationException("You should log in first in order to logout.");
            }

            var usernName = Session.User.Username;
            Session.User = null;

            return $"User {usernName} successfully logged out!";
        }
    }
}
