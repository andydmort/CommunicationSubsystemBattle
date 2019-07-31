namespace Player
{
    partial class PlanningScreen
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
            this.ReadyButton = new System.Windows.Forms.Button();
            this.PlanningTable = new System.Windows.Forms.TableLayoutPanel();
            this.CarrierRadio = new System.Windows.Forms.RadioButton();
            this.BattleshipRadio = new System.Windows.Forms.RadioButton();
            this.CruiserRadio = new System.Windows.Forms.RadioButton();
            this.SubRadio = new System.Windows.Forms.RadioButton();
            this.DestroyerRadio = new System.Windows.Forms.RadioButton();
            this.HorizontalRadio = new System.Windows.Forms.RadioButton();
            this.VerticalRadio = new System.Windows.Forms.RadioButton();
            this.ShipsBox = new System.Windows.Forms.GroupBox();
            this.DestroyerCheck = new System.Windows.Forms.Label();
            this.SubCheck = new System.Windows.Forms.Label();
            this.CruiserCheck = new System.Windows.Forms.Label();
            this.BattleshipCheck = new System.Windows.Forms.Label();
            this.CarrierCheck = new System.Windows.Forms.Label();
            this.OrientationBox = new System.Windows.Forms.GroupBox();
            this.TLLabel = new System.Windows.Forms.Label();
            this.SYBLabel = new System.Windows.Forms.Label();
            this.TimeLeftLabel = new System.Windows.Forms.Label();
            this.ShipsBox.SuspendLayout();
            this.OrientationBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ReadyButton
            // 
            this.ReadyButton.Enabled = false;
            this.ReadyButton.Location = new System.Drawing.Point(16, 475);
            this.ReadyButton.Name = "ReadyButton";
            this.ReadyButton.Size = new System.Drawing.Size(251, 33);
            this.ReadyButton.TabIndex = 0;
            this.ReadyButton.Text = "Ready";
            this.ReadyButton.UseVisualStyleBackColor = true;
            this.ReadyButton.Click += new System.EventHandler(this.ReadyButton_Click);
            // 
            // PlanningTable
            // 
            this.PlanningTable.ColumnCount = 10;
            this.PlanningTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.Location = new System.Drawing.Point(276, 12);
            this.PlanningTable.Name = "PlanningTable";
            this.PlanningTable.RowCount = 10;
            this.PlanningTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.PlanningTable.Size = new System.Drawing.Size(500, 500);
            this.PlanningTable.TabIndex = 1;
            // 
            // CarrierRadio
            // 
            this.CarrierRadio.AutoSize = true;
            this.CarrierRadio.Location = new System.Drawing.Point(13, 27);
            this.CarrierRadio.Name = "CarrierRadio";
            this.CarrierRadio.Size = new System.Drawing.Size(104, 24);
            this.CarrierRadio.TabIndex = 2;
            this.CarrierRadio.TabStop = true;
            this.CarrierRadio.Text = "Carrier (5)";
            this.CarrierRadio.UseVisualStyleBackColor = true;
            this.CarrierRadio.CheckedChanged += new System.EventHandler(this.CarrierRadio_CheckedChanged);
            // 
            // BattleshipRadio
            // 
            this.BattleshipRadio.AutoSize = true;
            this.BattleshipRadio.Location = new System.Drawing.Point(13, 57);
            this.BattleshipRadio.Name = "BattleshipRadio";
            this.BattleshipRadio.Size = new System.Drawing.Size(128, 24);
            this.BattleshipRadio.TabIndex = 3;
            this.BattleshipRadio.TabStop = true;
            this.BattleshipRadio.Text = "Battleship (4)";
            this.BattleshipRadio.UseVisualStyleBackColor = true;
            this.BattleshipRadio.CheckedChanged += new System.EventHandler(this.BattleshipRadio_CheckedChanged);
            // 
            // CruiserRadio
            // 
            this.CruiserRadio.AutoSize = true;
            this.CruiserRadio.Location = new System.Drawing.Point(13, 87);
            this.CruiserRadio.Name = "CruiserRadio";
            this.CruiserRadio.Size = new System.Drawing.Size(107, 24);
            this.CruiserRadio.TabIndex = 4;
            this.CruiserRadio.TabStop = true;
            this.CruiserRadio.Text = "Cruiser (3)";
            this.CruiserRadio.UseVisualStyleBackColor = true;
            this.CruiserRadio.CheckedChanged += new System.EventHandler(this.CruiserRadio_CheckedChanged);
            // 
            // SubRadio
            // 
            this.SubRadio.AutoSize = true;
            this.SubRadio.Location = new System.Drawing.Point(13, 117);
            this.SubRadio.Name = "SubRadio";
            this.SubRadio.Size = new System.Drawing.Size(134, 24);
            this.SubRadio.TabIndex = 5;
            this.SubRadio.TabStop = true;
            this.SubRadio.Text = "Submarine (3)";
            this.SubRadio.UseVisualStyleBackColor = true;
            this.SubRadio.CheckedChanged += new System.EventHandler(this.SubRadio_CheckedChanged);
            // 
            // DestroyerRadio
            // 
            this.DestroyerRadio.AutoSize = true;
            this.DestroyerRadio.Location = new System.Drawing.Point(13, 147);
            this.DestroyerRadio.Name = "DestroyerRadio";
            this.DestroyerRadio.Size = new System.Drawing.Size(126, 24);
            this.DestroyerRadio.TabIndex = 6;
            this.DestroyerRadio.TabStop = true;
            this.DestroyerRadio.Text = "Destroyer (2)";
            this.DestroyerRadio.UseVisualStyleBackColor = true;
            this.DestroyerRadio.CheckedChanged += new System.EventHandler(this.DestroyerRadio_CheckedChanged);
            // 
            // HorizontalRadio
            // 
            this.HorizontalRadio.AutoSize = true;
            this.HorizontalRadio.Location = new System.Drawing.Point(11, 25);
            this.HorizontalRadio.Name = "HorizontalRadio";
            this.HorizontalRadio.Size = new System.Drawing.Size(106, 24);
            this.HorizontalRadio.TabIndex = 7;
            this.HorizontalRadio.TabStop = true;
            this.HorizontalRadio.Text = "Horizontal";
            this.HorizontalRadio.UseVisualStyleBackColor = true;
            this.HorizontalRadio.CheckedChanged += new System.EventHandler(this.HorizontalRadio_CheckedChanged);
            // 
            // VerticalRadio
            // 
            this.VerticalRadio.AutoSize = true;
            this.VerticalRadio.Location = new System.Drawing.Point(11, 55);
            this.VerticalRadio.Name = "VerticalRadio";
            this.VerticalRadio.Size = new System.Drawing.Size(87, 24);
            this.VerticalRadio.TabIndex = 8;
            this.VerticalRadio.TabStop = true;
            this.VerticalRadio.Text = "Vertical";
            this.VerticalRadio.UseVisualStyleBackColor = true;
            this.VerticalRadio.CheckedChanged += new System.EventHandler(this.VerticalRadio_CheckedChanged);
            // 
            // ShipsBox
            // 
            this.ShipsBox.Controls.Add(this.DestroyerCheck);
            this.ShipsBox.Controls.Add(this.SubCheck);
            this.ShipsBox.Controls.Add(this.CruiserCheck);
            this.ShipsBox.Controls.Add(this.BattleshipCheck);
            this.ShipsBox.Controls.Add(this.CarrierCheck);
            this.ShipsBox.Controls.Add(this.DestroyerRadio);
            this.ShipsBox.Controls.Add(this.SubRadio);
            this.ShipsBox.Controls.Add(this.CruiserRadio);
            this.ShipsBox.Controls.Add(this.BattleshipRadio);
            this.ShipsBox.Controls.Add(this.CarrierRadio);
            this.ShipsBox.Location = new System.Drawing.Point(12, 71);
            this.ShipsBox.Name = "ShipsBox";
            this.ShipsBox.Size = new System.Drawing.Size(254, 203);
            this.ShipsBox.TabIndex = 9;
            this.ShipsBox.TabStop = false;
            this.ShipsBox.Text = "Ships";
            // 
            // DestroyerCheck
            // 
            this.DestroyerCheck.AutoSize = true;
            this.DestroyerCheck.Location = new System.Drawing.Point(230, 149);
            this.DestroyerCheck.Name = "DestroyerCheck";
            this.DestroyerCheck.Size = new System.Drawing.Size(18, 20);
            this.DestroyerCheck.TabIndex = 11;
            this.DestroyerCheck.Text = "✓";
            this.DestroyerCheck.Visible = false;
            // 
            // SubCheck
            // 
            this.SubCheck.AutoSize = true;
            this.SubCheck.Location = new System.Drawing.Point(230, 119);
            this.SubCheck.Name = "SubCheck";
            this.SubCheck.Size = new System.Drawing.Size(18, 20);
            this.SubCheck.TabIndex = 10;
            this.SubCheck.Text = "✓";
            this.SubCheck.Visible = false;
            // 
            // CruiserCheck
            // 
            this.CruiserCheck.AutoSize = true;
            this.CruiserCheck.Location = new System.Drawing.Point(230, 89);
            this.CruiserCheck.Name = "CruiserCheck";
            this.CruiserCheck.Size = new System.Drawing.Size(18, 20);
            this.CruiserCheck.TabIndex = 9;
            this.CruiserCheck.Text = "✓";
            this.CruiserCheck.Visible = false;
            // 
            // BattleshipCheck
            // 
            this.BattleshipCheck.AutoSize = true;
            this.BattleshipCheck.Location = new System.Drawing.Point(230, 59);
            this.BattleshipCheck.Name = "BattleshipCheck";
            this.BattleshipCheck.Size = new System.Drawing.Size(18, 20);
            this.BattleshipCheck.TabIndex = 8;
            this.BattleshipCheck.Text = "✓";
            this.BattleshipCheck.Visible = false;
            // 
            // CarrierCheck
            // 
            this.CarrierCheck.AutoSize = true;
            this.CarrierCheck.Location = new System.Drawing.Point(230, 29);
            this.CarrierCheck.Name = "CarrierCheck";
            this.CarrierCheck.Size = new System.Drawing.Size(18, 20);
            this.CarrierCheck.TabIndex = 7;
            this.CarrierCheck.Text = "✓";
            this.CarrierCheck.Visible = false;
            // 
            // OrientationBox
            // 
            this.OrientationBox.Controls.Add(this.VerticalRadio);
            this.OrientationBox.Controls.Add(this.HorizontalRadio);
            this.OrientationBox.Location = new System.Drawing.Point(15, 291);
            this.OrientationBox.Name = "OrientationBox";
            this.OrientationBox.Size = new System.Drawing.Size(251, 96);
            this.OrientationBox.TabIndex = 10;
            this.OrientationBox.TabStop = false;
            this.OrientationBox.Text = "Orientation";
            // 
            // TLLabel
            // 
            this.TLLabel.AutoSize = true;
            this.TLLabel.Location = new System.Drawing.Point(22, 39);
            this.TLLabel.Name = "TLLabel";
            this.TLLabel.Size = new System.Drawing.Size(79, 20);
            this.TLLabel.TabIndex = 11;
            this.TLLabel.Text = "Time Left:";
            // 
            // SYBLabel
            // 
            this.SYBLabel.AutoSize = true;
            this.SYBLabel.Location = new System.Drawing.Point(21, 9);
            this.SYBLabel.Name = "SYBLabel";
            this.SYBLabel.Size = new System.Drawing.Size(119, 20);
            this.SYBLabel.TabIndex = 12;
            this.SYBLabel.Text = "Set Your Board";
            // 
            // TimeLeftLabel
            // 
            this.TimeLeftLabel.AutoSize = true;
            this.TimeLeftLabel.Location = new System.Drawing.Point(107, 39);
            this.TimeLeftLabel.Name = "TimeLeftLabel";
            this.TimeLeftLabel.Size = new System.Drawing.Size(27, 20);
            this.TimeLeftLabel.TabIndex = 13;
            this.TimeLeftLabel.Text = "60";
            // 
            // PlanningScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 520);
            this.Controls.Add(this.TimeLeftLabel);
            this.Controls.Add(this.SYBLabel);
            this.Controls.Add(this.TLLabel);
            this.Controls.Add(this.OrientationBox);
            this.Controls.Add(this.ShipsBox);
            this.Controls.Add(this.PlanningTable);
            this.Controls.Add(this.ReadyButton);
            this.Name = "PlanningScreen";
            this.Text = "PlanningScreen";
            this.ShipsBox.ResumeLayout(false);
            this.ShipsBox.PerformLayout();
            this.OrientationBox.ResumeLayout(false);
            this.OrientationBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PlanningScreen_FormClosing);
        }

        #endregion

        

        private System.Windows.Forms.Button ReadyButton;
        private System.Windows.Forms.TableLayoutPanel PlanningTable;
        private System.Windows.Forms.RadioButton BattleshipRadio;
        private System.Windows.Forms.RadioButton CruiserRadio;
        private System.Windows.Forms.RadioButton SubRadio;
        private System.Windows.Forms.RadioButton DestroyerRadio;
        private System.Windows.Forms.RadioButton CarrierRadio;
        private System.Windows.Forms.RadioButton HorizontalRadio;
        private System.Windows.Forms.RadioButton VerticalRadio;
        private System.Windows.Forms.GroupBox ShipsBox;
        private System.Windows.Forms.GroupBox OrientationBox;
        private System.Windows.Forms.Label DestroyerCheck;
        private System.Windows.Forms.Label SubCheck;
        private System.Windows.Forms.Label CruiserCheck;
        private System.Windows.Forms.Label BattleshipCheck;
        private System.Windows.Forms.Label CarrierCheck;
        private System.Windows.Forms.Label TLLabel;
        private System.Windows.Forms.Label SYBLabel;
        private System.Windows.Forms.Label TimeLeftLabel;
    }
}