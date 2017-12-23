using System;
using Stations.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Stations.Models.Enums;
using System.Globalization;
using Newtonsoft.Json;

namespace Stations.DataProcessor
{
	public class Serializer
	{
		public static string ExportDelayedTrains(StationsDbContext context, string dateAsString)
		{
            var status = TripStatus.Delayed;
            var date = DateTime.ParseExact(dateAsString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var delayedTrains = context.Trains.Where(e => e.Trips.Any(x => x.Status == status && x.DepartureTime <= date))
                .Distinct()
                .Select(e => new
                {
                    TrainNumber = e.TrainNumber,
                    DelayedTimes = e.Trips.Where(t => t.Status == status && t.DepartureTime <= date).Count(),
                    MaxDelayedTime = e.Trips.Where(t => t.Status == status && t.DepartureTime <= date)
                    .OrderByDescending(t => t.TimeDifference).Select(t => t.TimeDifference)
                    .FirstOrDefault()
                })
                .OrderByDescending(o=>o.DelayedTimes)
                .ThenByDescending(o=>o.MaxDelayedTime)
                .ThenBy(o=>o.TrainNumber)
                .ToArray();

            var serializedTrains = JsonConvert.SerializeObject(delayedTrains, Formatting.Indented);
            return serializedTrains;

    }

		public static string ExportCardsTicket(StationsDbContext context, string cardType)
		{
			throw new NotImplementedException();
		}
	}
}