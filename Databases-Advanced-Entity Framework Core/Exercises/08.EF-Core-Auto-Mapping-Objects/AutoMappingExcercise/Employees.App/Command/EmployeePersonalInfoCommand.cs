namespace Employees.App.Command
{
    using Employees.DtoModels;
    using Employees.Services;
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    class EmployeePersonalInfoCommand : ICommand
    {
        private readonly EmployeeService employeeService;
        public EmployeePersonalInfoCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public string Execute(params string[] args)
        {
            var employeeId = int.Parse(args[0]);

            var employee = employeeService.PersonById(employeeId);
            var birthday = "[no birthday specified]";
            if (employee.Birthday != null)
            {
                birthday = employee.Birthday.Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            string address = employee.Address;
            if (address == null)
            {
                address = "[no address specified]";
            }

            string result = $"ID: {employeeId} - {employee.FirstName} {employee.LastName} - ${employee.Salary:F2}" + Environment.NewLine +
            $"Birthday: {birthday}" + Environment.NewLine +
            $"Address: {address}";

            return result;
        }
    }
}
