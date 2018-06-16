namespace Container
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnStartSimulation = new System.Windows.Forms.Button();
            this.btnrot = new System.Windows.Forms.Button();
            this.btnpos = new System.Windows.Forms.Button();
            this.txtCSVorient = new System.Windows.Forms.TextBox();
            this.txtCSVPos = new System.Windows.Forms.TextBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnCloseServer = new System.Windows.Forms.Button();
            this.btnsendfromserver = new System.Windows.Forms.Button();
            this.btnSendData = new System.Windows.Forms.Button();
            this.btnClient = new System.Windows.Forms.Button();
            this.btnServer = new System.Windows.Forms.Button();
            this.lbmsgCount = new System.Windows.Forms.Label();
            this.brnRecive = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.unityHWNDLabel = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new Container.SelectablePanel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.button3);
            this.splitContainer1.Panel2.Controls.Add(this.textBox1);
            this.splitContainer1.Panel2.Controls.Add(this.button2);
            this.splitContainer1.Panel2.Controls.Add(this.button1);
            this.splitContainer1.Panel2.Controls.Add(this.btnStartSimulation);
            this.splitContainer1.Panel2.Controls.Add(this.btnrot);
            this.splitContainer1.Panel2.Controls.Add(this.btnpos);
            this.splitContainer1.Panel2.Controls.Add(this.txtCSVorient);
            this.splitContainer1.Panel2.Controls.Add(this.txtCSVPos);
            this.splitContainer1.Panel2.Controls.Add(this.txtMessage);
            this.splitContainer1.Panel2.Controls.Add(this.btnDisconnect);
            this.splitContainer1.Panel2.Controls.Add(this.btnCloseServer);
            this.splitContainer1.Panel2.Controls.Add(this.btnsendfromserver);
            this.splitContainer1.Panel2.Controls.Add(this.btnSendData);
            this.splitContainer1.Panel2.Controls.Add(this.btnClient);
            this.splitContainer1.Panel2.Controls.Add(this.btnServer);
            this.splitContainer1.Panel2.Controls.Add(this.lbmsgCount);
            this.splitContainer1.Panel2.Controls.Add(this.brnRecive);
            this.splitContainer1.Panel2.Controls.Add(this.btnSend);
            this.splitContainer1.Panel2.Controls.Add(this.unityHWNDLabel);
            this.splitContainer1.Size = new System.Drawing.Size(1211, 588);
            this.splitContainer1.SplitterDistance = 705;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 1;
            // 
            // btnStartSimulation
            // 
            this.btnStartSimulation.Location = new System.Drawing.Point(214, 364);
            this.btnStartSimulation.Name = "btnStartSimulation";
            this.btnStartSimulation.Size = new System.Drawing.Size(132, 23);
            this.btnStartSimulation.TabIndex = 16;
            this.btnStartSimulation.Text = "Start Simulation";
            this.btnStartSimulation.UseVisualStyleBackColor = true;
            this.btnStartSimulation.Click += new System.EventHandler(this.btnStartSimulation_Click);
            // 
            // btnrot
            // 
            this.btnrot.Location = new System.Drawing.Point(427, 336);
            this.btnrot.Name = "btnrot";
            this.btnrot.Size = new System.Drawing.Size(42, 23);
            this.btnrot.TabIndex = 15;
            this.btnrot.Text = "...";
            this.btnrot.UseVisualStyleBackColor = true;
            this.btnrot.Click += new System.EventHandler(this.btnrot_Click);
            // 
            // btnpos
            // 
            this.btnpos.Location = new System.Drawing.Point(427, 306);
            this.btnpos.Name = "btnpos";
            this.btnpos.Size = new System.Drawing.Size(42, 23);
            this.btnpos.TabIndex = 14;
            this.btnpos.Text = "...";
            this.btnpos.UseVisualStyleBackColor = true;
            this.btnpos.Click += new System.EventHandler(this.btnpos_Click);
            // 
            // txtCSVorient
            // 
            this.txtCSVorient.Location = new System.Drawing.Point(55, 336);
            this.txtCSVorient.Name = "txtCSVorient";
            this.txtCSVorient.Size = new System.Drawing.Size(365, 22);
            this.txtCSVorient.TabIndex = 13;
            // 
            // txtCSVPos
            // 
            this.txtCSVPos.Location = new System.Drawing.Point(55, 308);
            this.txtCSVPos.Name = "txtCSVPos";
            this.txtCSVPos.Size = new System.Drawing.Size(365, 22);
            this.txtCSVPos.TabIndex = 12;
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.Location = new System.Drawing.Point(68, 38);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(375, 110);
            this.txtMessage.TabIndex = 2;
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(346, 259);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(99, 23);
            this.btnDisconnect.TabIndex = 11;
            this.btnDisconnect.Text = "disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnCloseServer
            // 
            this.btnCloseServer.Location = new System.Drawing.Point(134, 259);
            this.btnCloseServer.Name = "btnCloseServer";
            this.btnCloseServer.Size = new System.Drawing.Size(99, 23);
            this.btnCloseServer.TabIndex = 10;
            this.btnCloseServer.Text = "disconnect";
            this.btnCloseServer.UseVisualStyleBackColor = true;
            this.btnCloseServer.Click += new System.EventHandler(this.btnCloseServer_Click);
            // 
            // btnsendfromserver
            // 
            this.btnsendfromserver.Location = new System.Drawing.Point(144, 215);
            this.btnsendfromserver.Name = "btnsendfromserver";
            this.btnsendfromserver.Size = new System.Drawing.Size(75, 23);
            this.btnsendfromserver.TabIndex = 9;
            this.btnsendfromserver.Text = "send";
            this.btnsendfromserver.UseVisualStyleBackColor = true;
            this.btnsendfromserver.Click += new System.EventHandler(this.btnresume_Click);
            // 
            // btnSendData
            // 
            this.btnSendData.Location = new System.Drawing.Point(358, 215);
            this.btnSendData.Name = "btnSendData";
            this.btnSendData.Size = new System.Drawing.Size(75, 23);
            this.btnSendData.TabIndex = 8;
            this.btnSendData.Text = "Send data";
            this.btnSendData.UseVisualStyleBackColor = true;
            this.btnSendData.Click += new System.EventHandler(this.btnSendData_Click);
            // 
            // btnClient
            // 
            this.btnClient.Location = new System.Drawing.Point(295, 162);
            this.btnClient.Name = "btnClient";
            this.btnClient.Size = new System.Drawing.Size(152, 23);
            this.btnClient.TabIndex = 7;
            this.btnClient.Text = "Connect to server";
            this.btnClient.UseVisualStyleBackColor = true;
            this.btnClient.Click += new System.EventHandler(this.btnClient_Click);
            // 
            // btnServer
            // 
            this.btnServer.Location = new System.Drawing.Point(67, 162);
            this.btnServer.Name = "btnServer";
            this.btnServer.Size = new System.Drawing.Size(152, 23);
            this.btnServer.TabIndex = 6;
            this.btnServer.Text = "Be The Server";
            this.btnServer.UseVisualStyleBackColor = true;
            this.btnServer.Click += new System.EventHandler(this.btnServer_Click);
            // 
            // lbmsgCount
            // 
            this.lbmsgCount.AutoSize = true;
            this.lbmsgCount.Location = new System.Drawing.Point(403, 11);
            this.lbmsgCount.Name = "lbmsgCount";
            this.lbmsgCount.Size = new System.Drawing.Size(46, 17);
            this.lbmsgCount.TabIndex = 5;
            this.lbmsgCount.Text = "label1";
            // 
            // brnRecive
            // 
            this.brnRecive.Location = new System.Drawing.Point(68, 125);
            this.brnRecive.Name = "brnRecive";
            this.brnRecive.Size = new System.Drawing.Size(75, 23);
            this.brnRecive.TabIndex = 4;
            this.brnRecive.Text = "Recive";
            this.brnRecive.UseVisualStyleBackColor = true;
            this.brnRecive.Click += new System.EventHandler(this.brnRecive_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(370, 125);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // unityHWNDLabel
            // 
            this.unityHWNDLabel.AutoSize = true;
            this.unityHWNDLabel.Location = new System.Drawing.Point(4, 11);
            this.unityHWNDLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.unityHWNDLabel.Name = "unityHWNDLabel";
            this.unityHWNDLabel.Size = new System.Drawing.Size(46, 17);
            this.unityHWNDLabel.TabIndex = 1;
            this.unityHWNDLabel.Text = "label2";
            // 
            // timer1
            // 
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(55, 364);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(132, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "Send Headers";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(214, 411);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(132, 23);
            this.button2.TabIndex = 18;
            this.button2.Text = "send next";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(705, 588);
            this.panel1.TabIndex = 1;
            this.panel1.TabStop = true;
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(55, 453);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 22);
            this.textBox1.TabIndex = 19;
            this.textBox1.Text = "10";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(176, 451);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 20;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1211, 588);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Noorcon MVN Biomech Test";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Label unityHWNDLabel;
		private SelectablePanel panel1;
        private System.Windows.Forms.Button brnRecive;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label lbmsgCount;
        private System.Windows.Forms.Button btnClient;
        private System.Windows.Forms.Button btnServer;
        private System.Windows.Forms.Button btnSendData;
        private System.Windows.Forms.Button btnsendfromserver;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnCloseServer;
        private System.Windows.Forms.Button btnStartSimulation;
        private System.Windows.Forms.Button btnrot;
        private System.Windows.Forms.Button btnpos;
        private System.Windows.Forms.TextBox txtCSVorient;
        private System.Windows.Forms.TextBox txtCSVPos;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox1;
    }
}

