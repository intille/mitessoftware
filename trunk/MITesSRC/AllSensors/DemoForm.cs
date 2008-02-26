using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using HousenCS.MITes;

namespace MITesLogger_PC
{
	/// <summary>
	/// Summary description for DemoForm.
	/// </summary>
	public class DemoForm : System.Windows.Forms.Form
	{
		private System.ComponentModel.Container components = null;
		private ArrayList someLabels = new ArrayList(50);
		private ArrayList someActiveIDs = new ArrayList(200);
		private int[] someActiveTimes = new int[5000];
		private MITesDemoObjectAnalyzer aMITesDOA;

		private int[] limitIDs = {736,553,549,582,683};
		
		/// <summary>
		/// 
		/// </summary>
		public void UpdateLabels()
		{
			int[] idList = aMITesDOA.GetIDs();
			int[] valList = aMITesDOA.GetVals();
			int[] repList = aMITesDOA.GetReps();
			int numIDs = aMITesDOA.GetNumIDs();

			AddNewButtons(idList, numIDs);

			for (int i = 0; i < numIDs; i++)
			{
				Console.WriteLine ("ID: " + idList[i] + " ValList: " + valList[i] + " RepList: " + repList[i]);

				if ((valList[i] == -1))
					AliveHighlight(idList[i]);
				if ((valList[i] != 0) && (repList[i] == 3))
					Highlight(idList[i]);
					//MotionHighlight(idList[i]);
				else if ((valList[i] != 0) && (repList[i] != 3))
					Highlight(idList[i]);
				else
					Highlight(idList[i]);
					//UnHighlight(idList[i]);
			}

			CheckTimeouts();
		}

		private void CheckTimeouts()
		{
			foreach (int anID in someActiveIDs)
			{
				// Not in motion filter and 1 sec elapsed
				if (((Environment.TickCount-someActiveTimes[anID]) > 1000) &&
					(someActiveTimes[anID] != 0))
					UnHighlight(anID);
			}
		}
																
		private bool IsValidID(int anID)
		{
			if (limitIDs.Length < 1)
				return true;
			for (int i = 0; i < limitIDs.Length; i++)
				if (anID == limitIDs[i])
					return true;
			return false;
		}

		private bool IDActive(int anID)
		{
			foreach (int oneID in someActiveIDs)
			{
				if (anID == oneID)
					return true;				
			}
			return false;

		}

		private const int GAP = 4; 
		private const int BUTTON_WIDTH = 184;
		private const int BUTTON_HEIGHT = 40;
		private int lastXLocation = GAP;
		private int lastYLocation = GAP; 
		private int maxYDim = 6*(BUTTON_HEIGHT + GAP) + GAP;
		private int maxXDim = 2*(BUTTON_WIDTH + GAP) + GAP;

		private void IncrementButtonLocation()
		{
			if (lastYLocation >= maxYDim)
			{
				lastYLocation = GAP;
				lastXLocation += BUTTON_WIDTH + GAP;
			}
			else
			{
				lastYLocation += BUTTON_HEIGHT + GAP;
			}
		}

		private void AddNewButtons(int[] idList, int numIDs)
		{
			for (int i = 0; i < numIDs; i++)
			{
				if (AddNewLabel(idList[i]))
					Console.WriteLine ("Added BUTTON: " + idList[i]);
			}
		}

		private bool AddNewLabel(int ID)
		{
			if (!IDActive(ID) && IsValidID(ID))
			{
				// Add it
				this.SuspendLayout();
				System.Windows.Forms.Label aLabel = new System.Windows.Forms.Label();
				aLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
				aLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
				aLabel.Name = "label" + ID;
				aLabel.Size = new System.Drawing.Size(BUTTON_WIDTH, BUTTON_HEIGHT);
				aLabel.Text = "Sensor " + ID;
				aLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

				aLabel.Location = new System.Drawing.Point(lastXLocation, lastYLocation);
				IncrementButtonLocation();

				someLabels.Add (aLabel);
				this.Controls.Add(aLabel);
				this.ResumeLayout(false);
				someActiveIDs.Add (ID);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		public DemoForm(MITesDemoObjectAnalyzer aMITesDOA)
		{
			InitializeComponent();
			this.ClientSize = new System.Drawing.Size(maxXDim, maxYDim);
			this.Refresh ();
			this.aMITesDOA = aMITesDOA;
		}

		/// <summary>
		/// 
		/// </summary>
		public void UpdateDemo()
		{
//			int obj = aMITesObjectAnalyzer683.GetLastValue ();
//			if (obj != MITesData.NO_VALUE)
//				aForm2.Highlight(683);
//			else
//				aForm2.UnHighlight(683);
//
//			obj = aMITesObjectAnalyzer553.GetLastValue ();
//			if (obj != MITesData.NO_VALUE)
//				aForm2.Highlight(553);
//			else
//				aForm2.UnHighlight(553);
//
//			obj = aMITesObjectAnalyzer549.GetLastValue ();
//			if (obj != MITesData.NO_VALUE)
//				aForm2.Highlight(549);
//			else
//				aForm2.UnHighlight(549);
//
//			obj = aMITesObjectAnalyzer736.GetLastValue ();
//			if (obj != MITesData.NO_VALUE)
//				aForm2.Highlight(736);
//			else
//				aForm2.UnHighlight(736);
//
//			obj = aMITesObjectAnalyzer582.GetLastValue ();
//			if (obj != MITesData.NO_VALUE)
//				aForm2.Highlight(582);
//			else
//				aForm2.UnHighlight(582);
//
//			obj = aMITesObjectAnalyzer347.GetLastValue ();
//			if (obj != MITesData.NO_VALUE)
//				aForm2.Highlight(347);
//			else
//				aForm2.UnHighlight(347);
//		
//		
		}		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public void Highlight(int id)
		{
			foreach (System.Windows.Forms.Label label in someLabels)
			{
				if (label.Text == ("Sensor " + id))
				{
					someActiveTimes[id] = Environment.TickCount;
					//Console.WriteLine ("Highlight: " + id);
					label.BackColor = System.Drawing.SystemColors.Highlight;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public void MotionHighlight(int id)
		{
			foreach (System.Windows.Forms.Label label in someLabels)
			{
				if (label.Text == ("Sensor " + id))
				{
					someActiveTimes[id] = 0; //Environment.TickCount;
					Console.WriteLine ("Motion Highlight: " + id);
					label.BackColor = System.Drawing.Color.Crimson;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public void AliveHighlight(int id)
		{
			foreach (System.Windows.Forms.Label label in someLabels)
			{
				if (label.Text == ("Sensor " + id))
				{
					someActiveTimes[id] = Environment.TickCount;
					Console.WriteLine ("Alive Highlight: " + id);
					label.BackColor = System.Drawing.Color.Yellow;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public void UnHighlight(int id)
		{
			foreach (System.Windows.Forms.Label label in someLabels)
			{
				if (label.Text == ("Sensor " + id))
				{
					//Console.WriteLine ("UnHighlight: " + id);
					label.BackColor = System.Drawing.SystemColors.Control;
				}
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// DemoForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 262);
			this.Name = "DemoForm";
			this.Text = "MITes";

		}
		#endregion

	}
}
