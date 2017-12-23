using P03_FootballBetting.Data;
using P03_FootballBetting.Data.Models;
using System;

namespace FootballBetting.StartUp
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            //using (var db = new FootballBettingContext())
            //{
            //    db.Database.EnsureCreated();
            //}
            var res = Result.Draw;
            Console.WriteLine((int)res);
        }
    }
}
