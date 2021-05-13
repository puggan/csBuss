using ConnectionGenerator = System.Collections.Generic.IEnumerable<System.Net.Sockets.TcpClient>;
using UiGenerator = System.Collections.Generic.IEnumerable<Bussen.Question>;
using IPAddress = System.Net.IPAddress;
using TcpListener = System.Net.Sockets.TcpListener;
using AsyncNullString = System.Threading.Tasks.Task<string?>;
using AsyncInt = System.Threading.Tasks.Task<int>;
using TcpStream = System.Net.Sockets.NetworkStream;
using TextEncoding = System.Text.Encoding;

namespace Bussen
{
    public class NetworkQuestion : Question
    {
        private TcpStream stream;
        private const byte EOL = (byte)10;
        private TextEncoding Encoding = TextEncoding.UTF8;
        private static TcpListener? server;

        public NetworkQuestion(TcpStream stream)
        {
            this.stream = stream;
        }

        public override void Tell(string text, bool newlineBefore, bool newlineAfter)
        {
            if (newlineBefore)
            {
                stream.WriteByte(EOL);
            }

            stream.Write(Encoding.GetBytes(text));

            if (newlineAfter)
            {
                stream.WriteByte(EOL);
            }
        }

        protected override async AsyncNullString Ask(string question)
        {
            const int bufferSize = 10240;
            byte[] buffer = new byte[bufferSize];
            Tell(question + " ", true, false);
            int inputSize = await stream.ReadAsync(buffer, 0, bufferSize);
            return Encoding.GetString(buffer, 0, inputSize);
        }
        
        public static ConnectionGenerator Listen(int port)
        {
            server = new(IPAddress.Any, port);
            server.Start();
            while (server != null)
            {
                yield return server.AcceptTcpClient();
            }
        }

        public static UiGenerator ListenUi(int port)
        {
            foreach(var connection in Listen(port))
            {
                yield return new NetworkQuestion(connection.GetStream());
            }
        }

        public static void stop()
        {
            server.Stop();
            server = null;
        }
    }
}