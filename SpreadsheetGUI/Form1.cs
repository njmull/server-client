// Written by 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;
using SpreadsheetUtilities;
using System.Diagnostics;

namespace SpreadsheetGUI
{
    public partial class Form1 : Form
    {
        AbstractSpreadsheet sheet;
        string currFileName = null;
        bool isCircular = false;
        public bool connected = false;
        private const int DATA_COL_WIDTH = 80;
        private const int DATA_ROW_HEIGHT = 22;
        private const int LABEL_COL_WIDTH = 30;
        private const int LABEL_ROW_HEIGHT = 30;
        private const int PADDING = 2;
        private const int SCROLLBAR_WIDTH = 20;
        private const int COL_COUNT = 26;
        private const int ROW_COUNT = 99;
        Stopwatch Stopwatch = new Stopwatch();


        /// <summary>
        /// Constructor for the Spreadsheet.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            sheet = new Spreadsheet(s => true, s => s.ToUpper(), "ps6");

            spreadsheetPanel1.SelectionChanged += displaySelection;

            
            
            spreadsheetPanel1.SetSelection(0, 0);
            displaySelection(spreadsheetPanel1);
            spreadsheetPanel1.SendToBack();
            spreadsheetPanel1.Focus();
            ContentField.BringToFront();
            ContentField.Focus();
            this.Hide();
        }

        public Form1(Spreadsheet sheet)
        {
            InitializeComponent();
            this.sheet = sheet;

            foreach (String cellName in sheet.GetNamesOfAllNonemptyCells())
            {
                updateCell(cellName, spreadsheetPanel1);
            }

            spreadsheetPanel1.SelectionChanged += displaySelection;

            spreadsheetPanel1.SetSelection(0, 0);
            displaySelection(spreadsheetPanel1);
            spreadsheetPanel1.SendToBack();
            spreadsheetPanel1.Focus();
            ContentField.BringToFront();
            ContentField.Focus();

            

        }

        // Every time the selection changes, this method is called with the
        // Spreadsheet as its parameter.  We display the current time in the cell.

        private void displaySelection(SpreadsheetPanel ss)
        {
            // Used for periodically checking for cirular dependencies
            //if (Stopwatch.IsRunning == false)
            //    Stopwatch.Start();

            //if(Stopwatch.ElapsedMilliseconds  60000)
            //{
            //    PeriodicCircCheck(ss);
            //    Stopwatch.Reset();
            //}

            int row, col;
            String value;

            if (connected == true)
                Model.Model.Unfocus();

            ss.GetSelection(out col, out row);
            ss.GetValue(col, row, out value);

            CellNameField.Text = (((Char)((65) + (col))) + "" + (row + 1));

            if (connected == true)
                Model.Model.Focus(CellNameField.Text);

            spreadsheetPanel1.GetFirstSelection(out int fcol, out int frow);

            int xpos = LABEL_COL_WIDTH + (col - fcol) * DATA_COL_WIDTH + 1;
            int ypos = LABEL_ROW_HEIGHT + (row - frow) * DATA_ROW_HEIGHT + 1;

            ContentField.Location = new Point(xpos, ypos);
            ContentField.BringToFront();
            ContentField.Focus();
            ContentsBar.Text = ContentField.Text;

            if (sheet.GetCellValue(CellNameField.Text) is FormulaError err)
            {
                ss.SetValue(col, row, "#REF ");
                CellValueField.Text = "#REF ";
            }

            else
            {
                ss.SetValue(col, row, sheet.GetCellValue(CellNameField.Text) + "");
                CellValueField.Text = sheet.GetCellValue(CellNameField.Text) + "";
            }



            if (sheet.GetCellContents(((Char)((65) + (col))) + "" + (row + 1)) is Formula form)
                ContentField.Text = "=" + sheet.GetCellContents(((Char)((65) + (col))) + "" + (row + 1));
            else
                ContentField.Text = "" + sheet.GetCellContents(((Char)((65) + (col))) + "" + (row + 1));

            //CS3505 changes///////////////////////////////////////////////////

            //Checks to see if a circular dependency was found and display #REF in the selected cell on update if it was
            if (isCircular == true)
            {
                MessageBox.Show("The formula you entered results in a circular exception.");
                ss.SetValue(col, row, "#REF ");
                CellValueField.Text = "#REF ";
                isCircular = false;
            }

            //end//////////////////////////////////////////////////////////////
            ContentField.Focus();


        }

        // Deals with the New menu
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tell the application context to run the form on the same
            // thread as the other forms.
            SpreadsheetApplicationContext.getAppContext().RunForm(new Form1());
        }

        // Deals with the Close menu
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {


            if (MessageBox.Show("Do you want to disconnect and exit?", "Spreadsheet", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                connected = false;
                Program.ConnectForm.Close();
                Close();
            }
            

        }

        private void EnterPressed(SpreadsheetPanel ss)
        {
            int row, col;

            ss.GetSelection(out col, out row);

            String name = CellNameField.Text;

            ISet<String> altered = null;
            ISet<String> circular = null;
            Model.Model.Edit(name, ContentField.Text);

            try
            {
                altered = sheet.SetContentsOfCell(name, ContentField.Text);
                circular = new HashSet<string>();

                HashSet<string> visited = new HashSet<string>();

                //CS 3505 changes ////////////////////////////////////


                foreach (string cell in sheet.GetDirectDependents(name))
                {
                    if (!visited.Contains(cell))
                    {
                        CircularDependencyCheck(cell, cell, visited, circular);
                    }
                    circular.Add(cell);
                }


                foreach (String s in altered)
                {
                    updateCell(s, ss);
                }

                foreach (string c in circular)
                {
                    sheet.SetCellCircularStatus(c, false);
                }


                //    //end ////////////////////////////////////////////////

                displaySelection(ss);

            }
            catch (FormulaFormatException e)
            {
                MessageBox.Show("The formula you entered is not valid.");
            }
            catch (CircularException e)
            {
                MessageBox.Show("The formula you entered results in a circular exception.");
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to change cell value; " + e.Message);
            }
        }

        /// <summary>
        /// Recursively check for circular dependency then modifies the cells circular status 
        /// accordingly and adds that cells name to a list for further checks.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="name"></param>
        /// <param name="visited"></param>
        /// <param name="altered"></param>
        public void CircularDependencyCheck(string start, string name, ISet<string> visited, ISet<string> altered)
        {
            visited.Add(name);
            foreach (string cell in sheet.GetDirectDependents(name))
            {
                if (cell.Equals(start))
                {
                    isCircular = true;
                    foreach (string c in sheet.GetDirectDependents(start))
                    {
                        sheet.SetCellCircularStatus(c, true);
                        altered.Add(c);
                        ////////
                        //CircularDependencyCheck(c, c, visited, altered);
                        /////////
                    }
                    sheet.SetCellCircularStatus(start, true);
                }
                else if (!visited.Contains(cell))
                {
                    CircularDependencyCheck(start, cell, visited, altered);
                }
            }
        }

        //public void PeriodicCircCheck(SpreadsheetPanel ss)
        //{
        //    HashSet<string> visited = new HashSet<string>();
        //    ISet<String> circular = new HashSet<string>();

        //    foreach (string name in sheet.GetNamesOfAllNonemptyCells())
        //    {
        //        foreach (string cell in sheet.GetDirectDependents(name))
        //        {
        //            if (!visited.Contains(cell))
        //            {
        //                CircularDependencyCheck(cell, cell, visited, circular);
        //            }
        //            circular.Add(cell);
        //        }

        //        foreach (String s in circular)
        //        {
        //            updateCell(s, ss);
        //        }

        //        foreach (string c in circular)
        //        {
        //            sheet.SetCellCircularStatus(c, false);
        //        }
        //    }
        //}



        public void updateCell(string name, SpreadsheetPanel ss)
        {
            int col = name.ElementAt(0) - 65;
            int row = int.Parse(name.Substring(1)) - 1;

            if (sheet.GetCellCircularStatus(name) == true)
            {
                ss.SetValue(col, row, "#REF ");
            }
            if (sheet.GetCellValue(name) is FormulaError err)
            {
                ss.SetValue(col, row, "#REF ");
            }
            //else
            //    ss.SetValue(col, row, sheet.GetCellValue(name) + "");


            //CS 3505 changes ////////////////////////////////////

            if (sheet.GetCellCircularStatus(name) == true)
            {
                ss.SetValue(col, row, "#REF ");
            }
            else if (sheet.GetCellCircularStatus(name) == false)
                ss.SetValue(col, row, sheet.GetCellValue(name) + "");

            //end ////////////////////////////////////////////////

        }

        private void EnterButton_Click(object sender, EventArgs e)
        {
            EnterPressed(spreadsheetPanel1);
        }

        private void ContentField_Validating(object sender, CancelEventArgs e)
        {
            //EnterPressed(spreadsheetPanel1);
        }

        private void ContentField_KeyDown(object sender, KeyEventArgs e)
        {
            ChangeCell(e.KeyCode);
        }

        private void ChangeCell(Keys e)
        {
            int row, col;

            spreadsheetPanel1.GetSelection(out col, out row);
            

            switch (e)
            {
                case Keys.Enter:
                    if ((row + 1) < 99)
                    {
                        EnterPressed(spreadsheetPanel1);

                        spreadsheetPanel1.SetSelection(col, row + 1);
                        displaySelection(spreadsheetPanel1);
                    }
                    break;
                case Keys.Tab:
                    if (((Char)((65) + (col))) < 'Z')
                    {
                        EnterPressed(spreadsheetPanel1);

                        spreadsheetPanel1.SetSelection(col + 1, row);
                        displaySelection(spreadsheetPanel1);
                    }
                    break;
                case Keys.Right:
                    if (((Char)((65) + (col))) < 'Z')
                    {
                        EnterPressed(spreadsheetPanel1);

                        spreadsheetPanel1.SetSelection(col + 1, row);
                        displaySelection(spreadsheetPanel1);
                    }
                    break;
                case Keys.Left:
                    if (((Char)((65) + (col))) > 'A')
                    {
                        EnterPressed(spreadsheetPanel1);

                        spreadsheetPanel1.SetSelection(col - 1, row);
                        displaySelection(spreadsheetPanel1);
                    }
                    break;
                case Keys.Up:
                    if ((row + 1) > 1)
                    {
                        EnterPressed(spreadsheetPanel1);

                        spreadsheetPanel1.SetSelection(col, row - 1);
                        displaySelection(spreadsheetPanel1);
                    }

                    break;
                case Keys.Down:
                    if ((row + 1) < 99)
                    {
                        EnterPressed(spreadsheetPanel1);

                        spreadsheetPanel1.SetSelection(col, row + 1);
                        displaySelection(spreadsheetPanel1);
                    }

                    break;
            }
        }

        private void spreadsheetPanel1_KeyDown(object sender, KeyEventArgs e)
        {
            ChangeCell(e.KeyCode);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image  
            // assigned to Button2.  
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Spreadsheet File|*.sprd|All Files|*";
            saveFileDialog1.Title = "Save a Spreadsheet File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                try
                {
                    sheet.Save(saveFileDialog1.FileName);
                    currFileName = saveFileDialog1.FileName;
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Unable to save Spreadsheet file: " + exception.Message);
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Displays an OpenFileDialog so the user can select a Cursor.  
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Spreadsheet File|*.sprd|All Files|*";
            openFileDialog1.Title = "Open a Spreadsheet File";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;

                if (sheet.GetSavedVersion(filename) != "ps6")
                {
                    MessageBox.Show("The version of this spreadsheet file does not match \"ps6\".");
                    return;
                }

                currFileName = filename;

                sheet = new Spreadsheet(filename, s => true, s => s.ToUpper(), "ps6");

                foreach (String cellName in sheet.GetNamesOfAllNonemptyCells())
                {
                    updateCell(cellName, spreadsheetPanel1);
                }

                spreadsheetPanel1.SelectionChanged += displaySelection;

                spreadsheetPanel1.SetSelection(0, 0);
                displaySelection(spreadsheetPanel1);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sheet.Changed == true)
            {
                //DialogResult result = MessageBox.Show("You have unsaved changes, do you want to save?", "Spreadsheet", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                DialogResult result = MessageBox.Show("Do you want to disconnect and exit?", "Spreadsheet", MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                if (result == DialogResult.Yes)
                {
                    // Add disconnect functionality...  disconnect();
                    e.Cancel = false;
                    connected = false;
                    Model.Model.Unfocus();
                    Model.Model.Disconnect();
                    Program.ConnectForm.ssclosing = true;
                    Program.ConnectForm.Close();
                   
                }
                else if (result == DialogResult.No)
                    e.Cancel = true;
                //else
                //    e.Cancel = true;
            }
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            Model.Model.Undo();
            ContentField.Invalidate();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Model.Model.Undo();
        }

        private void RevertButton_Click(object sender, EventArgs e)
        {
            Model.Model.Revert(CellNameField.Text);
            ContentField.Invalidate();
        }

        private void revertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Model.Model.Revert(CellNameField.Text);
        }

        public void UpdateReceivedCellContents(string name, string value)
        {
            
            int col = name.ElementAt(0) - 65;
            int row = int.Parse(name.Substring(1)) - 1;
            sheet.SetContentsOfCell(name, value);
            string cellVal = sheet.GetCellValue(name).ToString();
            spreadsheetPanel1.SetValue(col, row, cellVal);
            this.Invoke(new MethodInvoker(() => displaySelection(spreadsheetPanel1)));

        }
        public void UpdateFocus(string cell, int id)
        {
            int col = cell.ElementAt(0) - 65;
            int row = int.Parse(cell.Substring(1)) - 1;
            spreadsheetPanel1.AddFocus(id, col, row);
            spreadsheetPanel1.Invalidate();
        }

        public void Unfocus(int id)
        {
            spreadsheetPanel1.RemoveFocus(id);
        }

        private void ContentField_TextChanged(object sender, EventArgs e)
        {
            ContentsBar.Text = ContentField.Text;
        }

        private void spreadsheetPanel1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void newToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            HashSet<string> nonempty = new HashSet<string>();

            foreach (string s in sheet.GetNamesOfAllNonemptyCells())
                nonempty.Add(s);
            foreach (string s in nonempty)
                sheet.SetContentsOfCell(s, string.Empty);

            this.Enabled = false;
            this.Hide();
            this.WindowState = FormWindowState.Minimized;
            this.Invoke(new MethodInvoker(() => Program.ConnectForm.Visible = true));
            this.Invoke(new MethodInvoker(() => Program.ConnectForm.Enabled = true));
            this.Invoke(new MethodInvoker(() => Program.ConnectForm.Show()));
            this.Invoke(new MethodInvoker(() => Program.ConnectForm.Focus()));
            this.Invoke(new MethodInvoker(() => Program.ConnectForm.WindowState = FormWindowState.Normal));
            Model.Model.Unfocus();
            spreadsheetPanel1.Clear();
            this.spreadsheetPanel1.Invalidate();


        }

        private void ContentField_KeyPress(object sender, KeyPressEventArgs e)
        {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    
                }
            
        }
    }
}
