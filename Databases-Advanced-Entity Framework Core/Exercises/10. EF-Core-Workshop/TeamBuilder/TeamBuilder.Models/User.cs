using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamBuilder.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 3)]
        public string Username { get; set; }

        [MaxLength(25)]
        public string FirstName { get; set; }

        [MaxLength(25)]
        public string LastName { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 6)]
        public string Password { get; set; }

        public Gender Gender { get; set; }

        [Range(0, int.MaxValue)]
        public int? Age { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Invitation> ReceivedInvitations { get; set; } = new List<Invitation>();

        public ICollection<Event> CreatedEvents { get; set; } = new List<Event>();

        public ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();

        public ICollection<Team> CreatedTeams { get; set; } = new List<Team>();  
    }
}
