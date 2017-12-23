namespace Employees.App.Command
{
    using Employees.Services;
    using System;

    public class SetManagerCommand : ICommand
    {
        private readonly EmployeeService employeeService;

        public SetManagerCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public string Execute(params string[] args)
        {
            var employeeId = int.Parse(args[0]);
            var managerId = int.Parse(args[1]);

            var employeePersonalDto = employeeService.SetManager(employeeId, managerId);

            return $"{employeePersonalDto.Manager.FirstName} {employeePersonalDto.Manager.LastName} is successfuly added as a manager to {employeePersonalDto.FirstName} {employeePersonalDto.LastName}";
        }
    }
}
