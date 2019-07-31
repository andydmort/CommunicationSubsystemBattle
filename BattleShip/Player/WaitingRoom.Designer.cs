namespace Player
{
    partial class WaitingRoom
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
            this.ConnectBox = new System.Windows.Forms.GroupBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.PlayerNameTextBox = new System.Windows.Forms.TextBox();
            this.PlayerNameLabel = new System.Windows.Forms.Label();
            this.PortTextBox = new System.Windows.Forms.TextBox();
            this.PortLabel = new System.Windows.Forms.Label();
            this.IPAddressTextBox = new System.Windows.Forms.TextBox();
            this.IPAddressLabel = new System.Windows.Forms.Label();
            this.WaitingLabel = new System.Windows.Forms.Label();
            this.ConnectBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectBox
            // 
            this.ConnectBox.Controls.Add(this.ConnectButton);
            this.ConnectBox.Controls.Add(this.PlayerNameTextBox);
            this.ConnectBox.Controls.Add(this.PlayerNameLabel);
            this.ConnectBox.Controls.Add(this.PortTextBox);
            this.ConnectBox.Controls.Add(this.PortLabel);
            this.ConnectBox.Controls.Add(this.IPAddressTextBox);
            this.ConnectBox.Controls.Add(this.IPAddressLabel);
            this.ConnectBox.Location = new System.Drawing.Point(15, 12);
            this.ConnectBox.Name = "ConnectBox";
            this.ConnectBox.Size = new System.Drawing.Size(543, 159);
            this.ConnectBox.TabIndex = 0;
            this.ConnectBox.TabStop = false;
            this.ConnectBox.Text = "Connect";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(135, 108);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(242, 35);
            this.ConnectButton.TabIndex = 6;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // PlayerNameTextBox
            // 
            this.PlayerNameTextBox.Location = new System.Drawing.Point(123, 67);
            this.PlayerNameTextBox.Name = "PlayerNameTextBox";
            this.PlayerNameTextBox.Size = new System.Drawing.Size(133, 26);
            this.PlayerNameTextBox.TabIndex = 5;
            // 
            // PlayerNameLabel
            // 
            this.PlayerNameLabel.AutoSize = true;
            this.PlayerNameLabel.Location = new System.Drawing.Point(15, 67);
            this.PlayerNameLabel.Name = "PlayerNameLabel";
            this.PlayerNameLabel.Size = new System.Drawing.Size(102, 20);
            this.PlayerNameLabel.TabIndex = 4;
            this.PlayerNameLabel.Text = "Player Name:";
            // 
            // PortTextBox
            // 
            this.PortTextBox.Location = new System.Drawing.Point(322, 29);
            this.PortTextBox.Name = "PortTextBox";
            this.PortTextBox.Size = new System.Drawing.Size(149, 26);
            this.PortTextBox.TabIndex = 3;
            // 
            // PortLabel
            // 
            this.PortLabel.AutoSize = true;
            this.PortLabel.Location = new System.Drawing.Point(283, 32);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(42, 20);
            this.PortLabel.TabIndex = 2;
            this.PortLabel.Text = "Port:";
            // 
            // IPAddressTextBox
            // 
            this.IPAddressTextBox.Location = new System.Drawing.Point(112, 29);
            this.IPAddressTextBox.Name = "IPAddressTextBox";
            this.IPAddressTextBox.Size = new System.Drawing.Size(149, 26);
            this.IPAddressTextBox.TabIndex = 1;
            // 
            // IPAddressLabel
            // 
            this.IPAddressLabel.AutoSize = true;
            this.IPAddressLabel.Location = new System.Drawing.Point(15, 32);
            this.IPAddressLabel.Name = "IPAddressLabel";
            this.IPAddressLabel.Size = new System.Drawing.Size(91, 20);
            this.IPAddressLabel.TabIndex = 0;
            this.IPAddressLabel.Text = "IP Address:";
            // 
            // WaitingLabel
            // 
            this.WaitingLabel.AutoSize = true;
            this.WaitingLabel.Location = new System.Drawing.Point(237, 258);
            this.WaitingLabel.Name = "WaitingLabel";
            this.WaitingLabel.Size = new System.Drawing.Size(74, 20);
            this.WaitingLabel.TabIndex = 1;
            this.WaitingLabel.Text = "Waiting...";
            this.WaitingLabel.Visible = false;
            // 
            // WaitingRoom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 377);
            this.Controls.Add(this.WaitingLabel);
            this.Controls.Add(this.ConnectBox);
            this.Name = "WaitingRoom";
            this.Text = "Form1";
            this.ConnectBox.ResumeLayout(false);
            this.ConnectBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WaitingRoom_FormClosing);
        }

        #endregion

        private System.Windows.Forms.GroupBox ConnectBox;
        private System.Windows.Forms.TextBox PortTextBox;
        private System.Windows.Forms.Label PortLabel;
        private System.Windows.Forms.TextBox IPAddressTextBox;
        private System.Windows.Forms.Label IPAddressLabel;
        private System.Windows.Forms.TextBox PlayerNameTextBox;
        private System.Windows.Forms.Label PlayerNameLabel;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Label WaitingLabel;
    }
}

