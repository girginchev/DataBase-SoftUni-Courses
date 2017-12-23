using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Instagraph.DataProcessor.Dto
{
    public class UserFollowerDto
    {
        [Required]
        [MaxLength(30)]
        public string User { get; set; }

        [Required]
        [MaxLength(30)]
        public string Follower { get; set; }
    }
}
