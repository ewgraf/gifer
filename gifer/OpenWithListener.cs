using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gifer
{
    public class OpenWithListener
    {
        private TcpListener _tcpListener;
        private CancellationTokenSource _cts;
        public bool Running { get; private set; } = false;

        public OpenWithListener() { }

        public async void Start(Action<string> continueWith)
        {
            _cts = new CancellationTokenSource();
            _tcpListener = new TcpListener(Gifer.EndPoint); // to do: dynamikly select port and store in repository
            try {
                _tcpListener.Start();
                Running = true;
                //just fire and forget. We break from the "forgotten" async loops
                //in AcceptClientsAsync using a CancellationToken from `cts`
                await AcceptClientsAsync(_tcpListener, _cts.Token, continueWith);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
                Application.Exit();
            } finally {
                _tcpListener.Stop();
                Running = false;
            }
        }

        public void Stop()
        {
            try {
                _cts.Cancel();
                _tcpListener.Stop();
                Running = false;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        public async Task AcceptClientsAsync(TcpListener listener, CancellationToken token, Action<string> continueWith)
        {
            //once again, just fire and forget, and use the CancellationToken
            //to signal to the "forgotten" async invocation.
            while (!token.IsCancellationRequested) {
                try {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    NetworkStream networkStream = client.GetStream();
                    byte[] message = new byte[client.ReceiveBufferSize];
                    networkStream.Read(message, 0, client.ReceiveBufferSize);
                    networkStream.Close();
                    networkStream.Dispose();
                    string filePath = Encoding.UTF8.GetString(message).Replace("\0", ""); // due to buffer is 64k length and there are '0's after the string at 'message'
                    //MessageBox.Show(filePath);
                    continueWith(filePath);
                } catch (InvalidOperationException) { // when TcpListener is Stop'ed while AcceptTcpClientAsync()'int - InvalidOperationException of 'AccessToDisposedObject' is thrown
                    if (!token.IsCancellationRequested) {
                        throw;
                    }
                }
            }
        }
    }
}
