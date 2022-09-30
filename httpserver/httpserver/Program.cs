using System;
using System.Net;
using System.IO;

namespace httpserver
{
    class Program
    {
        private static bool _keepRunning = true;
        static void Main(string[] args)
        {
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                Program._keepRunning = false;
            };

            var httpServer = new HttpServer();
            httpServer.Start();

            while (Program._keepRunning) { }

            //Закрытие происходит на "Ctrl + C" в консоли

            //httpServer.Stop();
        }
    }
}
