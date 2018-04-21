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

namespace SpreadsheetGUI
{
    public partial class Form1 : Form
    {
        AbstractSpreadsheet sheet;
        string currFileName = null;
        bool isCircular = false;
        public bool connected = false;
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

        }

        // Every time the selection changes, this method is called with the
        // Spreadsheet as its parameter.  We display the current time in the cell.

        private void displaySelection(SpreadsheetPanel ss)
        {
            int row, col;
            String value;
            if (connected == true)
                Model.Model.Unfocus();

            ss.GetSelection(out col, out row);
            ss.GetValue(col, row, out value);

            CellNameField.Text = (((Char)((65) + (col))) + "" + (row + 1));

            if (connected == true)
                Model.Model.Focus(CellNameField.Text);


            if (sheet.GetCellValue(CellNameField.Text) is FormulaError err)
            {
                ss.SetValue(col, row, "FORMULA ERROR ");
                CellValueField.Text = "FORMULA ERROR ";
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
                ss.SetValue(col, row, "#REF");
                CellValueField.Text = "#REF";
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
            if (sheet.Changed == true)
            {
                if (MessageBox.Show("You have unsaved changes, do you want to exit?", "Spreadsheet", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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


        public void updateCell(string name, SpreadsheetPanel ss)
        {
            int col = name.ElementAt(0) - 65;
            int row = int.Parse(name.Substring(1)) - 1;

            if (sheet.GetCellCircularStatus(name) == true)
            {
                ss.SetValue(col, row, "#REF");
            }
            if (sheet.GetCellValue(name) is FormulaError err)
            {
                ss.SetValue(col, row, "FORMULA ERROR");
            }
            

            //CS 3505 changes ////////////////////////////////////

            if (sheet.GetCellCircularStatus(name) == true)
            {
                ss.SetValue(col, row, "#REF");
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
            EnterPressed(spreadsheetPanel1);
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
                DialogResult result = MessageBox.Show("Do you want to disconnect and exit?", "Spreadsheet", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);


                if (result == DialogResult.Yes)
                {
                    // Add disconnect functionality...  disconnect();
                    e.Cancel = false;
                    MessageBox.Show("Disconnected (still needs implemented)");
                }
                else if (result == DialogResult.No)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            Model.Model.Undo();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Model.Model.Undo();
        }

        private void RevertButton_Click(object sender, EventArgs e)
        {
            Model.Model.Revert(CellNameField.Text);
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
            spreadsheetPanel1.SetValue(col, row, value);
        }

        private void ContentField_TextChanged(object sender, EventArgs e)
        {

        }

        private void spreadsheetPanel1_Load(object sender, EventArgs e)
        {

        }

        private void CellValueField_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
