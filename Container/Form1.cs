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
using System.Messaging;
using System.Linq;
using System.Net;
using System.Net.Sockets;

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

        private MessagingQueueManager RMMessaging = new MessagingQueueManager(); 
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

            RMMessaging.CreateMessagingQueue("RMData");

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
            RMMessaging.SendMessage(txtMessage.Text);
            txtMessage.Text = "";
            lbmsgCount.Text = RMMessaging.MessageCounts().ToString();
        }

        private void brnRecive_Click(object sender, EventArgs e)
        {
            txtMessage.Text = RMMessaging.ReceiveMessage();
            lbmsgCount.Text = RMMessaging.MessageCounts().ToString();
        }
        ServerModel myServer;
        ClientModel client;
        private void btnServer_Click(object sender, EventArgs e)
        {

            myServer = new ServerModel();
            myServer.AcceptConnection();
            myServer.OnDataRecived += MyServer_OnDataRecived;
            myServer.OnClientConnected += MyServer_OnClientConnected;

            btnSend.Visible = false;
            btnClient.Visible = false;
         }

        private void MyServer_OnClientConnected(Socket client)
        {
            myServer.StartReciving();
        }

        private void MyServer_OnDataRecived(string data)
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
            client = new ClientModel();
            client.OnConnectedToServer += Client_OnConnectedToServer;
            client.OnDataRecived += Client_OnDataRecived;
            client.ConnectToServer();

            btnsendfromserver.Visible = false;
            btnServer.Visible = false;
        }

        private void Client_OnDataRecived(string data)
        {
            txtMessage.Invoke((MethodInvoker)(() =>
            {
                txtMessage.Text = data;
            }));
        }

        private void Client_OnConnectedToServer(Socket server)
        {
            client.StartReciving();
        }

        private void btnSendData_Click(object sender, EventArgs e)
        {
            client.SendData(txtMessage.Text);
            txtMessage.Text = "";
        }

        private void btnresume_Click(object sender, EventArgs e)
        {
            myServer.SendData(txtMessage.Text);
            txtMessage.Text = "";
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            client.Disconnect();
        }

        private void btnCloseServer_Click(object sender, EventArgs e)
        {
            myServer.Disconnect();
        }
    }
}
