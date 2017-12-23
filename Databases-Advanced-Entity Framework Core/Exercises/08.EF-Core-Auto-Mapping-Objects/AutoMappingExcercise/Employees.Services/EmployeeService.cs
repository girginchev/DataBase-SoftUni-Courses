

namespace Employees.Services
{
    using System;
    using Employees.Data;
    using Employees.DtoModels;
    using AutoMapper;
    using Employees.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper.QueryableExtensions;

    public class EmployeeService
    {
        private readonly EmployeesContext context;

        public static object PersonalById { get; set; }

        public EmployeeService(EmployeesContext context)
        {
            this.context = context;
        }

        public EmployeeDto ById(int employeeId)
        {
            var empoyee = context.Employees.Find(employeeId);

            var employeeDto = Mapper.Map<EmployeeDto>(empoyee);

            return employeeDto;
        }

        public void AddEmployee(EmployeeDto dto)
        {
            var employee = Mapper.Map<Employee>(dto);
            context.Employees.Add(employee);
            context.SaveChanges();
        }

        public string SetBirthday(int employeeId, DateTime date)
        {
            var employee = context.Employees.Find(employeeId);
            employee.Birthday = date;
            context.SaveChanges();
            return $"{employee.FirstName} {employee.LastName}";
        }

        public string SetAddress(int employeeId, string address)
        {
            var employee = context.Employees.Find(employeeId);
            employee.Address = address;
            context.SaveChanges();
            return $"{employee.FirstName} {employee.LastName}";
        }

        public EmployeePersonalDto PersonById(int employeeId)
        {
            var empoyee = context.Employees.Find(employeeId);

            var employeeDto = Mapper.Map<EmployeePersonalDto>(empoyee);

            return employeeDto;
        }

        public EmployeePersonalDto SetManager(int employeeId, int managerId)
        {
            var employee = context.Employees
                .Find(employeeId);

            var manager = context.Employees
                .Find(managerId);

            employee.Manager = manager;

            context.SaveChanges();

            var employeePersonalDto = Mapper.Map<EmployeePersonalDto>(employee);

            return employeePersonalDto;
        }

        public ManagerDto GetManager(int managerId)
        {
            var employee = context.Employees
                .Include(m => m.ManagerEmployees)
                .SingleOrDefault(m => m.Id == managerId);

            var managerDto = Mapper.Map<ManagerDto>(employee);

            return managerDto;
        }

        public List<EmployeeManagerDto> OlderThan(int age)
        {
            var employees = context.Employees
                .Where(e => e.Birthday != null && Math.Floor((DateTime.Now - e.Birthday.Value).TotalDays / 365) > age)
                .Include(e => e.Manager)
                .ProjectTo<EmployeeManagerDto>()
                .ToList();

            return employees;
        }
    }
}
