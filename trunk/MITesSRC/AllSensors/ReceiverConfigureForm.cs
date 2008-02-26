using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using HousenCS.MITes;
using System.Threading;


namespace MITesLogger_PC
{
	/// <summary>
	/// Summary description for ReceiverConfigureForm.
	/// </summary>
	public class ReceiverConfigureForm : System.Windows.Forms.Form
	{
		private MITesReceiverController mrc;
		//private Form1 mainForm;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.TextBox textBoxToSet;
		private System.Windows.Forms.TextBox textBoxCurrent;
		private System.Windows.Forms.MenuItem menuItemDone;
		private System.Windows.Forms.MenuItem menuItemChange;
		private System.Windows.Forms.MenuItem menuItemAdd;
		private System.Windows.Forms.MenuItem menuItemA0;
		private System.Windows.Forms.MenuItem menuItemA1;
		private System.Windows.Forms.MenuItem menuItemA2;
		private System.Windows.Forms.MenuItem menuItemA3;
		private System.Windows.Forms.MenuItem menuItemA4;
		private System.Windows.Forms.MenuItem menuItemA5;
		private System.Windows.Forms.MenuItem menuItemA6;
		private System.Windows.Forms.MenuItem menuItemA7;
		private System.Windows.Forms.MenuItem menuItemA8;
		private System.Windows.Forms.MenuItem menuItemA9;
		private System.Windows.Forms.MenuItem menuItemA10;
		private System.Windows.Forms.MenuItem menuItemA11;
		private System.Windows.Forms.MenuItem menuItemA12;
		private System.Windows.Forms.MenuItem menuItemA13;
		private System.Windows.Forms.MenuItem menuItemA14;
		private System.Windows.Forms.MenuItem menuItemA15;
		private System.Windows.Forms.MenuItem menuItemA16;
		private System.Windows.Forms.MenuItem menuItemA17;
		private System.Windows.Forms.MenuItem menuItemA18;
		private System.Windows.Forms.MenuItem menuItemA19;
		private System.Windows.Forms.MenuItem menuItemA20;
		private System.Windows.Forms.MenuItem menuItemRemove;
		private System.Windows.Forms.MenuItem menuItemSet;
		private System.Windows.Forms.MenuItem menuItemCheck;
		private System.Windows.Forms.MenuItem menuItemR0;
		private System.Windows.Forms.MenuItem menuItemR1;
		private System.Windows.Forms.MenuItem menuItemR2;
		private System.Windows.Forms.MenuItem menuItemR3;
		private System.Windows.Forms.MenuItem menuItemR4;
		private System.Windows.Forms.MenuItem menuItemR5;
		private System.Windows.Forms.MenuItem menuItemR6;
		private System.Windows.Forms.MenuItem menuItemR7;
		private System.Windows.Forms.MenuItem menuItemR8;
		private System.Windows.Forms.MenuItem menuItemR9;
		private System.Windows.Forms.MenuItem menuItemR10;
		private System.Windows.Forms.MenuItem menuItemR11;
		private System.Windows.Forms.MenuItem menuItemR12;
		private System.Windows.Forms.MenuItem menuItemR13;
		private System.Windows.Forms.MenuItem menuItemR14;
		private System.Windows.Forms.MenuItem menuItemR15;
		private System.Windows.Forms.MenuItem menuItemR16;
		private System.Windows.Forms.MenuItem menuItemR17;
		private System.Windows.Forms.MenuItem menuItemR18;
		private System.Windows.Forms.MenuItem menuItemR19;
		private System.Windows.Forms.MenuItem menuItemR20;

		private int[] currentIDs = new int[6];
		private int numCurrentIDs = 0;
		private int[] newIDs = new int[6];
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem7;
		private int numNewIDs = 0;
		private MITesActivityLogger aMITesActivityLogger;

		/// <summary>
		/// A form used to manually add unusual sensor configurations and set the MITesReceiver controller.
		/// </summary>
		/// <param name="mrc">The controller operating the MITesReceiver</param>
		/// <param name="mainForm">The main form of the application</param>
		/// <param name="aMITesActivityLogger">Logging object for comments</param>
		public ReceiverConfigureForm(MITesReceiverController mrc, Form1 mainForm, MITesActivityLogger aMITesActivityLogger)
		{
			this.aMITesActivityLogger = aMITesActivityLogger;
			InitializeComponent();
			this.mrc = mrc;

			for (int i = 0; i < 6; i++)
			{
				currentIDs[i] = MITesReceiverController.NO_ID;
				newIDs[i] = MITesReceiverController.NO_ID;
			}
			numCurrentIDs = 0;
			numNewIDs = 0;
		}

		private string GetChannelString(int num, int[] someIDs)
		{
			String str = "";
			for (int i = 0; i < num; i++)
			{
				str += " " + someIDs[i];
			}
			return str;
		}

		private string GetCurrentIDString()
		{
			return "Current: " + GetChannelString(numCurrentIDs, currentIDs);
		}

		private string GetToSetIDString()
		{
			return "Change to: " + GetChannelString(numNewIDs, newIDs);
		}

		/// <summary>
		/// 
		/// </summary>
		public void ReadChannels()
		{
			textBoxCurrent.Text = "Reading channels...";
			numCurrentIDs = mrc.ReadChannels (currentIDs);
			Console.WriteLine("Read num channels: " + numCurrentIDs);
			if (numCurrentIDs > 0)
			{
				for (int i = 0; i < currentIDs.Length; i++)
					newIDs[i] = currentIDs[i];
				numNewIDs = numCurrentIDs;
			}	
			textBoxCurrent.Text = GetCurrentIDString();
			textBoxToSet.Text = GetToSetIDString();
		}

		private void SetChannels()
		{
			for (int i = 0; i < numNewIDs; i++)
				Console.WriteLine("Try to set Channel " + i + ": " + newIDs[i]);
		}

		private void AddToIDs(int id)
		{
			if (IsInIDs(id,newIDs))
				return;
	
			bool isAdded = false;
			for (int i = 0; i < 6; i++)
			{
				if (!isAdded)
					if (newIDs[i] == MITesReceiverController.NO_ID) // empty
					{
						newIDs[i] = id;
						isAdded = true;
						numNewIDs++;
					}
			}
			if (!isAdded)
			{
				textBoxToSet.Text = "Too full. Delete one first.";
				Thread.Sleep(2000);
			}
		
			textBoxToSet.Text = GetToSetIDString();
		}

		private void RemoveFromIDs(int id)
		{
			for (int i = 0; i < 6; i++)
			{
				if (newIDs[i] == id)
					newIDs[i] = MITesReceiverController.NO_ID;
			}
			CleanUpZeros();
			
			textBoxToSet.Text = GetToSetIDString();
		}

		private int GetIDInt(string id) 
		{
			return Convert.ToInt32(id);
		}

		private bool IsInIDs(int id, int[] someIDs)
		{
			for (int i = 0; i < 6; i++)
			{
				if (someIDs[i] == id)
					return true;
			}
			return false;
		}

		private void CleanUpZeros()
		{
			int[] temp = new int[6];
			for (int i = 0; i < 6; i++)
				temp[i] = MITesReceiverController.NO_ID;

			int index = 0;
			for (int i = 0; i < 6; i++)
			{
				if (newIDs[i] != MITesReceiverController.NO_ID)
				{
					temp[index] = newIDs[i];
					index++;
				}
			}
	
			for (int i = 0; i < 6; i++)
				newIDs[i] = temp[i];
		
			numNewIDs = index;
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItemDone = new System.Windows.Forms.MenuItem();
			this.menuItemChange = new System.Windows.Forms.MenuItem();
			this.menuItemAdd = new System.Windows.Forms.MenuItem();
			this.menuItemA0 = new System.Windows.Forms.MenuItem();
			this.menuItemA1 = new System.Windows.Forms.MenuItem();
			this.menuItemA2 = new System.Windows.Forms.MenuItem();
			this.menuItemA3 = new System.Windows.Forms.MenuItem();
			this.menuItemA4 = new System.Windows.Forms.MenuItem();
			this.menuItemA5 = new System.Windows.Forms.MenuItem();
			this.menuItemA6 = new System.Windows.Forms.MenuItem();
			this.menuItemA7 = new System.Windows.Forms.MenuItem();
			this.menuItemA8 = new System.Windows.Forms.MenuItem();
			this.menuItemA9 = new System.Windows.Forms.MenuItem();
			this.menuItemA10 = new System.Windows.Forms.MenuItem();
			this.menuItemA11 = new System.Windows.Forms.MenuItem();
			this.menuItemA12 = new System.Windows.Forms.MenuItem();
			this.menuItemA13 = new System.Windows.Forms.MenuItem();
			this.menuItemA14 = new System.Windows.Forms.MenuItem();
			this.menuItemA15 = new System.Windows.Forms.MenuItem();
			this.menuItemA16 = new System.Windows.Forms.MenuItem();
			this.menuItemA17 = new System.Windows.Forms.MenuItem();
			this.menuItemA18 = new System.Windows.Forms.MenuItem();
			this.menuItemA19 = new System.Windows.Forms.MenuItem();
			this.menuItemA20 = new System.Windows.Forms.MenuItem();
			this.menuItemRemove = new System.Windows.Forms.MenuItem();
			this.menuItemR0 = new System.Windows.Forms.MenuItem();
			this.menuItemR1 = new System.Windows.Forms.MenuItem();
			this.menuItemR2 = new System.Windows.Forms.MenuItem();
			this.menuItemR3 = new System.Windows.Forms.MenuItem();
			this.menuItemR4 = new System.Windows.Forms.MenuItem();
			this.menuItemR5 = new System.Windows.Forms.MenuItem();
			this.menuItemR6 = new System.Windows.Forms.MenuItem();
			this.menuItemR7 = new System.Windows.Forms.MenuItem();
			this.menuItemR8 = new System.Windows.Forms.MenuItem();
			this.menuItemR9 = new System.Windows.Forms.MenuItem();
			this.menuItemR10 = new System.Windows.Forms.MenuItem();
			this.menuItemR11 = new System.Windows.Forms.MenuItem();
			this.menuItemR12 = new System.Windows.Forms.MenuItem();
			this.menuItemR13 = new System.Windows.Forms.MenuItem();
			this.menuItemR14 = new System.Windows.Forms.MenuItem();
			this.menuItemR15 = new System.Windows.Forms.MenuItem();
			this.menuItemR16 = new System.Windows.Forms.MenuItem();
			this.menuItemR17 = new System.Windows.Forms.MenuItem();
			this.menuItemR18 = new System.Windows.Forms.MenuItem();
			this.menuItemR19 = new System.Windows.Forms.MenuItem();
			this.menuItemR20 = new System.Windows.Forms.MenuItem();
			this.menuItemSet = new System.Windows.Forms.MenuItem();
			this.menuItemCheck = new System.Windows.Forms.MenuItem();
			this.textBoxToSet = new System.Windows.Forms.TextBox();
			this.textBoxCurrent = new System.Windows.Forms.TextBox();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.Add(this.menuItemDone);
			this.mainMenu1.MenuItems.Add(this.menuItemChange);
			// 
			// menuItemDone
			// 
			this.menuItemDone.Text = "Done";
			this.menuItemDone.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// menuItemChange
			// 
			this.menuItemChange.MenuItems.Add(this.menuItem1);
			this.menuItemChange.MenuItems.Add(this.menuItemAdd);
			this.menuItemChange.MenuItems.Add(this.menuItemRemove);
			this.menuItemChange.MenuItems.Add(this.menuItemSet);
			this.menuItemChange.MenuItems.Add(this.menuItemCheck);
			this.menuItemChange.Text = "Change";
			// 
			// menuItemAdd
			// 
			this.menuItemAdd.MenuItems.Add(this.menuItemA0);
			this.menuItemAdd.MenuItems.Add(this.menuItemA1);
			this.menuItemAdd.MenuItems.Add(this.menuItemA2);
			this.menuItemAdd.MenuItems.Add(this.menuItemA3);
			this.menuItemAdd.MenuItems.Add(this.menuItemA4);
			this.menuItemAdd.MenuItems.Add(this.menuItemA5);
			this.menuItemAdd.MenuItems.Add(this.menuItemA6);
			this.menuItemAdd.MenuItems.Add(this.menuItemA7);
			this.menuItemAdd.MenuItems.Add(this.menuItemA8);
			this.menuItemAdd.MenuItems.Add(this.menuItemA9);
			this.menuItemAdd.MenuItems.Add(this.menuItemA10);
			this.menuItemAdd.MenuItems.Add(this.menuItemA11);
			this.menuItemAdd.MenuItems.Add(this.menuItemA12);
			this.menuItemAdd.MenuItems.Add(this.menuItemA13);
			this.menuItemAdd.MenuItems.Add(this.menuItemA14);
			this.menuItemAdd.MenuItems.Add(this.menuItemA15);
			this.menuItemAdd.MenuItems.Add(this.menuItemA16);
			this.menuItemAdd.MenuItems.Add(this.menuItemA17);
			this.menuItemAdd.MenuItems.Add(this.menuItemA18);
			this.menuItemAdd.MenuItems.Add(this.menuItemA19);
			this.menuItemAdd.MenuItems.Add(this.menuItemA20);
			this.menuItemAdd.Text = "Add";
			// 
			// menuItemA0
			// 
			this.menuItemA0.Text = "0";
			this.menuItemA0.Click += new System.EventHandler(this.menuItemA0_Click);
			// 
			// menuItemA1
			// 
			this.menuItemA1.Text = "1";
			this.menuItemA1.Click += new System.EventHandler(this.menuItemA1_Click);
			// 
			// menuItemA2
			// 
			this.menuItemA2.Text = "2";
			this.menuItemA2.Click += new System.EventHandler(this.menuItemA2_Click);
			// 
			// menuItemA3
			// 
			this.menuItemA3.Text = "3";
			this.menuItemA3.Click += new System.EventHandler(this.menuItemA3_Click);
			// 
			// menuItemA4
			// 
			this.menuItemA4.Text = "4";
			this.menuItemA4.Click += new System.EventHandler(this.menuItemA4_Click);
			// 
			// menuItemA5
			// 
			this.menuItemA5.Text = "5";
			this.menuItemA5.Click += new System.EventHandler(this.menuItemA5_Click);
			// 
			// menuItemA6
			// 
			this.menuItemA6.Text = "6";
			this.menuItemA6.Click += new System.EventHandler(this.menuItemA6_Click);
			// 
			// menuItemA7
			// 
			this.menuItemA7.Text = "7";
			this.menuItemA7.Click += new System.EventHandler(this.menuItemA7_Click);
			// 
			// menuItemA8
			// 
			this.menuItemA8.Text = "8";
			this.menuItemA8.Click += new System.EventHandler(this.menuItemA8_Click);
			// 
			// menuItemA9
			// 
			this.menuItemA9.Text = "9";
			this.menuItemA9.Click += new System.EventHandler(this.menuItemA9_Click);
			// 
			// menuItemA10
			// 
			this.menuItemA10.Text = "10";
			this.menuItemA10.Click += new System.EventHandler(this.menuItemA10_Click);
			// 
			// menuItemA11
			// 
			this.menuItemA11.Text = "11";
			this.menuItemA11.Click += new System.EventHandler(this.menuItemA11_Click);
			// 
			// menuItemA12
			// 
			this.menuItemA12.Text = "12";
			this.menuItemA12.Click += new System.EventHandler(this.menuItemA12_Click);
			// 
			// menuItemA13
			// 
			this.menuItemA13.Text = "13";
			this.menuItemA13.Click += new System.EventHandler(this.menuItemA13_Click);
			// 
			// menuItemA14
			// 
			this.menuItemA14.Text = "14";
			this.menuItemA14.Click += new System.EventHandler(this.menuItemA14_Click);
			// 
			// menuItemA15
			// 
			this.menuItemA15.Text = "15";
			this.menuItemA15.Click += new System.EventHandler(this.menuItemA15_Click);
			// 
			// menuItemA16
			// 
			this.menuItemA16.Text = "16";
			this.menuItemA16.Click += new System.EventHandler(this.menuItemA16_Click);
			// 
			// menuItemA17
			// 
			this.menuItemA17.Text = "17";
			this.menuItemA17.Click += new System.EventHandler(this.menuItemA17_Click);
			//
			// menuItemA18
			// 
			this.menuItemA18.Text = "18";
			this.menuItemA18.Click += new System.EventHandler(this.menuItemA18_Click);
			// 
			// menuItemA19
			// 
			this.menuItemA19.Text = "19";
			this.menuItemA19.Click += new System.EventHandler(this.menuItemA19_Click);
			// 
			// menuItemA20
			// 
			this.menuItemA20.Text = "20";
			this.menuItemA20.Click += new System.EventHandler(this.menuItemA20_Click);
			// 
			// menuItemRemove
			// 
			this.menuItemRemove.MenuItems.Add(this.menuItemR0);
			this.menuItemRemove.MenuItems.Add(this.menuItemR1);
			this.menuItemRemove.MenuItems.Add(this.menuItemR2);
			this.menuItemRemove.MenuItems.Add(this.menuItemR3);
			this.menuItemRemove.MenuItems.Add(this.menuItemR4);
			this.menuItemRemove.MenuItems.Add(this.menuItemR5);
			this.menuItemRemove.MenuItems.Add(this.menuItemR6);
			this.menuItemRemove.MenuItems.Add(this.menuItemR7);
			this.menuItemRemove.MenuItems.Add(this.menuItemR8);
			this.menuItemRemove.MenuItems.Add(this.menuItemR9);
			this.menuItemRemove.MenuItems.Add(this.menuItemR10);
			this.menuItemRemove.MenuItems.Add(this.menuItemR11);
			this.menuItemRemove.MenuItems.Add(this.menuItemR12);
			this.menuItemRemove.MenuItems.Add(this.menuItemR13);
			this.menuItemRemove.MenuItems.Add(this.menuItemR14);
			this.menuItemRemove.MenuItems.Add(this.menuItemR15);
			this.menuItemRemove.MenuItems.Add(this.menuItemR16);
			this.menuItemRemove.MenuItems.Add(this.menuItemR17);
			this.menuItemRemove.MenuItems.Add(this.menuItemR18);
			this.menuItemRemove.MenuItems.Add(this.menuItemR19);
			this.menuItemRemove.MenuItems.Add(this.menuItemR20);
			this.menuItemRemove.Text = "Remove";
			// 
			// menuItemR0
			// 
			this.menuItemR0.Text = "0 (non accel)";
			this.menuItemR0.Click += new System.EventHandler(this.menuItemR0_Click);
			// 
			// menuItemR1
			// 
			this.menuItemR1.Text = "1";
			this.menuItemR1.Click += new System.EventHandler(this.menuItemR1_Click);
			// 
			// menuItemR2
			// 
			this.menuItemR2.Text = "2";
			this.menuItemR2.Click += new System.EventHandler(this.menuItemR2_Click);
			// 
			// menuItemR3
			// 
			this.menuItemR3.Text = "3";
			this.menuItemR3.Click += new System.EventHandler(this.menuItemR3_Click);
			// 
			// menuItemR4
			// 
			this.menuItemR4.Text = "4";
			this.menuItemR4.Click += new System.EventHandler(this.menuItemR4_Click);
			// 
			// menuItemR5
			// 
			this.menuItemR5.Text = "5";
			this.menuItemR5.Click += new System.EventHandler(this.menuItemR5_Click);
			// 
			// menuItemR6
			// 
			this.menuItemR6.Text = "6";
			this.menuItemR6.Click += new System.EventHandler(this.menuItemR6_Click);
			// 
			// menuItemR7
			// 
			this.menuItemR7.Text = "7";
			this.menuItemR7.Click += new System.EventHandler(this.menuItemR7_Click);
			// 
			// menuItemR8
			// 
			this.menuItemR8.Text = "8";
			this.menuItemR8.Click += new System.EventHandler(this.menuItemR8_Click);
			// 
			// menuItemR9
			// 
			this.menuItemR9.Text = "9";
			this.menuItemR9.Click += new System.EventHandler(this.menuItemR9_Click);
			// 
			// menuItemR10
			// 
			this.menuItemR10.Text = "10";
			this.menuItemR10.Click += new System.EventHandler(this.menuItemR10_Click);
			// 
			// menuItemR11
			// 
			this.menuItemR11.Text = "11";
			this.menuItemR11.Click += new System.EventHandler(this.menuItemR11_Click);
			// 
			// menuItemR12
			// 
			this.menuItemR12.Text = "12";
			this.menuItemR12.Click += new System.EventHandler(this.menuItemR12_Click);
			// 
			// menuItemR13
			// 
			this.menuItemR13.Text = "13";
			this.menuItemR13.Click += new System.EventHandler(this.menuItemR13_Click);
			// 
			// menuItemR14
			// 
			this.menuItemR14.Text = "14";
			this.menuItemR14.Click += new System.EventHandler(this.menuItemR14_Click);
			// 
			// menuItemR15
			// 
			this.menuItemR15.Text = "15";
			this.menuItemR15.Click += new System.EventHandler(this.menuItemR15_Click);
			// 
			// menuItemR16
			// 
			this.menuItemR16.Text = "16";
			this.menuItemR16.Click += new System.EventHandler(this.menuItemR16_Click);
			//
			// menuItemR17
			// 
			this.menuItemR17.Text = "17";
			this.menuItemR17.Click += new System.EventHandler(this.menuItemR17_Click);
			// 
			// menuItemR18
			// 
			this.menuItemR18.Text = "18";
			this.menuItemR18.Click += new System.EventHandler(this.menuItemR18_Click);
			// 
			// menuItemR19
			// 
			this.menuItemR19.Text = "19";
			this.menuItemR19.Click += new System.EventHandler(this.menuItemR19_Click);
			// 
			// menuItemR20
			// 
			this.menuItemR20.Text = "20";
			this.menuItemR20.Click += new System.EventHandler(this.menuItemR20_Click);
			// 
			// menuItemSet
			// 
			this.menuItemSet.Text = "Set";
			this.menuItemSet.Click += new System.EventHandler(this.menuItemSet_Click);
			// 
			// menuItemCheck
			// 
			this.menuItemCheck.Text = "Check";
			this.menuItemCheck.Click += new System.EventHandler(this.menuItemCheck_Click);
			// 
			// textBoxToSet
			// 
			this.textBoxToSet.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Regular);
			this.textBoxToSet.Location = new System.Drawing.Point(8, 40);
			this.textBoxToSet.ReadOnly = true;
			this.textBoxToSet.Size = new System.Drawing.Size(160, 22);
			this.textBoxToSet.Text = "Change to:";
			// 
			// textBoxCurrent
			// 
			this.textBoxCurrent.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Regular);
			this.textBoxCurrent.Location = new System.Drawing.Point(8, 8);
			this.textBoxCurrent.ReadOnly = true;
			this.textBoxCurrent.Size = new System.Drawing.Size(160, 22);
			this.textBoxCurrent.Text = "Channels: ";
			// 
			// menuItem1
			// 
			this.menuItem1.MenuItems.Add(this.menuItem2);
			this.menuItem1.MenuItems.Add(this.menuItem3);
			this.menuItem1.MenuItems.Add(this.menuItem4);
			this.menuItem1.MenuItems.Add(this.menuItem5);
			this.menuItem1.MenuItems.Add(this.menuItem6);
			this.menuItem1.MenuItems.Add(this.menuItem7);
			this.menuItem1.Text = "Common";
			// 
			// menuItem2
			// 
			this.menuItem2.Text = "Channel 0 only";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Text = "Channel 1 only";
			this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Text = "Channels 0-1";
			this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Text = "Channels 0,1,7";
			this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Text = "Channels 0,1,7,8";
			this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Text = "Channels 0,1,7,8,11,14";
			this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
			// 
			// ReceiverConfigureForm
			// 
			this.Controls.Add(this.textBoxCurrent);
			this.Controls.Add(this.textBoxToSet);
			this.Menu = this.mainMenu1;
			this.Text = "Set MITes";

		}
		#endregion

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
			//mainForm.SetMaxPlots();
			this.Hide();
		}

		private void SetNewChannels()
		{
			if (numNewIDs > 0)
			{
				textBoxCurrent.Text = "Setting!";
				aMITesActivityLogger.WriteLogComment("Set num channels to :" + numNewIDs);
				for (int i = 0; i < numNewIDs; i++)
					aMITesActivityLogger.WriteLogComment("Set channel " + i + " to :" + newIDs[i]);
				mrc.SetChannels(numNewIDs,newIDs);
				Thread.Sleep(500);
				ReadChannels();
			}
			else
			{
				textBoxCurrent.Text = "Can't set to 0 channels!";
				Thread.Sleep(2000);
				textBoxCurrent.Text = GetToSetIDString();
			}			
		}

		private void menuItemSet_Click(object sender, System.EventArgs e)
		{
			SetNewChannels();
		}

		private void menuItemA0_Click(object sender, System.EventArgs e)
		{
			AddToIDs(0);
		}

		private void menuItemA1_Click(object sender, System.EventArgs e)
		{
			AddToIDs(1);
		}

		private void menuItemA2_Click(object sender, System.EventArgs e)
		{
			AddToIDs(2);		
		}

		private void menuItemA3_Click(object sender, System.EventArgs e)
		{
			AddToIDs(3);
		}

		private void menuItemA4_Click(object sender, System.EventArgs e)
		{
			AddToIDs(4);
		}

		private void menuItemA5_Click(object sender, System.EventArgs e)
		{
			AddToIDs(5);
		}

		private void menuItemA6_Click(object sender, System.EventArgs e)
		{
			AddToIDs(6);
		}

		private void menuItemA7_Click(object sender, System.EventArgs e)
		{
			AddToIDs(7);
		}

		private void menuItemA8_Click(object sender, System.EventArgs e)
		{
			AddToIDs(8);
		}

		private void menuItemA9_Click(object sender, System.EventArgs e)
		{
			AddToIDs(9);
		}

		private void menuItemA10_Click(object sender, System.EventArgs e)
		{
			AddToIDs(10);
		}

		private void menuItemA11_Click(object sender, System.EventArgs e)
		{
			AddToIDs(11);
		}

		private void menuItemA12_Click(object sender, System.EventArgs e)
		{
			AddToIDs(12);
		}

		private void menuItemA13_Click(object sender, System.EventArgs e)
		{
			AddToIDs(13);
		}

		private void menuItemA14_Click(object sender, System.EventArgs e)
		{
			AddToIDs(14);
		}

		private void menuItemA15_Click(object sender, System.EventArgs e)
		{
			AddToIDs(15);
		}

		private void menuItemA16_Click(object sender, System.EventArgs e)
		{
			AddToIDs(16);
		}

		private void menuItemA17_Click(object sender, System.EventArgs e)
		{
			AddToIDs(17);
		}

		private void menuItemA18_Click(object sender, System.EventArgs e)
		{
			AddToIDs(18);
		}

		private void menuItemA19_Click(object sender, System.EventArgs e)
		{
			AddToIDs(19);
		}

		private void menuItemA20_Click(object sender, System.EventArgs e)
		{
			AddToIDs(20);
		}
	
		private void menuItemR0_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(0);
		}

		private void menuItemR1_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(1);
		}

		private void menuItemR2_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(2);		
		}

		private void menuItemR3_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(3);
		}

		private void menuItemR4_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(4);
		}

		private void menuItemR5_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(5);
		}

		private void menuItemR6_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(6);
		}

		private void menuItemR7_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(7);
		}

		private void menuItemR8_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(8);
		}

		private void menuItemR9_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(9);
		}

		private void menuItemR10_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(10);
		}

		private void menuItemR11_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(11);
		}

		private void menuItemR12_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(12);
		}

		private void menuItemR13_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(13);
		}

		private void menuItemR14_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(14);
		}

		private void menuItemR15_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(15);
		}

		private void menuItemR16_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(16);
		}

		private void menuItemR17_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(17);
		}

		private void menuItemR18_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(18);
		}

		private void menuItemR19_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(19);
		}

		private void menuItemR20_Click(object sender, System.EventArgs e)
		{
			RemoveFromIDs(20);
		}

		private void menuItemCheck_Click(object sender, System.EventArgs e)
		{
			ReadChannels();
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			numNewIDs = 1; 
			newIDs[0] = 0;
			newIDs[1] = 0;
			newIDs[2] = 0;
			newIDs[3] = 0;
			newIDs[4] = 0;
			newIDs[5] = 0;
			SetNewChannels();
		}

		private void menuItem3_Click(object sender, System.EventArgs e)
		{
			numNewIDs = 1; 
			newIDs[0] = 1;
			newIDs[1] = 0;
			newIDs[2] = 0;
			newIDs[3] = 0;
			newIDs[4] = 0;
			newIDs[5] = 0;
			SetNewChannels();

		}

		private void menuItem4_Click(object sender, System.EventArgs e)
		{
			numNewIDs = 2; 
			newIDs[0] = 0;
			newIDs[1] = 1;
			newIDs[2] = 0;
			newIDs[3] = 0;
			newIDs[4] = 0;
			newIDs[5] = 0;
			SetNewChannels();

		}

		private void menuItem5_Click(object sender, System.EventArgs e)
		{
			numNewIDs = 3; 
			newIDs[0] = 0;
			newIDs[1] = 1;
			newIDs[2] = 7;
			newIDs[3] = 0;
			newIDs[4] = 0;
			newIDs[5] = 0;
			SetNewChannels();

		}

		private void menuItem6_Click(object sender, System.EventArgs e)
		{
			numNewIDs = 4; 
			newIDs[0] = 0;
			newIDs[1] = 1;
			newIDs[2] = 7;
			newIDs[3] = 8;
			newIDs[4] = 0;
			newIDs[5] = 0;
			SetNewChannels();

		}

		private void menuItem7_Click(object sender, System.EventArgs e)
		{
			numNewIDs = 6; 
			newIDs[0] = 0;
			newIDs[1] = 1;
			newIDs[2] = 7;
			newIDs[3] = 8;
			newIDs[4] = 11;
			newIDs[5] = 14;
			SetNewChannels();
		}
	}
}
