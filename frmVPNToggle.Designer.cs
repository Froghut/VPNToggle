namespace VPNToggle
{
	partial class frmVPNToggle
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			button_DisableVPN = new Button();
			button_EnableVPN = new Button();
			comboBox1 = new ComboBox();
			txt_RouterIP = new TextBox();
			label1 = new Label();
			label2 = new Label();
			txt_RouterPassword = new TextBox();
			btn_GetStatus = new Button();
			SuspendLayout();
			// 
			// button_DisableVPN
			// 
			button_DisableVPN.BackColor = Color.Red;
			button_DisableVPN.Location = new Point(12, 64);
			button_DisableVPN.Name = "button_DisableVPN";
			button_DisableVPN.Size = new Size(204, 154);
			button_DisableVPN.TabIndex = 0;
			button_DisableVPN.Text = "Disable VPN for this Machine";
			button_DisableVPN.UseVisualStyleBackColor = false;
			button_DisableVPN.Click += button_DisableVPN_Click;
			// 
			// button_EnableVPN
			// 
			button_EnableVPN.BackColor = Color.Lime;
			button_EnableVPN.Location = new Point(222, 64);
			button_EnableVPN.Name = "button_EnableVPN";
			button_EnableVPN.Size = new Size(204, 154);
			button_EnableVPN.TabIndex = 1;
			button_EnableVPN.Text = "Enable VPN for this Machine";
			button_EnableVPN.UseVisualStyleBackColor = false;
			button_EnableVPN.Click += button_EnableVPN_Click;
			// 
			// comboBox1
			// 
			comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBox1.FormattingEnabled = true;
			comboBox1.Location = new Point(12, 35);
			comboBox1.Name = "comboBox1";
			comboBox1.Size = new Size(414, 23);
			comboBox1.TabIndex = 2;
			comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
			// 
			// txt_RouterIP
			// 
			txt_RouterIP.Location = new Point(73, 6);
			txt_RouterIP.Name = "txt_RouterIP";
			txt_RouterIP.Size = new Size(97, 23);
			txt_RouterIP.TabIndex = 3;
			txt_RouterIP.Text = "192.168.1.1";
			txt_RouterIP.TextChanged += textBox1_TextChanged;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(12, 9);
			label1.Name = "label1";
			label1.Size = new Size(55, 15);
			label1.TabIndex = 4;
			label1.Text = "Router IP";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(198, 9);
			label2.Name = "label2";
			label2.Size = new Size(95, 15);
			label2.TabIndex = 5;
			label2.Text = "Router Password";
			// 
			// txt_RouterPassword
			// 
			txt_RouterPassword.Location = new Point(299, 6);
			txt_RouterPassword.Name = "txt_RouterPassword";
			txt_RouterPassword.PasswordChar = '*';
			txt_RouterPassword.Size = new Size(127, 23);
			txt_RouterPassword.TabIndex = 6;
			txt_RouterPassword.TextChanged += txt_RouterPassword_TextChanged;
			// 
			// btn_GetStatus
			// 
			btn_GetStatus.Location = new Point(12, 64);
			btn_GetStatus.Name = "btn_GetStatus";
			btn_GetStatus.Size = new Size(414, 154);
			btn_GetStatus.TabIndex = 7;
			btn_GetStatus.Text = "Get VPN Status from Router";
			btn_GetStatus.UseVisualStyleBackColor = true;
			btn_GetStatus.Click += btn_GetStatus_Click;
			// 
			// frmVPNToggle
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(437, 230);
			Controls.Add(btn_GetStatus);
			Controls.Add(txt_RouterPassword);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(txt_RouterIP);
			Controls.Add(comboBox1);
			Controls.Add(button_EnableVPN);
			Controls.Add(button_DisableVPN);
			Name = "frmVPNToggle";
			Text = "VPN Toggle";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button button_DisableVPN;
		private Button button_EnableVPN;
		private ComboBox comboBox1;
		private TextBox txt_RouterIP;
		private Label label1;
		private Label label2;
		private TextBox txt_RouterPassword;
		private Button btn_GetStatus;
	}
}
