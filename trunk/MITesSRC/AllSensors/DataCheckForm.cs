using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using HousenCS.MITes;

namespace MITesLogger_SM
{
	/// <summary>
	/// Summary description for DataCheckForm.
	/// </summary>
	public class DataCheckForm : System.Windows.Forms.Form
	{
		private MITesReceiverController mrc;
		private System.Windows.Forms.MainMenu mainMenu2;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.TextBox textBoxPort;
		private System.Windows.Forms.MenuItem menuItem1;

		private bool isActive = false; 
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		public void SetIsActive(bool val)
		{
			isActive = val;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool GetIsActive()
		{
			return isActive;
		}

		private void InitializeComponent()
		{
			this.mainMenu2 = new System.Windows.Forms.MainMenu();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.textBoxPort = new System.Windows.Forms.TextBox();
			// 
			// mainMenu2
			// 
			this.mainMenu2.MenuItems.Add(this.menuItem2);
			this.mainMenu2.MenuItems.Add(this.menuItem1);
			// 
			// menuItem2
			// 
			this.menuItem2.Text = "Done";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Text = "Grab data";
			// 
			// textBoxPort
			// 
			this.textBoxPort.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Regular);
			this.textBoxPort.Location = new System.Drawing.Point(8, 8);
			this.textBoxPort.ReadOnly = true;
			this.textBoxPort.Size = new System.Drawing.Size(160, 22);
			this.textBoxPort.Text = "Port: ";
			// 
			// DataCheckForm
			// 
			this.Controls.Add(this.textBoxPort);
			this.Menu = this.mainMenu2;
			this.Text = "Check data";
			this.Load += new System.EventHandler(this.DataCheckForm_Load);

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mrc"></param>
		/// <param name="mainForm"></param>
		public DataCheckForm(MITesReceiverController mrc, Form1 mainForm)
		{
			this.mrc = mrc;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			SetIsActive(false);
			this.Hide();		
		}

		private void SetCurrentPort()
		{
			if ((mrc != null) && (mrc.GetComPortNumber() != 0))
			{
				// Found valid port
				textBoxPort.Text = "Using COM: " + mrc.GetComPortNumber ();
			}
			else
			{
				textBoxPort.Text = "No MITes Receiver!";
			}
		}

		private void DataCheckForm_Load(object sender, System.EventArgs e)
		{
			SetCurrentPort();
		}

	}
}
