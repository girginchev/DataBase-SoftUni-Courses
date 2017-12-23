using P01_StudentSystem;
using P01_StudentSystem.Data;
using System;

namespace StudentSystem
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new StudentSystemContext())
            {
                db.Database.EnsureCreated();
            }
        }
    }
}
