using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace gifer
{
    public class Gifer
    {
        private static int Port = 42357;
        public static IPEndPoint EndPoint = new IPEndPoint(IPAddress.Loopback, Port);
    }
}
