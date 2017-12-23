using System;
using Stations.Data;
using System.Text;
using Newtonsoft.Json;
using Stations.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Stations.DataProcessor.Dto;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Stations.Models.Enums;
using System.Globalization;
using System.Xml.Linq;

namespace Stations.DataProcessor
{
    public static class Deserializer
    {
        private const string FailureMessage = "Invalid data format.";
        private const string SuccessMessage = "Record {0} successfully imported.";

        public static string ImportStations(StationsDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var deserializedStations = JsonConvert.DeserializeObject<StationDto[]>(jsonString);

            var validStations = new List<Station>();

            foreach (var station in deserializedStations)
            {
                if (!IsValid(station))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }
                if (validStations.Any(e => e.Name == station.Name))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }
                if (station.Town == null)
                {
                    station.Town = station.Name;
                }
                var validStation = new Station()
                {
                    Name = station.Name,
                    Town = station.Town
                };
                validStations.Add(validStation);
                sb.AppendLine(string.Format(SuccessMessage, station.Name));
            }
            context.Stations.AddRange(validStations);
            context.SaveChanges();

            return sb.ToString();
        }



        public static string ImportClasses(StationsDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializedclasses = JsonConvert.DeserializeObject<SeatingClassDto[]>(jsonString);
            var validatedclasses = new List<SeatingClass>();

            foreach (var sc in deserializedclasses)
            {
                if (!IsValid(sc))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }
                var seatingClassExists = validatedclasses.Any(e => e.Name == sc.Name || e.Abbreviation == sc.Abbreviation);
                if (seatingClassExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }
                var seatingClass = AutoMapper.Mapper.Map<SeatingClass>(sc);
                sb.AppendLine(string.Format(SuccessMessage, seatingClass.Name));
                validatedclasses.Add(seatingClass);

            }
            context.SeatingClasses.AddRange(validatedclasses);
            context.SaveChanges();
            return sb.ToString();
        }

        public static string ImportTrains(StationsDbContext context, string jsonString)
        {
            var trainsFromJson = JsonConvert.DeserializeObject<TrainDto[]>(jsonString);

            var result = new StringBuilder();

            var validTrains = new List<Train>();

            foreach (var train in trainsFromJson)
            {
                string trainNumber = train.TrainNumber;
                var type = Enum.Parse<TrainType>(train.Type ?? "HighSpeed");
                var seats = train.Seats;

                var isTrainNumberValid = !String.IsNullOrWhiteSpace(trainNumber)
                    && trainNumber?.Length <= 10
                    && !validTrains.Any(t => t.TrainNumber == trainNumber);

                var areSeatsValid = seats
                    .All(s => context.SeatingClasses
                        .Any(sc => sc.Name == s.Name && sc.Abbreviation == s.Abbreviation)
                        && s.Quantity != null
                        && s.Quantity >= 0);

                if (!isTrainNumberValid || !areSeatsValid)
                {
                    result.AppendLine(FailureMessage);
                    continue;
                }

                var currentTrain = new Train()
                {
                    TrainNumber = trainNumber,
                    Type = type,
                    TrainSeats = new List<TrainSeat>()
                };

                foreach (var seat in train.Seats)
                {
                    var currentSeat = new TrainSeat()
                    {
                        Train = currentTrain,
                        SeatingClass = context.SeatingClasses.SingleOrDefault(sc => sc.Name == seat.Name && sc.Abbreviation == seat.Abbreviation),
                        Quantity = seat.Quantity.Value
                    };

                    currentTrain.TrainSeats.Add(currentSeat);
                }

                validTrains.Add(currentTrain);
                result.AppendLine(string.Format(SuccessMessage, train.TrainNumber));
            }

            context.AddRange(validTrains);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        public static string ImportTrips(StationsDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var deserializedTrips = JsonConvert.DeserializeObject<TripDto[]>(jsonString, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            var validTrips = new List<Trip>();
            foreach (var tripDto in deserializedTrips)
            {
                if (!IsValid(tripDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var train = context.Trains.SingleOrDefault(e => e.TrainNumber == tripDto.Train);
                var destStation = context.Stations.SingleOrDefault(e => e.Name == tripDto.DestinationStation);
                var origStation = context.Stations.SingleOrDefault(e => e.Name == tripDto.OriginStation);
                var depTime = DateTime.ParseExact(tripDto.DepartureTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                var arrTime = DateTime.ParseExact(tripDto.ArrivalTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                TimeSpan timeDiff;
                if (tripDto.TimeDifference != null)
                {
                    timeDiff = TimeSpan.ParseExact(tripDto.TimeDifference, @"hh\:mm", CultureInfo.InvariantCulture);
                }

                var status = Enum.Parse<TripStatus>(tripDto.Status ?? "OnTime");

                if (train == null || destStation == null || origStation == null)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }
                if (depTime > arrTime)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var trip = new Trip()
                {
                    Train = train,
                    DestinationStation = destStation,
                    OriginStation = origStation,
                    DepartureTime = depTime,
                    ArrivalTime = arrTime,
                    Status = status,
                    TimeDifference = timeDiff
                };
                validTrips.Add(trip);
                sb.AppendLine($"Trip from {trip.OriginStation.Name} to {trip.DestinationStation.Name} imported");
            }
            context.Trips.AddRange(validTrips);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportCards(StationsDbContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var deserializedCards = XDocument.Parse(xmlString);

            var validCards = new List<CustomerCard>();

            foreach (var element in deserializedCards.Root.Elements())
            {
                var name = element.Element("Name")?.Value;
                var ageString = element.Element("Age")?.Value;
                if (String.IsNullOrWhiteSpace(ageString) || String.IsNullOrWhiteSpace(name))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var age = int.Parse(ageString);
                if (age < 0 || age > 120 || name.Length > 128)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var cardTypeString = element.Element("CardType")?.Value;
                CardType cardType;

                if (cardTypeString != null)
                {
                    cardType = Enum.Parse<CardType>(cardTypeString);
                }
                else
                {
                    cardType = CardType.Normal;
                }
                var card = new CustomerCard()
                {
                    Name = name,
                    Age = age,
                    Type = cardType
                };
                validCards.Add(card);
                sb.AppendLine(string.Format(SuccessMessage, card.Name));
            }
            context.Cards.AddRange(validCards);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportTickets(StationsDbContext context, string xmlString)
        {
            var xmlDoc = XDocument.Parse(xmlString);

            var elements = xmlDoc.Root.Elements();

            var result = new StringBuilder();

            var validTickets = new List<Ticket>();

            foreach (var element in elements)
            {
                decimal price = decimal.Parse(element.Attribute("price").Value);
                string seat = element.Attribute("seat").Value;
                string orgStation = element.Element("Trip")?.Element("OriginStation")?.Value;
                string destStation = element.Element("Trip")?.Element("DestinationStation")?.Value;
                string departureTimeAsString = element.Element("Trip")?.Element("DepartureTime")?.Value;
                string cardName = element.Element("Card")?.Attribute("Name")?.Value;

                int seatNumber;
                if (!int.TryParse(seat.Substring(2), out seatNumber) || price < 0)
                {
                    result.AppendLine(FailureMessage);
                    continue;
                }

                CustomerCard card = context.Cards.SingleOrDefault(c => c.Name == cardName);
                DateTime departureTime = DateTime.ParseExact(departureTimeAsString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                var trip = context
                    .Trips
                    .Include(t => t.OriginStation)
                    .Include(t => t.DestinationStation)
                    .SingleOrDefault(t => t.OriginStation.Name == orgStation
                                    && t.DestinationStation.Name == destStation
                                    && t.DepartureTime == departureTime
                                    && t.Train.TrainSeats
                                        .Any(ts => ts.SeatingClass.Abbreviation == seat.Substring(0, 2)
                                            && ts.Quantity > 0
                                            && seatNumber <= ts.Quantity)
                                    );

                if (trip == null || (cardName != null && card == null))
                {
                    result.AppendLine(FailureMessage);
                    continue;
                }

                var currentTicket = new Ticket()
                {
                    Price = price,
                    SeatingPlace = seat,
                    TripId = trip.Id,
                    CustomerCardId = card?.Id ?? null
                };

                validTickets.Add(currentTicket);
                result.AppendLine($"Ticket from {trip.OriginStation.Name} to {trip.DestinationStation.Name} " +
                    $"departing at {trip.DepartureTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)} imported.");
            }

            context.Tickets.AddRange(validTickets);
            context.SaveChanges();

            return result.ToString().TrimEnd();

        }

        public static bool IsValid(object obj)
        {
            var validationContet = new ValidationContext(obj);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(obj, validationContet, validationResults, true);
            return isValid;

        }
    }
}