namespace p_server
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listClients = new System.Windows.Forms.ListBox();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnOpenLogs = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.timerQuantum = new System.Windows.Forms.Timer(this.components);
            this.btnUnLock = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listClients
            // 
            this.listClients.BackColor = System.Drawing.SystemColors.Control;
            this.listClients.FormattingEnabled = true;
            this.listClients.Location = new System.Drawing.Point(12, 51);
            this.listClients.Name = "listClients";
            this.listClients.Size = new System.Drawing.Size(333, 56);
            this.listClients.TabIndex = 0;
            this.listClients.SelectedIndexChanged += new System.EventHandler(this.listClients_SelectedIndexChanged);
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.Control;
            this.txtLog.Location = new System.Drawing.Point(13, 210);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(332, 186);
            this.txtLog.TabIndex = 1;
            this.txtLog.Text = "";
            // 
            // btnStartServer
            // 
            this.btnStartServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(87)))), ((int)(((byte)(70)))));
            this.btnStartServer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnStartServer.Font = new System.Drawing.Font("Arial Rounded MT Bold", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartServer.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnStartServer.Location = new System.Drawing.Point(13, 123);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(332, 35);
            this.btnStartServer.TabIndex = 3;
            this.btnStartServer.Text = "Iniciar Servidor";
            this.btnStartServer.UseVisualStyleBackColor = false;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(57)))), ((int)(((byte)(39)))));
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(0, -1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(841, 56);
            this.panel1.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Red;
            this.label5.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Gainsboro;
            this.label5.Location = new System.Drawing.Point(169, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 18);
            this.label5.TabIndex = 12;
            this.label5.Text = "Inactivo";
            // 
            // panel4
            // 
            this.panel4.Location = new System.Drawing.Point(545, 56);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(28, 281);
            this.panel4.TabIndex = 11;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(60)))), ((int)(((byte)(27)))));
            this.panel3.Location = new System.Drawing.Point(532, 56);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(40, 406);
            this.panel3.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(60)))), ((int)(((byte)(27)))));
            this.label3.Font = new System.Drawing.Font("Arial Rounded MT Bold", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label3.Location = new System.Drawing.Point(12, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 28);
            this.label3.TabIndex = 0;
            this.label3.Text = "SERVIDOR";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(184)))), ((int)(((byte)(176)))));
            this.panel2.Controls.Add(this.btnUnLock);
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.btnOpenLogs);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.btnStartServer);
            this.panel2.Controls.Add(this.txtLog);
            this.panel2.Controls.Add(this.listClients);
            this.panel2.Location = new System.Drawing.Point(0, 55);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(604, 406);
            this.panel2.TabIndex = 7;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(57)))), ((int)(((byte)(39)))));
            this.panel5.Location = new System.Drawing.Point(545, -4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(41, 410);
            this.panel5.TabIndex = 11;
            // 
            // btnOpenLogs
            // 
            this.btnOpenLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(87)))), ((int)(((byte)(70)))));
            this.btnOpenLogs.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOpenLogs.Font = new System.Drawing.Font("Arial Rounded MT Bold", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenLogs.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOpenLogs.Location = new System.Drawing.Point(395, 56);
            this.btnOpenLogs.Name = "btnOpenLogs";
            this.btnOpenLogs.Size = new System.Drawing.Size(112, 43);
            this.btnOpenLogs.TabIndex = 10;
            this.btnOpenLogs.Text = "Abrir Logs";
            this.btnOpenLogs.UseVisualStyleBackColor = false;
            this.btnOpenLogs.Click += new System.EventHandler(this.btnOpenLogs_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(14, 180);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(207, 18);
            this.label4.TabIndex = 5;
            this.label4.Text = "Información del Servidor ";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(10, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "Conectados";
            // 
            // timerQuantum
            // 
            this.timerQuantum.Enabled = true;
            this.timerQuantum.Interval = 1000;
            this.timerQuantum.Tick += new System.EventHandler(this.timerQuantum_Tick);
            // 
            // btnUnLock
            // 
            this.btnUnLock.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(87)))), ((int)(((byte)(70)))));
            this.btnUnLock.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnUnLock.Font = new System.Drawing.Font("Arial Rounded MT Bold", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUnLock.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUnLock.Location = new System.Drawing.Point(395, 123);
            this.btnUnLock.Name = "btnUnLock";
            this.btnUnLock.Size = new System.Drawing.Size(112, 43);
            this.btnUnLock.TabIndex = 12;
            this.btnUnLock.Text = "Abrir Logs";
            this.btnUnLock.UseVisualStyleBackColor = false;
            this.btnUnLock.Click += new System.EventHandler(this.btnUnLock_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 461);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listClients;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnOpenLogs;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer timerQuantum;
        private System.Windows.Forms.Button btnUnLock;
    }
}

