using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Stations.Models
{
    public class TrainSeat
    {
        public int Id { get; set; }

        [Required]
        public int TrainId { get; set; }
        public Train Train { get; set; }

        [Required]
        public int SeatingClassId { get; set; }
        public SeatingClass SeatingClass { get; set; }

        [Required]
        [Range(0,int.MaxValue)]
        public int Quantity { get; set; }
    }
}
