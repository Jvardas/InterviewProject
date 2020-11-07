using IPInfoProviderLibrary;
using System;

namespace TestApp
{
    /// <summary>
    /// Test console app to test that the Class Library is working properly
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            IPInfoProvider ipInfo = new IPInfoProvider();
            string ip = "205.251.66.2";
            var ipD = ipInfo.GetDetails(ip);

            Console.WriteLine(ipD.City + " " + ipD.Continent + " " + ipD.Country + " " + ipD.Latitude + " " + ipD.Longtitude);

        }
    }
}
