using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class ClientModel
{
    public delegate void OnDataRecivedDelegate(string data);
    public event OnDataRecivedDelegate OnDataRecived;
    public delegate void OnConnectedToServerDelegate(Socket server);
    public event OnConnectedToServerDelegate OnConnectedToServer;

    public int BufferSize = 100;
    TcpClient client;
    bool stopReciver = false;
    System.IO.Stream connectionStream;
    bool connected = false;
    string latestRecivedData = null;

    public bool IsConnected { get { return connected; } }

    public ClientModel(int bufferSize = 100)
    {
        client = new TcpClient();
        BufferSize = bufferSize;
    }

    public void ConnectToServer(string IP = "127.0.0.1", int port = 8001)
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


    public void SendData(string data)
    {

        ASCIIEncoding asen = new ASCIIEncoding();
        byte[] ba = asen.GetBytes(data);
        Console.WriteLine("Transmitting.....");

        connectionStream.Write(ba, 0, ba.Length);
    }

    void dataRead(IAsyncResult result)
    {
        byte[] buffer = result.AsyncState as byte[];

        string data = ASCIIEncoding.ASCII.GetString(buffer, 0, buffer.Length);

        OnDataRecived.Invoke(data);

        if (!stopReciver)
        {
            buffer = new byte[BufferSize];
            connectionStream.BeginRead(buffer, 0, BufferSize, dataRead, buffer);
        }

    }
    public void StartReciving()
    {
        if (!connected)
            return;
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
