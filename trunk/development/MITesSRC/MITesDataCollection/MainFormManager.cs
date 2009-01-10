using System;
using System.Windows.Forms;



namespace WocketsInterface
{
    public class MainFormManager
    {


        public static bool exitAppOnClose;

        public static Form MainForm;
        public static Form PrevForm;

        
        public Form CurrentForm
        {
            get { return MainForm; }

            set
            {

                if (MainForm != null)
                {
                    // close the current form, but don't exit the application
                    exitAppOnClose = false;
                    PrevForm = MainForm;

                    // show the new form
                    MainForm = value;
                    MainForm.Show();

                    PrevForm.Hide();
                    exitAppOnClose = true;

                }
                else
                { // show the new form
                    MainForm = value;
                    MainForm.Show();
                }
            }
        }

     
        public MainFormManager()
        {
            exitAppOnClose = true;
        }

       
    }//end class
}//end namespace
