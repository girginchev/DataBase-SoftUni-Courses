namespace Employees.App.Command
{
    using Employees.Services;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ManagerInfoCommand : ICommand
    {
        private readonly EmployeeService employeeService;

        public ManagerInfoCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public string Execute(params string[] args)
        {
            int managerId = int.Parse(args[0]);

            var managerDto = employeeService.GetManager(managerId);

            var output = new StringBuilder();

            output.Append($"{managerDto.FirstName} {managerDto.LastName} | employees: {managerDto.ManagerEmployeesCount}" + Environment.NewLine);

            foreach (var emp in managerDto.ManagerEmployees)
            {
                output.Append($"    - {emp.FirstName} {emp.LastName} - ${emp.Salary:f2}" + Environment.NewLine);
            }

            return output.ToString().TrimEnd();
        }
    }
}
