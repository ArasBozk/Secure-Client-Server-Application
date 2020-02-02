namespace Cs432_projectStep1_client_0._01
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Port = new System.Windows.Forms.TextBox();
            this.textBox_IP = new System.Windows.Forms.TextBox();
            this.textBox_UserName = new System.Windows.Forms.TextBox();
            this.textBox_Password = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button_connect = new System.Windows.Forms.Button();
            this.button_Disconnect = new System.Windows.Forms.Button();
            this.ConsoleServer = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ClientInput_Console = new System.Windows.Forms.RichTextBox();
            this.button_sendMessage = new System.Windows.Forms.Button();
            this.button_Enroll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port:";
            // 
            // textBox_Port
            // 
            this.textBox_Port.Location = new System.Drawing.Point(93, 34);
            this.textBox_Port.Name = "textBox_Port";
            this.textBox_Port.Size = new System.Drawing.Size(100, 20);
            this.textBox_Port.TabIndex = 1;
            // 
            // textBox_IP
            // 
            this.textBox_IP.Location = new System.Drawing.Point(93, 75);
            this.textBox_IP.Name = "textBox_IP";
            this.textBox_IP.Size = new System.Drawing.Size(100, 20);
            this.textBox_IP.TabIndex = 2;
            // 
            // textBox_UserName
            // 
            this.textBox_UserName.Location = new System.Drawing.Point(93, 139);
            this.textBox_UserName.Name = "textBox_UserName";
            this.textBox_UserName.Size = new System.Drawing.Size(100, 20);
            this.textBox_UserName.TabIndex = 3;
            // 
            // textBox_Password
            // 
            this.textBox_Password.Location = new System.Drawing.Point(93, 176);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.Size = new System.Drawing.Size(100, 20);
            this.textBox_Password.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "IP:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "User Name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 179);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Password:";
            // 
            // button_connect
            // 
            this.button_connect.Location = new System.Drawing.Point(55, 217);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(75, 23);
            this.button_connect.TabIndex = 8;
            this.button_connect.Text = "Login";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // button_Disconnect
            // 
            this.button_Disconnect.Location = new System.Drawing.Point(103, 259);
            this.button_Disconnect.Name = "button_Disconnect";
            this.button_Disconnect.Size = new System.Drawing.Size(75, 23);
            this.button_Disconnect.TabIndex = 9;
            this.button_Disconnect.Text = "Disconnect";
            this.button_Disconnect.UseVisualStyleBackColor = true;
            // 
            // ConsoleServer
            // 
            this.ConsoleServer.Location = new System.Drawing.Point(302, 37);
            this.ConsoleServer.Name = "ConsoleServer";
            this.ConsoleServer.ReadOnly = true;
            this.ConsoleServer.Size = new System.Drawing.Size(261, 320);
            this.ConsoleServer.TabIndex = 10;
            this.ConsoleServer.Text = "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(302, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Server Activity:";
            // 
            // ClientInput_Console
            // 
            this.ClientInput_Console.Location = new System.Drawing.Point(34, 424);
            this.ClientInput_Console.Name = "ClientInput_Console";
            this.ClientInput_Console.Size = new System.Drawing.Size(392, 156);
            this.ClientInput_Console.TabIndex = 12;
            this.ClientInput_Console.Text = "";
            // 
            // button_sendMessage
            // 
            this.button_sendMessage.Location = new System.Drawing.Point(454, 445);
            this.button_sendMessage.Name = "button_sendMessage";
            this.button_sendMessage.Size = new System.Drawing.Size(100, 92);
            this.button_sendMessage.TabIndex = 13;
            this.button_sendMessage.Text = "Send Message";
            this.button_sendMessage.UseVisualStyleBackColor = true;
            this.button_sendMessage.Click += new System.EventHandler(this.button_sendMessage_Click);
            // 
            // button_Enroll
            // 
            this.button_Enroll.Location = new System.Drawing.Point(149, 217);
            this.button_Enroll.Name = "button_Enroll";
            this.button_Enroll.Size = new System.Drawing.Size(75, 23);
            this.button_Enroll.TabIndex = 14;
            this.button_Enroll.Text = "Enroll";
            this.button_Enroll.UseVisualStyleBackColor = true;
            this.button_Enroll.Click += new System.EventHandler(this.button_Enroll_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 592);
            this.Controls.Add(this.button_Enroll);
            this.Controls.Add(this.button_sendMessage);
            this.Controls.Add(this.ClientInput_Console);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ConsoleServer);
            this.Controls.Add(this.button_Disconnect);
            this.Controls.Add(this.button_connect);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_Password);
            this.Controls.Add(this.textBox_UserName);
            this.Controls.Add(this.textBox_IP);
            this.Controls.Add(this.textBox_Port);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Port;
        private System.Windows.Forms.TextBox textBox_IP;
        private System.Windows.Forms.TextBox textBox_UserName;
        private System.Windows.Forms.TextBox textBox_Password;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button_connect;
        private System.Windows.Forms.Button button_Disconnect;
        private System.Windows.Forms.RichTextBox ConsoleServer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox ClientInput_Console;
        private System.Windows.Forms.Button button_sendMessage;
        private System.Windows.Forms.Button button_Enroll;
    }
}

