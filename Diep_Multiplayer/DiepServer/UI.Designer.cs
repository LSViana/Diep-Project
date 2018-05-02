namespace DiepServer
{
    partial class UI
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
            this.labelServerIP = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelUserAmount = new System.Windows.Forms.Label();
            this.panelGameUI = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server IP:";
            // 
            // labelServerIP
            // 
            this.labelServerIP.AutoSize = true;
            this.labelServerIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelServerIP.ForeColor = System.Drawing.Color.Red;
            this.labelServerIP.Location = new System.Drawing.Point(123, 9);
            this.labelServerIP.Name = "labelServerIP";
            this.labelServerIP.Size = new System.Drawing.Size(15, 13);
            this.labelServerIP.TabIndex = 0;
            this.labelServerIP.Text = "[]";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Connected Clients:";
            // 
            // labelUserAmount
            // 
            this.labelUserAmount.AutoSize = true;
            this.labelUserAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUserAmount.ForeColor = System.Drawing.Color.Red;
            this.labelUserAmount.Location = new System.Drawing.Point(123, 39);
            this.labelUserAmount.Name = "labelUserAmount";
            this.labelUserAmount.Size = new System.Drawing.Size(15, 13);
            this.labelUserAmount.TabIndex = 0;
            this.labelUserAmount.Text = "[]";
            // 
            // panelGameUI
            // 
            this.panelGameUI.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelGameUI.BackColor = System.Drawing.Color.White;
            this.panelGameUI.Location = new System.Drawing.Point(15, 67);
            this.panelGameUI.Name = "panelGameUI";
            this.panelGameUI.Size = new System.Drawing.Size(909, 497);
            this.panelGameUI.TabIndex = 2;
            // 
            // UI
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(936, 576);
            this.Controls.Add(this.panelGameUI);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelUserAmount);
            this.Controls.Add(this.labelServerIP);
            this.Controls.Add(this.label1);
            this.Name = "UI";
            this.ShowIcon = false;
            this.Text = "Server";
            this.Load += new System.EventHandler(this.UI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelServerIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelUserAmount;
        private System.Windows.Forms.Panel panelGameUI;
    }
}

