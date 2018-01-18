using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShitheadServer
{
    public class Program
    {
        private static Server server;

        public static void Main(string[] args)
        {
            server = new Server(7772);
            server.Run();
        }
    }
}
