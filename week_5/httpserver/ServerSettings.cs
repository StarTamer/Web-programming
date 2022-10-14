using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace httpserver
{
    class ServerSettings
    {
        public int Port { get; set; } = 8888;

        public string Path { get; set; } = @"./STEAM/";
    }
}
