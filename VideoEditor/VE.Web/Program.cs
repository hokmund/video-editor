using System;
using Microsoft.Owin.Hosting;

namespace VE.Web
{
    internal class Program
    {
        private static void Main()
        {
            // Specify the URI to use for the local host:
            const string baseUri = "http://localhost:8080";

            Console.WriteLine("Starting web Server...");
            WebApp.Start<Startup>(baseUri);
            Console.WriteLine("Server running at {0} - press Enter to quit. ", baseUri);
            Console.ReadLine();
        }
    }
}
