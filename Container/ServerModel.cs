using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ServerModel 
{

    public delegate void OnDataRecivedDelegate(string data);
    public event OnDataRecivedDelegate OnDataRecived;
    public delegate void OnClientConnectedDelegate(Socket client);
    public event OnClientConnectedDelegate OnClientConnected;

    public int BufferSize = 1500;
    IPAddress ipAd;
    TcpListener server;
    Socket client;
    bool stopReciver = false;
    string latestRecivedData = null;

    bool connected = false;
    public bool IsConnected { get { return connected; } }

    public ServerModel(string IP = "127.0.0.1", int port = 8002)
    {
        ipAd = IPAddress.Parse(IP);
        server = new TcpListener(ipAd, port);
    }

    public void AcceptConnection(int bufferSize = 1500)
    {
        this.BufferSize = bufferSize;
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
        var are = (AutoResetEvent)e.UserToken;
        are.Set();

        latestRecivedData = "";
        for (int i = 0; i < e.BytesTransferred; i++)
            latestRecivedData += Convert.ToChar(e.Buffer[i]);

        OnDataRecived.Invoke(latestRecivedData);
    }

    public string GetTheLatestRecivedData()
    {
        return latestRecivedData;
    }

    public void SendData(string data)
    {
        if(connected)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            client.Send(asen.GetBytes(data));
        }
    }
}
