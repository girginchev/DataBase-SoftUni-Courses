using System;
using System.Collections.Generic;
using System.Text;
using Employees.App.Command;

namespace Employees.App.Command
{
    class ExitCommand : ICommand
    {
        public string Execute(params string[] args)
        {
            Console.WriteLine("GoodBye");
            Environment.Exit(0);

            return string.Empty;
        }
    }
}
