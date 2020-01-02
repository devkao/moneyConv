using MoneyConv.Implementation.V1;
using System;
using System.ServiceModel;

namespace MoneyConvConsoleHost
{
    /// <summary>
    /// basic console application to host the wcf service
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            var srvHst = new ServiceHost(typeof(MoneyConvImplV1));

            srvHst.Open();

            Console.WriteLine("press any key to quit");
            Console.ReadLine();
            srvHst.Close();
        }
    }
}