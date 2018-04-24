namespace SpreadsheetGUI
{
    partial class Form2
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.OutputLog = new System.Windows.Forms.RichTextBox();
            this.ListOfSpreadSheetsBox = new System.Windows.Forms.ListBox();
            this.LoadSpreadsheetButton = new System.Windows.Forms.Button();
            this.LoadFileTextBox = new System.Windows.Forms.TextBox();
            this.Port_Label = new System.Windows.Forms.Label();
            this.IP_Label = new System.Windows.Forms.Label();
            this.IP_TextBox = new System.Windows.Forms.TextBox();
            this.Port_TextBox = new System.Windows.Forms.TextBox();
            this.ExitButton = new System.Windows.Forms.Button();
            this.ServerLabel = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.OutputLog);
            this.panel1.Controls.Add(this.ListOfSpreadSheetsBox);
            this.panel1.Controls.Add(this.LoadSpreadsheetButton);
            this.panel1.Controls.Add(this.LoadFileTextBox);
            this.panel1.Controls.Add(this.Port_Label);
            this.panel1.Controls.Add(this.IP_Label);
            this.panel1.Controls.Add(this.IP_TextBox);
            this.panel1.Controls.Add(this.Port_TextBox);
            this.panel1.Controls.Add(this.ExitButton);
            this.panel1.Controls.Add(this.ServerLabel);
            this.panel1.Controls.Add(this.ConnectButton);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(750, 488);
            this.panel1.TabIndex = 0;
            // 
            // OutputLog
            // 
            this.OutputLog.Location = new System.Drawing.Point(471, 277);
            this.OutputLog.Margin = new System.Windows.Forms.Padding(2);
            this.OutputLog.Name = "OutputLog";
            this.OutputLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.OutputLog.Size = new System.Drawing.Size(276, 79);
            this.OutputLog.TabIndex = 11;
            this.OutputLog.Text = "";
            this.OutputLog.Visible = false;
            // 
            // ListOfSpreadSheetsBox
            // 
            this.ListOfSpreadSheetsBox.FormattingEnabled = true;
            this.ListOfSpreadSheetsBox.Location = new System.Drawing.Point(5, 180);
            this.ListOfSpreadSheetsBox.Name = "ListOfSpreadSheetsBox";
            this.ListOfSpreadSheetsBox.Size = new System.Drawing.Size(742, 264);
            this.ListOfSpreadSheetsBox.TabIndex = 15;
            this.ListOfSpreadSheetsBox.TabStop = false;
            this.ListOfSpreadSheetsBox.Visible = false;
            this.ListOfSpreadSheetsBox.SelectedIndexChanged += new System.EventHandler(this.ListOfSpreadSheetsBox_SelectedIndexChanged);
            // 
            // LoadSpreadsheetButton
            // 
            this.LoadSpreadsheetButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadSpreadsheetButton.Location = new System.Drawing.Point(493, 455);
            this.LoadSpreadsheetButton.Name = "LoadSpreadsheetButton";
            this.LoadSpreadsheetButton.Size = new System.Drawing.Size(124, 30);
            this.LoadSpreadsheetButton.TabIndex = 8;
            this.LoadSpreadsheetButton.Text = "Load";
            this.LoadSpreadsheetButton.UseVisualStyleBackColor = true;
            this.LoadSpreadsheetButton.Visible = false;
            this.LoadSpreadsheetButton.Click += new System.EventHandler(this.LoadSpreadsheetButton_Click);
            // 
            // LoadFileTextBox
            // 
            this.LoadFileTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadFileTextBox.Location = new System.Drawing.Point(5, 458);
            this.LoadFileTextBox.Name = "LoadFileTextBox";
            this.LoadFileTextBox.Size = new System.Drawing.Size(352, 24);
            this.LoadFileTextBox.TabIndex = 7;
            this.LoadFileTextBox.Visible = false;
            // 
            // Port_Label
            // 
            this.Port_Label.AutoSize = true;
            this.Port_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Port_Label.Location = new System.Drawing.Point(47, 145);
            this.Port_Label.Name = "Port_Label";
            this.Port_Label.Size = new System.Drawing.Size(40, 16);
            this.Port_Label.TabIndex = 10;
            this.Port_Label.Text = "Port:";
            // 
            // IP_Label
            // 
            this.IP_Label.AutoSize = true;
            this.IP_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IP_Label.Location = new System.Drawing.Point(47, 96);
            this.IP_Label.Name = "IP_Label";
            this.IP_Label.Size = new System.Drawing.Size(88, 16);
            this.IP_Label.TabIndex = 9;
            this.IP_Label.Text = "IP Address:";
            // 
            // IP_TextBox
            // 
            this.IP_TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IP_TextBox.Location = new System.Drawing.Point(167, 93);
            this.IP_TextBox.Name = "IP_TextBox";
            this.IP_TextBox.Size = new System.Drawing.Size(352, 22);
            this.IP_TextBox.TabIndex = 1;
            // 
            // Port_TextBox
            // 
            this.Port_TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Port_TextBox.Location = new System.Drawing.Point(167, 139);
            this.Port_TextBox.Name = "Port_TextBox";
            this.Port_TextBox.Size = new System.Drawing.Size(352, 22);
            this.Port_TextBox.TabIndex = 2;
            // 
            // ExitButton
            // 
            this.ExitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExitButton.Location = new System.Drawing.Point(623, 455);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(124, 30);
            this.ExitButton.TabIndex = 6;
            this.ExitButton.Text = "Exit";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // ServerLabel
            // 
            this.ServerLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ServerLabel.AutoSize = true;
            this.ServerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerLabel.Location = new System.Drawing.Point(214, 20);
            this.ServerLabel.Name = "ServerLabel";
            this.ServerLabel.Size = new System.Drawing.Size(336, 25);
            this.ServerLabel.TabIndex = 6;
            this.ServerLabel.Text = "Connect to spreadsheet server";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConnectButton.Location = new System.Drawing.Point(569, 93);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(124, 68);
            this.ConnectButton.TabIndex = 3;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(774, 512);
            this.Controls.Add(this.panel1);
            this.Name = "Form2";
            this.Text = "Server Window";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Label ServerLabel;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Label Port_Label;
        private System.Windows.Forms.Label IP_Label;
        private System.Windows.Forms.TextBox IP_TextBox;
        private System.Windows.Forms.TextBox Port_TextBox;
        private System.Windows.Forms.RichTextBox OutputLog;
        private System.Windows.Forms.Button LoadSpreadsheetButton;
        private System.Windows.Forms.TextBox LoadFileTextBox;
        private System.Windows.Forms.ListBox ListOfSpreadSheetsBox;
    }
}