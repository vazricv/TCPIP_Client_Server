using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RightMechanics
{
    //data send and recived enum type
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

        public delegate void OnDataRecivedDelegate(string data, TransmitedDataType dataType);
        public event OnDataRecivedDelegate OnDataRecived;
        public delegate void OnClientConnectedDelegate(Socket client);
        public event OnClientConnectedDelegate OnClientConnected;


        public string IP = "127.0.0.1";
        public int port = 8001;
        public int BufferSize = 2000;

        IPAddress ipAd;
        TcpListener server;
        Socket client;
        bool stopReciver = false;
        string latestRecivedData = null;

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

        public void StartReciving()
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
            } while (!stopReciver);
            recieveArgs.Completed -= recieveArgs_Completed;

        }

        public void StopReciving()
        {
            stopReciver = true;
        }

        void recieveArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            TransmitedDataType datatype = TransmitedDataType.Unknown;

            var are = (AutoResetEvent)e.UserToken;
            are.Set();

            latestRecivedData = "";
            for (int i = 0; i < e.BytesTransferred; i++)
                latestRecivedData += Convert.ToChar(e.Buffer[i]);
            latestRecivedData = latestRecivedData.TrimEnd();

            if (latestRecivedData.Length > 4 && latestRecivedData.StartsWith("C"))
            {
                datatype = latestRecivedData.StartsWith("C101") ? TransmitedDataType.Message : latestRecivedData.StartsWith("C102") ?
                    TransmitedDataType.Status : latestRecivedData.StartsWith("C103") ? TransmitedDataType.Command :
                    latestRecivedData.StartsWith("C104") ? TransmitedDataType.Headers : latestRecivedData.StartsWith("C105")? TransmitedDataType.RawData: TransmitedDataType.Unknown;
                if (datatype != TransmitedDataType.Unknown)
                    latestRecivedData = latestRecivedData.Substring(4);
            }

            OnDataRecived.Invoke(latestRecivedData, datatype);
        }

        public string GetTheLatestRecivedData()
        {
            return latestRecivedData;
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
        public delegate void OnDataRecivedDelegate(string data, TransmitedDataType dataType);
        public event OnDataRecivedDelegate OnDataRecived;
        public delegate void OnConnectedToServerDelegate(Socket server);
        public event OnConnectedToServerDelegate OnConnectedToServer;

        public string IP = "127.0.0.1";
        public int port = 8001;

        public int BufferSize = 2000;
        TcpClient client;
        bool stopReciver = false;
        System.IO.Stream connectionStream;
        bool connected = false;
        string latestRecivedData = null;

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
            latestRecivedData = data;

            OnDataRecived.Invoke(data, datatype);

            //if (!stopReciver)
            //{
            //    buffer = new byte[BufferSize];
            //    connectionStream.BeginRead(buffer, 0, BufferSize, dataRead, buffer);
            //}

        }
        public void StartReciving()
        {
            if (!connected)
                return;
            stopReciver = false;
            byte[] buffer = new byte[BufferSize];

            connectionStream.BeginRead(buffer, 0, BufferSize, dataRead, buffer);

        }

        public void StopReciving()
        {
            stopReciver = true;
        }

        public string GetTheLatestRecivedData()
        {
            return latestRecivedData;
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