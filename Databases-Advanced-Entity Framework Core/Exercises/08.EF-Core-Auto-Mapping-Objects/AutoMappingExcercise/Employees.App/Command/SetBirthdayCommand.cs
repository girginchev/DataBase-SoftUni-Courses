using Employees.DtoModels;
using Employees.Services;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace Employees.App.Command
{
    class SetBirthdayCommand : ICommand
    {
        private readonly EmployeeService employeeService;
        public SetBirthdayCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);
            var date = DateTime.ParseExact(args[1], "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var employeeName = employeeService.SetBirthday(employeeId, date);

            return $"{employeeName}'s birthday was set to {date}";
        }
    }
}
