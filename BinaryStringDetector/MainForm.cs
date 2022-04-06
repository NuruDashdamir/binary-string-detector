using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BinaryStringDetector
{
    public partial class MainForm : Form
    {
        private List<String> notFoundText;

        public MainForm()
        {
            InitializeComponent();
            notFoundText = new List<String> {"(not found)"};
        }

        public List<String> getValidStringsFromFile(String fileLocation, int minimalDetectedStringLength = 7, bool unicodeOverAscii = false, bool onlyNullTerminated = false)
        {
            byte[] fileContent;
            List<String> validStringList = new List<String>(); 
            
            // handle file
            try {fileContent = System.IO.File.ReadAllBytes(fileLocation);}
            catch {return validStringList;}
            
            String validString = "";
            int validStrLenCounter = 0;
            long startingAdress = 0;

            for (int i = 0; i < fileContent.Length - System.Convert.ToInt32(unicodeOverAscii); i++)
            {
                byte firstByte = fileContent[i];

                //this logic works because of by unicode I mean ascii unicode: 0x0000-0x00FF
                //ascii or unicode logic in next 2 lines
                byte secondByte = 0;
                if (unicodeOverAscii) secondByte = fileContent[++i];

                if ( secondByte == 0
                     && ((firstByte < 127 && firstByte > 31)
                     || (firstByte == 9 || firstByte == 10 || firstByte == 13))
                    )
                {
                    validString += System.Convert.ToChar(firstByte);
                    validStrLenCounter++;
                }
                else
                {
                    if (validStrLenCounter > minimalDetectedStringLength && (!onlyNullTerminated || firstByte == 0))
                    {
                        // print with hexadecimal location of found string
                        validStringList.Add(startingAdress.ToString("X2").PadLeft(8, '0') + " - " + validString);
                    }
                    validStrLenCounter = 0;
                    validString = "";
                    startingAdress = i;
                }
            }

            // in case file is finished and last else is 
            if (!onlyNullTerminated && validString.Length != 0) validStringList.Add(startingAdress.ToString("X2").PadLeft(8, '0') + " - " + validString);
            
            return validStringList;
        }

        private void prepareListboxes(String filename)
        {
            int minimalValidStrLen = (int)numUpDownMinLen.Value;
            listBoxAsciiStrings.DataSource = getValidStringsFromFile(filename, minimalValidStrLen, false, checkBoxOnlyNullTerminated.Checked);
            if (listBoxAsciiStrings.Items.Count == 0) listBoxAsciiStrings.DataSource = notFoundText;
            listBoxUnicodeStrings.DataSource = getValidStringsFromFile(filename, minimalValidStrLen, true, checkBoxOnlyNullTerminated.Checked);
            if (listBoxUnicodeStrings.Items.Count == 0) listBoxUnicodeStrings.DataSource = notFoundText;
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) prepareListboxes(openFileDialog1.FileName);
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            prepareListboxes(System.IO.Path.GetFullPath(s[0]));
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }


    }
}
