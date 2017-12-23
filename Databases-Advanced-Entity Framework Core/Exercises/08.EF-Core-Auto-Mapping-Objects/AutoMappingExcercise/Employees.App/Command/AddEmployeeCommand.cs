using Employees.DtoModels;
using Employees.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Employees.App.Command
{
    class AddEmployeeCommand : ICommand
    {
        private readonly EmployeeService employeeService;
        public AddEmployeeCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public string Execute(params string[] args)
        {
            string firstName = args[0];
            string lastName = args[1];
            decimal salary = decimal.Parse(args[2]);

            var employeeDto = new EmployeeDto(firstName, lastName, salary);

            employeeService.AddEmployee(employeeDto);

            return $"Employee {firstName} {lastName} succesfully added!";
            
        }
    }
}
