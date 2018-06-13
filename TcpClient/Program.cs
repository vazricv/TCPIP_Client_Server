using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.IO;

namespace NoorconTcpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");

                tcpclnt.Connect("127.0.0.1", 8001);
                // use the ipaddress as in the server program

                Console.WriteLine("Connected");
                Console.Write("Enter the string to be transmitted : ");

                while(true)
                {
                    String str = Console.ReadLine();
                    Stream stm = tcpclnt.GetStream();

                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] ba = asen.GetBytes(str);
                    Console.WriteLine("Transmitting.....");

                    stm.Write(ba, 0, ba.Length);

                    byte[] bb = new byte[100];
                    int k = stm.Read(bb, 0, 100);
                    string msg = "";
                    for (int i = 0; i < k; i++)
                    {
                        msg += Convert.ToChar(bb[i]);
                        Console.Write(Convert.ToChar(bb[i]));
                    }

                    if (msg.Contains("END"))
                    {
                        tcpclnt.Close();
                        return;
                    }

                }

            }

            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }
    }
}
