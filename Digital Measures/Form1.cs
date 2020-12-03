using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.Drawing.Drawing2D;

namespace Digital_Measures
{
    public partial class Form1 : Form
    {
        private string sfdPath = "";
        public Form1()
        {
            InitializeComponent();
        }

        OpenFileDialog ofd = new OpenFileDialog();
        //open file button
        private void button2_Click(object sender, EventArgs e)
        {
            //This filter makes it so you can only open csv files
            ofd.Filter = "CSV|*.csv";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //show file name and extension
                textBox2.Text = ofd.SafeFileName;
            }
        }
        //select destination folder
        private void button3_Click(object sender, EventArgs e)
        {
            //this filter makes it so it automatically saves as a csv
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV|*.csv";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //show file path
                textBox3.Text = sfd.FileName;
                sfdPath = sfd.FileName;
            }
        }
        //convert file button
        private void button1_Click(object sender, EventArgs e)
        {
            //used background worker so the updating of the progress bar is done more accurately
            backgroundWorker1.RunWorkerAsync();
        }
        
        private void progressBar1_Click(object sender, EventArgs e)
        {
            
        }
        //done button
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int progress = 0;
            TextFieldParser parser = new TextFieldParser(ofd.FileName);
            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");
            string[] line = { };
            List<string> lineList = new List<string>();
            //reads in the csv file and discards commas
            while (!parser.EndOfData)
            {
                //reads the current lines and stores that in the line array
                line = parser.ReadFields();
                //adds each element of the array into an arraylist
                foreach (string field in line)
                {
                    if (field.Contains(","))
                    {
                        lineList.Add("\"" + field + "\"");
                    }
                    else
                    {
                        lineList.Add(field);
                    }
                }

            }
            parser.Close();
            //removing empty elements from arraylist
            for (int i = 0; i < lineList.Count; i++)
            {
                if (lineList[i] == "")
                {
                    lineList.RemoveAt(i);
                }
            }
            //converting arraylist into string
            string[] s1 = lineList.ToArray();
            progress += 25;
            backgroundWorker1.ReportProgress(progress);
            HeaderConversion(s1);
            //convert to proper headers
            void HeaderConversion(string[] fileLine)
            {
                //making an array of the correct headers we want
                string[] correctHeader = new string[] { "Instructor_ID", "TYT_TERM", "TYY_YEAR", "TITLE", "COURSEPRE", "COURSENUM", "SECTION", "ENROLL", "CHOURS", "DELIERY_MODE\n" };
                //replacing the old headers with the new ones
                for (int i = 0; i < 10; i++)
                {
                    fileLine[i] = correctHeader[i];
                }
                DeliveryModeConversion(fileLine);
                progress += 25;
                backgroundWorker1.ReportProgress(progress);
            }
            //convert to proper delivery modes
            void DeliveryModeConversion(string[] fileLine)
            {
                //created a dictionary of the old(left)/new(right) delivery modes
                Dictionary<string, string> deliveryModes = new Dictionary<string, string>(){
                {"Lab with Credit - Undergrad", "12 Lab Credit UG\n" },
                {"Traditional UG (0% online)", "00A Traditional UG\n" },
                {"Arranged Graduate", "08 Arranged GR\n" },
                {"Blended GR (50 to <75% online)", "23A Blended GR\n" },
                {"\"Partial UG (>0%, <50% online)\"", "30A Partial UG\n" },
                {"Blended UG (50 to <75% online)", "22A Blended UG\n" },
                {"Interactive Video - Undergrad", "14 ITV UG\n" },
                {"Lab with No Credit", "03 Lab No Credit\n" },
                {"Virtual Class Meeting UG", "34 VCM UG\n" },
                {"Virtual Class Meeting GR", "35 VCM GR\n" },
                {"Online UG (75% or more online)", "20A Online UG\n" },
                {"Online GR (75% or more online)", "21A Online GR\n" },
                {"Arranged Undergraduate", "04 Arranged UG\n" },
                {"Traditional GR (0% online)", "01A Traditional GR\n" },
                {"Internship - Graduate", "17 Internship GR\n" },
                {"Arranged Music - Undergraduate", "06 Arr Music UG\n" },
                {"Zero Level", "09 Zero Level\n" },
                {"Interactive Video - Graduate", "15 ITV GR\n" },
                {"\"Partial GR (>0%, <50% online)\"", "31A Partial GR\n" },
                {"Clinical - Graduate", "19 Clinical GR\n" },
                {"Clinical - Undergraduate", "18 Clinical UG\n" },
                {"Arranged Music - No Credit", "11 Arr Music No Credit\n" },
                {"Lab with Credit - Graduate", "13 Lab Credit GR\n" },
                {"Internship - Undergraduate", "16 Internship UG\n" },
                {"CEL UG (75% or more online)", "24A CEL UG\n" },
                {"CEL GR (75% or more online)", "25A CEL GR\n" },
                {"CEL - Arranged - Undergraduate", "28 CEL Arr UG\n" },
                {"\"CEL Partial UG (>0%, <50% online)\"", "32A CEL Partial UG\n" },
                {"\"CEL Partial GR (>0%, <50% online)\"", "33A CEL Partial GR\n" },
                {"CEL Internship - Undergraduate", "29 CEL Internship UG\n" },
                {"CEL Blended GR (50 to <75% online)", "27A CEL Blended GR\n" },
                {"CEL Blended UG (50 to <75% online)", "26A CEL Blended UG\n" },
            };
                //goes through array looking for the key (old delivery modes)
                //replaces key with the value (new delivery modes)
                for (int i = 0; i < fileLine.Length; i++)
                {
                    //trimming each element of spaces, new lines , and carriage return
                    char[] trimChars = { ' ', '\n', '\r' };
                    String cleanWord = fileLine[i].Trim(trimChars);
                    foreach (KeyValuePair<String, String> pair in deliveryModes)
                    {
                        if (string.Equals(cleanWord, pair.Key) == true)
                        {
                            fileLine[i] = pair.Value;
                        }
                    }
                }
                progress += 25;
                backgroundWorker1.ReportProgress(progress);
                CombineBack(fileLine);
            }
            //combines back to proper csv format
            void CombineBack(string[] fileLine)
            {
                string endString = "";
                //joining all array elements to string seperated by comma
                for (int i = 0; i < fileLine.Length; i++)
                {
                    endString = String.Join(",", fileLine);
                }
                //writing to the csv file
                using (StreamWriter sw = new StreamWriter(sfdPath))
                {
                    sw.WriteLine(endString);
                }
                progress += 25;
                backgroundWorker1.ReportProgress(progress);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int progress = 0;
            int i = 0;
            progress += e.ProgressPercentage;
            while (progressBar1.Value != 100)
            {
                progressBar1.Value = i;
                i++;
                if (i >= progress)
                {
                    System.Threading.Thread.Sleep(50);
                }
                System.Threading.Thread.Sleep(40);
            }
        }
        //completed message box
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Successfully Converted File");
        }
        //clear button
        private void button5_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            textBox2.Text = "";
            textBox3.Text = "";
        }
    }
}
