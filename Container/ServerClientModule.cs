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
    public enum NetworkStates
    {
        Unknown = 0,
        Initialized,
        WaitingForConnection,
        Connected,
        LostConnection,
        Disconnected,
        NetworkError,
        FailedToConnect
    }
    public class ServerModel
    {

        public delegate void OnDataReceivedDelegate(string data, TransmitedDataType dataType);
        public event OnDataReceivedDelegate OnDataReceived;
        public delegate void OnClientConnectedDelegate(Socket client);
        public event OnClientConnectedDelegate OnClientConnected;
        public delegate void OnClientDisconnectedDelegate();
        public event OnClientDisconnectedDelegate OnClientDisconnected;

        public string IP = "127.0.0.1";
        public int port = 8001;
        public int BufferSize = 2000;
        public bool AutoStart = true;

        public NetworkStates State = NetworkStates.Unknown;
        public NetworkStates ClientState = NetworkStates.Unknown;

        private Exception networkError;
        public Exception NetworkError { get { return networkError; } }

        IPAddress ipAd;
        TcpListener server;
        Socket client;
        bool stopReceiver = false;
        string latestReceivedData = null;

        bool connected = false;
        public bool IsConnected { get { return connected; } }

        public ServerModel(string IP = null, int port = 8001, int bufferSize = 2000)
        {
            if (string.IsNullOrWhiteSpace(IP))
                IP = LocalHost.MyLocalIPAddress();

            this.BufferSize = bufferSize;
            this.IP = IP;
            this.port = port;
            ipAd = IPAddress.Parse(IP);
            server = new TcpListener(ipAd, port);
            
            State = NetworkStates.Initialized;
        }

        ~ServerModel()
        {
            if (connected)
            {
                StopReceiving();
                Disconnect();
                server = null;
                client = null;
            }
        }
        public bool DropAndReset()
        {
            if(State != NetworkStates.WaitingForConnection)
            {
                
                if (client != null && client.Connected)
                    client.Disconnect(true);
                client = null;
                connected = false;
                stopReceiver = true;
                server.Stop();
                System.Threading.Thread.Sleep(2000);
                //server = new TcpListener(ipAd, port);
                AcceptConnection();
                return true;
            }
            return false;
        }

        public void AcceptConnection()
        {

            server.Start();
            State = NetworkStates.WaitingForConnection;
            server.BeginAcceptSocket(AcceptSocket, server);
        }

        public void Disconnect()
        {
            if (connected)
            {
                SendData(NetworkStates.Disconnected.ToString(), TransmitedDataType.Status);
                client.Disconnect(false);
                server.Stop();
                client.Close();
                client = null;
                connected = false;
                  StopReceiving();
            }
            State = NetworkStates.Disconnected;
        }

        void AcceptSocket(IAsyncResult result)
        {
            try
            {
                var server = ((TcpListener)result.AsyncState);
                client = ((TcpListener)result.AsyncState).EndAcceptSocket(result);
                Console.WriteLine("Connection accepted from " + client.RemoteEndPoint);
                connected = true;
                if (OnClientConnected != null)
                    OnClientConnected.Invoke(client);

                State = NetworkStates.Connected;
                networkError = null;
                SendData(State.ToString(), TransmitedDataType.Status);
                if (AutoStart)
                    StartReceiving();
            }
            catch (Exception e)
            {
                State = NetworkStates.NetworkError;
                networkError = e;
            }
        }

        public void StartReceiving()
        {
            if (!connected)
                return;
            try
            {
                var readEvent = new AutoResetEvent(false);
                var recieveArgs = new SocketAsyncEventArgs()
                {
                    UserToken = readEvent
                };
                byte[] buffer = new byte[BufferSize];
                recieveArgs.SetBuffer(buffer, 0, BufferSize);
                recieveArgs.Completed += recieveArgs_Completed;
                stopReceiver = false;
                do
                {
                    client.ReceiveAsync(recieveArgs);
                    readEvent.WaitOne();//Wait for recieve
                   if (!IsConnected)
                        stopReceiver = true;
                } while (!stopReceiver);
                recieveArgs.Completed -= recieveArgs_Completed;
            }
            catch (Exception e)
            {
                State = NetworkStates.NetworkError;
                networkError = e;
                SendData(State.ToString(), TransmitedDataType.Status);
            }

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
                    latestReceivedData.StartsWith("C104") ? TransmitedDataType.Headers : latestReceivedData.StartsWith("C105") ? TransmitedDataType.RawData : TransmitedDataType.Unknown;

                if (datatype != TransmitedDataType.Unknown)
                    latestReceivedData = latestReceivedData.Substring(4);

                if (datatype == TransmitedDataType.Status)
                {

                    var stateID = Enum.Parse(typeof(NetworkStates), latestReceivedData);

                    if (stateID is NetworkStates)
                    {
                        ClientState = (NetworkStates)stateID;
                        if (ClientState == NetworkStates.Disconnected)
                        {
                            StopReceiving();
                            DropAndReset();
                            if (OnClientDisconnected != null)
                                OnClientDisconnected.Invoke();
                        }
                        // return;
                    }
                }


            }

            if (OnDataReceived != null)
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
                try
                {
                    if (dataType != TransmitedDataType.Unknown)
                        data = "C" + (int)dataType + data;

                    if (data.Length < BufferSize)
                    {
                        data = data.PadRight(BufferSize);
                    }
                    ASCIIEncoding asen = new ASCIIEncoding();
                    client.Send(asen.GetBytes(data));
                    networkError = null;
                }
                catch (Exception e)
                {
                    networkError = e;
                    State = NetworkStates.NetworkError;

                }
            }
        }
    }

    public class ClientModel
    {
        public delegate void OnDataReceivedDelegate(string data, TransmitedDataType dataType);
        public event OnDataReceivedDelegate OnDataReceived;
        public delegate void OnConnectedToServerDelegate(Socket server);
        public event OnConnectedToServerDelegate OnConnectedToServer;
        public delegate void OnServerDisconnectedDelegate();
        public event OnServerDisconnectedDelegate OnServerDisconnected;

        public string IP = "127.0.0.1";
        public int port = 8001;

        public int BufferSize = 2000;
        public bool AutoStart = true;

        public NetworkStates State = NetworkStates.Unknown;
        public NetworkStates ServerState = NetworkStates.Unknown;

        private Exception networkError;
        public Exception NetworkError { get { return networkError; } }

        TcpClient client;
        bool stopReceiver = false;
        System.IO.Stream connectionStream;
        bool connected = false;
        string latestReceivedData = null;

        public bool IsConnected { get { return connected; } }

        public ClientModel(string IP = null, int port = 8001, int bufferSize = 2000)
        {
            if (string.IsNullOrWhiteSpace(IP))
                IP = LocalHost.MyLocalIPAddress();

            this.IP = IP;
            this.port = port;
            client = new TcpClient();
            BufferSize = bufferSize;
            State = NetworkStates.Initialized;
        }

         ~ClientModel()
        {
            if(connected)
            {
                StopReceiving();
                Disconnect();
                client = null;
            }
        }

        public bool ConnectToServer()
        {

            try
            {

                IAsyncResult ar = client.BeginConnect(IP, port, null, client);
                bool result = ar.AsyncWaitHandle.WaitOne(1000, false);

                if (!result || !client.Connected)
                {
                    State = NetworkStates.FailedToConnect;
                    return false;
                }
               
                //client.Connect(IP, port);
                // use the ipaddress as in the server program
                connectionStream = client.GetStream();
                Console.WriteLine("Connected");
                connected = true;
                if (OnConnectedToServer != null)
                    OnConnectedToServer.Invoke(client.Client);
                State = NetworkStates.Connected;
                networkError = null;
                SendData(State.ToString(), TransmitedDataType.Status);
                if (AutoStart)
                    StartReceiving();
                return true;
            }
            catch (System.Net.Sockets.SocketException sockEx)
            {
                networkError = sockEx;
                State = NetworkStates.NetworkError;
            }
            return false;
        }

        public void SendPositionAndOrientation(string position,string orientation)
        {
            SendData(position+"\n"+orientation, TransmitedDataType.RawData);
        }
        public void SendCommand(string command)
        {
            SendData(command, TransmitedDataType.Command);
        }
        public void SendData(string data, TransmitedDataType dataType)
        {
            if (IsConnected)
            {
                try
                {
                    if (dataType != TransmitedDataType.Unknown)
                        data = "C" + (int)dataType + data;
                    if (data.Length < BufferSize)
                    {
                        data = data.PadRight(BufferSize);
                    }
                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] ba = asen.GetBytes(data);
                    Console.WriteLine("Transmitting.....");

                    connectionStream.Write(ba, 0, ba.Length);
                    networkError = null;
                }
                catch(Exception e)
                {
                    networkError = e;
                    State = NetworkStates.NetworkError;

                }
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
                if (datatype == TransmitedDataType.Status)
                {
                    var stateID = Enum.Parse(typeof(NetworkStates), data.Substring(4));

                    if (stateID is NetworkStates)
                    {
                        ServerState = (NetworkStates)stateID;
                        if (ServerState == NetworkStates.Disconnected)
                        {
                            StopReceiving();
                            State = NetworkStates.Disconnected;
                            connected = false;
                            if (OnServerDisconnected != null)
                                OnServerDisconnected.Invoke();
                        }
                        // return;
                    }
                }
                if (datatype != TransmitedDataType.Unknown)
                    data = data.Substring(4);
            }
            latestReceivedData = data;

            if (OnDataReceived != null)
                OnDataReceived.Invoke(data, datatype);

            if (!stopReceiver)
            {
                buffer = new byte[BufferSize];
                connectionStream.BeginRead(buffer, 0, BufferSize, dataRead, buffer);
            }

        }
        public void StartReceiving()
        {
            if (!connected)
                return;
            try
            {
                stopReceiver = false;
                byte[] buffer = new byte[BufferSize];

                connectionStream.BeginRead(buffer, 0, BufferSize, dataRead, buffer);
                networkError = null;
            }
            catch(Exception e)
            {
                networkError = e;
                State = NetworkStates.NetworkError;
                SendData(State.ToString(), TransmitedDataType.Status);
            }

        }

        public void StopReceiving()
        {
            try
            {
                stopReceiver = true;
                connectionStream.EndRead(null);
            }
            catch(Exception e)
            {
                networkError = e;
                State = NetworkStates.NetworkError;
            }
        }

        public string GetTheLatestReceivedData()
        {
            return latestReceivedData;
        }
        public void Disconnect()
        {
            if (connected)
            {
                try
                {
                    State = NetworkStates.Disconnected;
                    SendData(State.ToString(), TransmitedDataType.Status);
                    System.Threading.Thread.Sleep(2000);
                    StopReceiving();
                    client.Close();
                    //client = new TcpClient();

                    connected = false;
                    
                 
                }
                catch(Exception e)
                {
                    networkError = e;
                    State = NetworkStates.NetworkError;
                }
            }
        }
    }

    public static class LocalHost
    {
        public static string MyLocalIPAddress()
        {
            string myip = string.Empty;

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    myip = ip.ToString();
                    break;
                }
            }
            return myip;
        }
    }
}