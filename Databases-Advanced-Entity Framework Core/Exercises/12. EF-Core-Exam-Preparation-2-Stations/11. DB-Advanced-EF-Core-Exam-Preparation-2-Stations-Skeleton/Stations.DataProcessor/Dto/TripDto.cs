using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Stations.DataProcessor.Dto
{
    /*
    "Train": "KB20012",
    "OriginStation": "Sofia",
    "DestinationStation": "Sofia Sever",
    "DepartureTime": "27/12/2016 12:00",
    "ArrivalTime": "27/12/2016 12:30",
    "Status": "OnTime",
     */
    public class TripDto
    {
        [Required]
        [MaxLength(10)]
        public string Train { get; set; }

        [Required]
        public string OriginStation { get; set; }

        [Required]
        public string DestinationStation { get; set; }

        [Required]
        public string DepartureTime { get; set; }

        [Required]
        public string ArrivalTime { get; set; }

        public string Status { get; set; } = "OnTime";

        public string TimeDifference { get; set; }
    }
}
