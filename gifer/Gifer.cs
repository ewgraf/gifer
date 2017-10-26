using System.Net;

namespace gifer {
    public class Gifer {
        private static int Port = 42357;
        public static IPEndPoint EndPoint = new IPEndPoint(IPAddress.Loopback, Port);
    }
}
