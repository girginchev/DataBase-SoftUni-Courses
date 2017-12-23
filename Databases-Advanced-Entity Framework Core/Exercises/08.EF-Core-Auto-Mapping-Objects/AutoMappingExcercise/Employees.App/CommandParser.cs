namespace Employees.App
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;
    using System.Linq;
    using Employees.App.Command;

    internal class CommandParser
    {
        private readonly IServiceProvider serviceProvider;

        public CommandParser(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public static ICommand Parse(IServiceProvider serviceProvider, string commandName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var commandTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ICommand))).ToArray();

            var commandType = commandTypes.SingleOrDefault(t => t.Name.ToLower() == $"{commandName.ToLower()}command");

            if (commandType == null)
            {
                throw new InvalidOperationException("Invalid command!");
            }

            var constructor = commandType.GetConstructors().First();

            var constructorParams = constructor.GetParameters().Select(p => p.ParameterType).ToArray();

            var constructorArgs = constructorParams.Select(p => serviceProvider.GetService(p)).ToArray();

            var command = (ICommand)constructor.Invoke(constructorArgs);

            return command;
        }
    }
}
