using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Stations.Models.Enums;

namespace Stations.Models
{
    public class Trip
    {
        public int Id { get; set; }

        [Required]
        public int OriginStationId { get; set; }
        public Station OriginStation { get; set; }

        [Required]
        public int DestinationStationId { get; set; }
        public Station DestinationStation { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        public int TrainId { get; set; }
        public Train Train { get; set; }

        public TripStatus Status { get; set; } 

        public TimeSpan? TimeDifference { get; set; }

    }
}
