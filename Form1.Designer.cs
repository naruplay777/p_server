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
            this.listClients = new System.Windows.Forms.ListBox();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listClients
            // 
            this.listClients.FormattingEnabled = true;
            this.listClients.Location = new System.Drawing.Point(11, 74);
            this.listClients.Name = "listClients";
            this.listClients.Size = new System.Drawing.Size(366, 56);
            this.listClients.TabIndex = 0;
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(11, 136);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(366, 211);
            this.txtLog.TabIndex = 1;
            this.txtLog.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(154, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // btnStartServer
            // 
            this.btnStartServer.Location = new System.Drawing.Point(11, 33);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(366, 35);
            this.btnStartServer.TabIndex = 3;
            this.btnStartServer.Text = "button1";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 360);
            this.Controls.Add(this.btnStartServer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.listClients);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listClients;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStartServer;
    }
}

