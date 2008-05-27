using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HousenCS.MITes;
using System.Threading;

namespace MITesDataCollection
{
    public partial class ReceiverConfigureForm : Form
    {
        private MITesReceiverController mrc;

        private int[] currentIDs = new int[6];
        private int numCurrentIDs = 0;
        private int[] newIDs = new int[6];
        private int numNewIDs = 0;
        private MITesActivityLogger aMITesActivityLogger;

        /// <summary>
        /// A form used to manually add unusual sensor configurations and set the MITesReceiver controller.
        /// </summary>
        /// <param name="mrc">The controller operating the MITesReceiver</param>
        /// <param name="mainForm">The main form of the application</param>
        /// <param name="aMITesActivityLogger">Logging object for comments</param>
        public ReceiverConfigureForm(MITesReceiverController mrc, MITesDataCollectionForm mainForm, MITesActivityLogger aMITesActivityLogger)
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
            numCurrentIDs = mrc.ReadChannels(currentIDs);
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
            if (IsInIDs(id, newIDs))
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
                mrc.SetChannels(numNewIDs, newIDs);
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