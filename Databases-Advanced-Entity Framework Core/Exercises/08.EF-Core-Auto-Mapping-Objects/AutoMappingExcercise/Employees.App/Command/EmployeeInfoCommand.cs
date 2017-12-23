namespace Employees.App.Command
{
    using Employees.DtoModels;
    using Employees.Services;
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    class EmployeeInfoCommand : ICommand
    {
        private readonly EmployeeService employeeService;
        public EmployeeInfoCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public string Execute(params string[] args)
        {
            var employeeId = int.Parse(args[0]);

            var employee = employeeService.ById(employeeId);

            return $"ID: {employeeId} - {employee.FirstName} {employee.LastName} - ${employee.Salary:F2}";
        }
    }
}
