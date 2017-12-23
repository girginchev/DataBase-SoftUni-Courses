using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Employees.Services.Contract;
using Microsoft.Extensions.DependencyInjection;
using Employees.Services;
using Employees.Data;

namespace Employees.App
{
    public class Engine
    {
        private readonly IServiceProvider serviceProvider;

        public Engine(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        internal void Run()
        {
            using (var db = new EmployeesContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            while (true)
            {
                var input = Console.ReadLine();

                var commandTokens = input.Split(' ');

                var commandName = commandTokens[0];

                var commandArgs = commandTokens.Skip(1).ToArray();

                var command = CommandParser.Parse(serviceProvider, commandName);

                var result = command.Execute(commandArgs);

                Console.WriteLine(result);

            }
        }
    }
}
