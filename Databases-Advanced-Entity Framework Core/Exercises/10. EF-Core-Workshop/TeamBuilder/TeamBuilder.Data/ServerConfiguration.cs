using System;
using System.Collections.Generic;
using System.Text;

namespace TeamBuilder.Data
{
    public class ServerConfiguration
    {
        internal static string ConnectionString => @"Server=.;Database=TeamBuilder;Integrated Security=True";
    }
}
