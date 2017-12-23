
namespace Employees.App.Command
{
    using Employees.DtoModels;
    using Employees.Services;
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;


    class SetAddressCommand : ICommand
    {

        private readonly EmployeeService employeeService;
        public SetAddressCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);
            var address = string.Join(" ", args.Skip(1));
            var employeeName = employeeService.SetAddress(employeeId, address);

            return $"{employeeName}'s address was set to {address}";
        }
    }
}
