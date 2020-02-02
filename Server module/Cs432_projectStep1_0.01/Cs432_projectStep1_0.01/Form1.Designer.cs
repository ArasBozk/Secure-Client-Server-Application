namespace Cs432_projectStep1_0._01
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
            this.textBox_Port = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_IP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ServerConsol = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button_startServer = new System.Windows.Forms.Button();
            this.button_shutDown = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_serverPassword = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox_Port
            // 
            this.textBox_Port.Location = new System.Drawing.Point(99, 55);
            this.textBox_Port.Name = "textBox_Port";
            this.textBox_Port.Size = new System.Drawing.Size(100, 20);
            this.textBox_Port.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Port Num:";
            // 
            // textBox_IP
            // 
            this.textBox_IP.Location = new System.Drawing.Point(99, 81);
            this.textBox_IP.Name = "textBox_IP";
            this.textBox_IP.Size = new System.Drawing.Size(100, 20);
            this.textBox_IP.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "IP";
            // 
            // ServerConsol
            // 
            this.ServerConsol.Location = new System.Drawing.Point(223, 33);
            this.ServerConsol.Name = "ServerConsol";
            this.ServerConsol.Size = new System.Drawing.Size(393, 553);
            this.ServerConsol.TabIndex = 4;
            this.ServerConsol.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(223, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Server Activity:";
            // 
            // button_startServer
            // 
            this.button_startServer.Location = new System.Drawing.Point(113, 152);
            this.button_startServer.Name = "button_startServer";
            this.button_startServer.Size = new System.Drawing.Size(75, 23);
            this.button_startServer.TabIndex = 6;
            this.button_startServer.Text = "Start Server";
            this.button_startServer.UseVisualStyleBackColor = true;
            this.button_startServer.Click += new System.EventHandler(this.button_startServer_Click);
            // 
            // button_shutDown
            // 
            this.button_shutDown.Location = new System.Drawing.Point(113, 181);
            this.button_shutDown.Name = "button_shutDown";
            this.button_shutDown.Size = new System.Drawing.Size(75, 23);
            this.button_shutDown.TabIndex = 7;
            this.button_shutDown.Text = "Shut Down";
            this.button_shutDown.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(-1, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Server Password:";
            // 
            // textBox_serverPassword
            // 
            this.textBox_serverPassword.Location = new System.Drawing.Point(99, 117);
            this.textBox_serverPassword.Name = "textBox_serverPassword";
            this.textBox_serverPassword.Size = new System.Drawing.Size(100, 20);
            this.textBox_serverPassword.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 606);
            this.Controls.Add(this.textBox_serverPassword);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button_shutDown);
            this.Controls.Add(this.button_startServer);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ServerConsol);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_IP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_Port);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_Port;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_IP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox ServerConsol;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_startServer;
        private System.Windows.Forms.Button button_shutDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_serverPassword;
    }
}

