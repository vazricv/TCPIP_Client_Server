using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;
//using System.Messaging;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.IO;
using RightMechanics;

namespace Container
{
    public partial class Form1 : Form
    {
        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private Process process;
        private IntPtr unityHWND = IntPtr.Zero;

        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);

       // private MessagingQueueManager RMMessaging = new MessagingQueueManager(); 
        public Form1()
        {
            InitializeComponent();

            try
            {
                process = new Process();
                process.StartInfo.FileName = "MVNBiomechViz.exe";
                process.StartInfo.Arguments = "-parentHWND " + panel1.Handle.ToInt32() + " " + Environment.CommandLine;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                process.WaitForInputIdle();
                // Doesn't work for some reason ?!
                //unityHWND = process.MainWindowHandle;
                EnumChildWindows(panel1.Handle, WindowEnum, IntPtr.Zero);

                unityHWNDLabel.Text = ""; // "Unity HWND: 0x" + unityHWND.ToString("X8");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ".\nCheck if Container.exe is placed next to Child.exe.");
            }

            //RMMessaging.CreateMessagingQueue("RMData");

        }

        private void ActivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }

        private void DeactivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            unityHWND = hwnd;
            ActivateUnityWindow();
            return 0;
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            MoveWindow(unityHWND, 0, 0, panel1.Width, panel1.Height, true);
            ActivateUnityWindow();
        }

        // Close Unity application
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                client.Disconnect();
                client = null;
                process.CloseMainWindow();

                Thread.Sleep(1000);
                while (process.HasExited == false)
                    process.Kill();
            }
            catch (Exception)
            {

            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            ActivateUnityWindow();
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            DeactivateUnityWindow();
        }

        
        private void btnSend_Click(object sender, EventArgs e)
        {
            //RMMessaging.SendMessage(txtMessage.Text);
            //txtMessage.Text = "";
            //lbmsgCount.Text = RMMessaging.MessageCounts().ToString();
        }

        private void brnRecive_Click(object sender, EventArgs e)
        {
            //txtMessage.Text = RMMessaging.ReceiveMessage();
            //lbmsgCount.Text = RMMessaging.MessageCounts().ToString();
        }
        ServerModel myServer;
        ClientModel client;
        private void btnServer_Click(object sender, EventArgs e)
        {

            myServer = new ServerModel("127.0.0.1");
            myServer.AcceptConnection();
            myServer.OnDataReceived += MyServer_OnDataRecived;
            myServer.OnClientConnected += MyServer_OnClientConnected;

            btnSendData.Visible = false;
            btnClient.Visible = false;
         }

        private void MyServer_OnClientConnected(Socket client)
        {
            myServer.StartReceiving();
        }

        private void MyServer_OnDataRecived(string data, TransmitedDataType dataType)
        {
            Console.WriteLine("Recived :");
            Console.WriteLine(data);
            txtMessage.Invoke((MethodInvoker)(() =>
            {
                txtMessage.Text = data;
            }));
        }
        private void btnClient_Click(object sender, EventArgs e)
        {
            //client = new ClientModel("127.0.0.1");
            client = new ClientModel();
           // client.OnConnectedToServer += Client_OnConnectedToServer;
            client.OnDataReceived += Client_OnDataRecived;
            client.ConnectToServer();

            btnsendfromserver.Visible = false;
            btnServer.Visible = false;
            btnCloseServer.Visible = false;
        }

        private void Client_OnDataRecived(string data, TransmitedDataType dataType)
        {
            txtMessage.Invoke((MethodInvoker)(() =>
            {
                txtMessage.Text = data;
            }));
            if (dataType == TransmitedDataType.Command)
            {
                if (data == "stop")
                {
                    timer1.Stop();
                }
                if (data == "start" && dataIsLoaded)
                {
                    timer1.Start();
                }
            }
        }

        //private void Client_OnConnectedToServer(Socket server)
        //{
        //    client.StartReceiving();
        //}

        private void btnSendData_Click(object sender, EventArgs e)
        {
            client.SendData(txtMessage.Text, TransmitedDataType.Message);
            txtMessage.Text = "";
        }

        private void btnresume_Click(object sender, EventArgs e)
        {
            myServer.SendData(txtMessage.Text,TransmitedDataType.Message);
            txtMessage.Text = "";
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            client.Disconnect();
            timer1.Stop();
        }

        private void btnCloseServer_Click(object sender, EventArgs e)
        {
            myServer.Disconnect();
        }

        private void btnpos_Click(object sender, EventArgs e)
        {
            using (var f = new OpenFileDialog())
            {
                f.Filter = "CSV file *.CSV|*.csv";
                f.Title = "Select CSV file or Positions";
                if (f.ShowDialog() != DialogResult.Cancel)
                {
                    txtCSVPos.Text = f.FileName;
                    posData = File.ReadAllLines(txtCSVPos.Text);
                }
            }
        }

        private void btnrot_Click(object sender, EventArgs e)
        {
            using (var f = new OpenFileDialog())
            {
                f.Filter = "CSV file *.CSV|*.csv";
                f.Title = "Select CSV file or Orientations";
                if (f.ShowDialog() != DialogResult.Cancel)
                {
                    txtCSVorient.Text = f.FileName;
                    rotData = File.ReadAllLines(txtCSVorient.Text);
                }
            }
        }
        string[] posData;
        string[] rotData;
        int frameNumber = 0;
        bool dataIsLoaded = false;
        private void btnStartSimulation_Click(object sender, EventArgs e)
        {
           
           // posData = File.ReadAllLines(txtCSVPos.Text);
            //rotData = File.ReadAllLines(txtCSVPos.Text);
            frameNumber = 1;

            dataIsLoaded = true;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            frameNumber++;
            if (posData.Length <= frameNumber)
                frameNumber = 1;
            string data = posData[frameNumber] +"\n"+ rotData[frameNumber];
            txtMessage.Text = data;
            lbmsgCount.Text = data.Length.ToString();
            
            client.SendData(data, TransmitedDataType.RawData);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string data = posData[0] + "\n" + rotData[0];
            txtMessage.Text = data;
            lbmsgCount.Text = data.Length.ToString();
            client.SendData(data, TransmitedDataType.Headers);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frameNumber++;
            if (posData.Length <= frameNumber)
                frameNumber = 1;
            string data = posData[frameNumber] + "\n" + rotData[frameNumber];
            txtMessage.Text = data;
            lbmsgCount.Text = data.Length.ToString();
            
            client.SendData(data, TransmitedDataType.RawData);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Interval = Int32.Parse(textBox1.Text);
        }

        private void btnStopSimulation_Click(object sender, EventArgs e)
        {
            if (btnStopSimulation.Text.StartsWith("Stop"))
            {
                timer1.Stop();
                btnStopSimulation.Text = "Resume Simulation";
            }
            else
            {
                timer1.Start();
                btnStopSimulation.Text = "Stop Simulation";
            }
        }
    }
}
