using System;
using System.Net;
//using Contoso.Practices.TdsServer;

namespace Contoso
{
    internal class Program
    {
        public static void Main()
        {
            new SimpleTelnetServer();
            Console.WriteLine("Done");
        }
    }
}