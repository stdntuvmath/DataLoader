using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LootLoaderDataLoaderFormBased
{
    public partial class Form1 : Form
    {

        private static TimeSpan start = new TimeSpan(8, 29, 55); //8:30 military time
        private static TimeSpan now = DateTime.Now.TimeOfDay;//whatever time it is now
        private static DateTime TodaysDate = DateTime.Today;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //if the year has advanced from yesterday to today
            //create a new database table for that year "lootloader_PriceData_currentYear"
            //{........}

            //activate the datacollection process
           
            while (true)
            {
                //get new time
                now= DateTime.Now.TimeOfDay;
                if (start <= now)
                {
                    // TODO: This line of code loads data into the 'lootLoaderDataSet.lootloader_PriceData_2021' table. You can move, or remove it, as needed.
                    backgroundWorker1.RunWorkerAsync();
                    break;

                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            RunMe.RunMeMethod("AMD", 1);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Application.Restart();
            //Environment.Exit(0);
        }
    }
}
