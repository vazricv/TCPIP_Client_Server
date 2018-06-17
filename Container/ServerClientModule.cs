using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RightMechanics
{
    //data send and Received enum type
    public enum TransmitedDataType
    {
        Unknown = 0,
        Message = 101,
        Status = 102,
        Command = 103,
        Headers = 104,
        RawData = 105,
    }

    public class ServerModel
    {

        public delegate void OnDataReceivedDelegate(string data, TransmitedDataType dataType);
        public event OnDataReceivedDelegate OnDataReceived;
        public delegate void OnClientConnectedDelegate(Socket client);
        public event OnClientConnectedDelegate OnClientConnected;


        public string IP = "127.0.0.1";
        public int port = 8001;
        public int BufferSize = 2000;

        IPAddress ipAd;
        TcpListener server;
        Socket client;
        bool stopReceiver = false;
        string latestReceivedData = null;

        bool connected = false;
        public bool IsConnected { get { return connected; } }

        public ServerModel(string IP = "127.0.0.1", int port = 8001, int bufferSize = 2000)
        {
            this.BufferSize = bufferSize;
            this.IP = IP;
            this.port = port;
            ipAd = IPAddress.Parse(IP);
            server = new TcpListener(ipAd, port);
        }

        public void AcceptConnection()
        {

            server.Start();
            server.BeginAcceptSocket(AcceptSocket, server);

        }
        public void Disconnect()
        {
            if (connected)
            {
                client.Close();
                server.Stop();
                connected = false;
            }
        }

        void AcceptSocket(IAsyncResult result)
        {
            var server = ((TcpListener)result.AsyncState);
            client = ((TcpListener)result.AsyncState).EndAcceptSocket(result);
            Console.WriteLine("Connection accepted from " + client.RemoteEndPoint);
            connected = true;

            OnClientConnected.Invoke(client);
        }

        public void StartReceiving()
        {
            if (!connected)
                return;

            var readEvent = new AutoResetEvent(false);
            var recieveArgs = new SocketAsyncEventArgs()
            {
                UserToken = readEvent
            };
            byte[] buffer = new byte[BufferSize];
            recieveArgs.SetBuffer(buffer, 0, BufferSize);
            recieveArgs.Completed += recieveArgs_Completed;

            do
            {
                client.ReceiveAsync(recieveArgs);
                readEvent.WaitOne();//Wait for recieve
            } while (!stopReceiver);
            recieveArgs.Completed -= recieveArgs_Completed;

        }

        public void StopReceiving()
        {
            stopReceiver = true;
        }

        void recieveArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            TransmitedDataType datatype = TransmitedDataType.Unknown;

            var are = (AutoResetEvent)e.UserToken;
            are.Set();

            latestReceivedData = "";
            for (int i = 0; i < e.BytesTransferred; i++)
                latestReceivedData += Convert.ToChar(e.Buffer[i]);
            latestReceivedData = latestReceivedData.TrimEnd();

            if (latestReceivedData.Length > 4 && latestReceivedData.StartsWith("C"))
            {
                datatype = latestReceivedData.StartsWith("C101") ? TransmitedDataType.Message : latestReceivedData.StartsWith("C102") ?
                    TransmitedDataType.Status : latestReceivedData.StartsWith("C103") ? TransmitedDataType.Command :
                    latestReceivedData.StartsWith("C104") ? TransmitedDataType.Headers : latestReceivedData.StartsWith("C105")? TransmitedDataType.RawData: TransmitedDataType.Unknown;
                if (datatype != TransmitedDataType.Unknown)
                    latestReceivedData = latestReceivedData.Substring(4);
            }

            OnDataReceived.Invoke(latestReceivedData, datatype);
        }

        public string GetTheLatestReceivedData()
        {
            return latestReceivedData;
        }

        public void SendData(string data, TransmitedDataType dataType)
        {
            if (connected)
            {

                if (dataType != TransmitedDataType.Unknown)
                    data = "C" + (int)dataType + data;

                if (data.Length < BufferSize)
                {
                    data = data.PadRight(BufferSize);
                }
                ASCIIEncoding asen = new ASCIIEncoding();
                client.Send(asen.GetBytes(data));
            }
        }
    }

    public class ClientModel
    {
        public delegate void OnDataReceivedDelegate(string data, TransmitedDataType dataType);
        public event OnDataReceivedDelegate OnDataReceived;
        public delegate void OnConnectedToServerDelegate(Socket server);
        public event OnConnectedToServerDelegate OnConnectedToServer;

        public string IP = "127.0.0.1";
        public int port = 8001;

        public int BufferSize = 2000;
        TcpClient client;
        bool stopReceiver = false;
        System.IO.Stream connectionStream;
        bool connected = false;
        string latestReceivedData = null;

        public bool IsConnected { get { return connected; } }

        public ClientModel(string IP = "127.0.0.1", int port = 8001, int bufferSize = 2000)
        {
            this.IP = IP;
            this.port = port;
            client = new TcpClient();
            BufferSize = bufferSize;
        }

        public void ConnectToServer()
        {

            try
            {
                client.Connect(IP, port);
                // use the ipaddress as in the server program
                connectionStream = client.GetStream();
                Console.WriteLine("Connected");
                connected = true;
                OnConnectedToServer.Invoke(client.Client);

            }
            catch (Exception err)
            {
                Console.WriteLine("Error..... " + err.StackTrace);
            }
        }

        public void SendData(string data, TransmitedDataType dataType)
        {
            if (IsConnected)
            {
                if (dataType != TransmitedDataType.Unknown)
                    data = "C" + (int)dataType + data;
                if(data.Length < BufferSize)
                {
                    data = data.PadRight(BufferSize);
                }
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes(data);
                Console.WriteLine("Transmitting.....");

                connectionStream.Write(ba, 0, ba.Length);
            }
        }

        void dataRead(IAsyncResult result)
        {
            byte[] buffer = result.AsyncState as byte[];

            string data = ASCIIEncoding.ASCII.GetString(buffer, 0, buffer.Length);

            TransmitedDataType datatype = TransmitedDataType.Unknown;
            data = data.TrimEnd();

            if (data.Length > 4 && data.StartsWith("C"))
            {
                datatype = data.StartsWith("C101") ? TransmitedDataType.Message : data.StartsWith("C102") ?
                    TransmitedDataType.Status : data.StartsWith("C103") ? TransmitedDataType.Command :
                    data.StartsWith("C104") ? TransmitedDataType.Headers : data.StartsWith("C105") ? TransmitedDataType.RawData : TransmitedDataType.Unknown;
                if (datatype != TransmitedDataType.Unknown)
                    data = data.Substring(4);
            }
            latestReceivedData = data;

            OnDataReceived.Invoke(data, datatype);

            //if (!stopReceiver)
            //{
            //    buffer = new byte[BufferSize];
            //    connectionStream.BeginRead(buffer, 0, BufferSize, dataRead, buffer);
            //}

        }
        public void StartReceiving()
        {
            if (!connected)
                return;
            stopReceiver = false;
            byte[] buffer = new byte[BufferSize];

            connectionStream.BeginRead(buffer, 0, BufferSize, dataRead, buffer);

        }

        public void StopReceiving()
        {
            stopReceiver = true;
        }

        public string GetTheLatestReceivedData()
        {
            return latestReceivedData;
        }
        public void Disconnect()
        {
            if (connected)
            {
                client.Close();
                connected = false;
            }
        }
    }

}