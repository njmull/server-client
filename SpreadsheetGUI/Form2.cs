using Controller;
using System;
using static Model.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Controller.Network;
using SS;
using System.Diagnostics;

namespace SpreadsheetGUI
{
    
    public partial class Form2 : Form
    {
        static bool connected = false;
        static Timer TimeoutClock;
        static int timeLeft = 60;
        static Socket theServer;
        public bool ssclosing = false;

        public Form2()
        {
            InitializeComponent();
        }

        private void TimeoutClock_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                // Display the new time left
                // by updating the Time Left label.
                timeLeft = timeLeft - 1;

                if (timeLeft == 50)
                    Model.Model.Ping();
                if (timeLeft == 40)
                    Model.Model.Ping();
                if (timeLeft == 30)
                    Model.Model.Ping();
                if (timeLeft == 20)
                    Model.Model.Ping();
                if (timeLeft == 10)
                    Model.Model.Ping();
            }
            else
            {
                // If the user ran out of time, stop the timer, show a message.
                TimeoutClock.Stop();
                MessageBox.Show("The server has timed out");
                connected = false;
                Program.MainForm.connected = false;
                Model.Model.Disconnect();
                SetWindowStates();
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (IP_TextBox.Text == "")
            {
                MessageBox.Show("Please enter a server address");
                return;
            }
            if (Port_TextBox.Text == "")
            {
                MessageBox.Show("Please enter a port number");
                return;
            }

            bool connecting = false;
            for (int i = 0; i < 1000; i++)
            {
                if (connecting == false)
                {
                    theServer = Network.ConnectToServer(FirstContact, IP_TextBox.Text, Convert.ToInt32(Port_TextBox.Text));
                    connecting = true;
                }
                if (connected == true)
                    break;
                System.Threading.Thread.Sleep(10);
            }
           
            if (connected == false)
                MessageBox.Show("Unable to connect to server");



            //this.Focus();
            //this.Hide();
        }

        private void FirstContact(SocketState state)
        {
            Register(state);
            connected = true;
            TimeoutClock = new Timer();
            TimeoutClock.Tick += new EventHandler(TimeoutClock_Tick);
            TimeoutClock.Interval = 1000; // 1 second
            this.Invoke(new MethodInvoker(() => TimeoutClock.Start()));
            // Change the action that is take when a network event occurs. Now when data is received,
            // the Networking library will invoke ProcessMessage (see below)
            state.callMe = ProcessMessage;
            Network.GetData(state);
        }

        private void ProcessMessage(SocketState state)
        {
            string totalData = state.sb.ToString();

            // Messages are separated by \3
            char ctrlc = (char)3;
            string[] parts = totalData.Split(ctrlc);


            //do commands
            foreach (string p in parts)
            {
                Receive(p);

                //Output log window for debugging.
                //try
                //    {
                //        this.Invoke(new MethodInvoker(() => OutputLog.AppendText(p)));
                //    }
                //catch { }

                // Then remove the processed message from the SocketState's growable buffer
                state.sb.Remove(0, p.Length);

            }

            // Now ask for more data. This will start an event loop.
            Network.GetData(state);
        }

        // This function will determine what a received message is, and deal with
        // it appropriately.
        public void Receive(string cmd)
        {
            string line = Convert.ToString((char)10);
            // Acknowledge new client and give list of existing spreadsheets. The list may be empty.
            if (cmd.Contains("connect_accepted "))
            {
                cmd = cmd.Substring(16);
                string [] sheets = Regex.Split(cmd, "\n");

                try
                {
                    foreach (string spreadsheet in sheets)
                    {
                        if (spreadsheet == "")
                            continue;

                        spreadsheet.Trim();
                        spreadsheet.TrimStart(' ');
                        this.Invoke(new MethodInvoker(() => ListOfSpreadSheetsBox.Items.Add(spreadsheet)));
                    }
                    ChangeFromConnectToLoad();
                }
                catch { }
                cmd = string.Empty;
            }

            // Unable to open or create the requested spreadsheet 
            if (cmd.Contains("file_load_error "))
            {
                MessageBox.Show("There was an error loading the requested file");
                cmd = string.Empty;
            }

            // Disconnect the client receiving the message
            if (cmd.Contains("disconnect "))
            {
                connected = false;
                cmd = string.Empty;
                Disconnect();
            }

            // Server ping to the client
            if (cmd.Contains("ping "))
            {
                Respond();
                cmd = string.Empty;
            }

            // Server response to ping from the client
            if (cmd.Contains("ping_response "))
            {
                //reset our timeout timer, whereever that may be
                timeLeft = 60;
                cmd = string.Empty;
            }

            // The full state of the spreadsheet, with the id and contents of every non-empty cell
            if (cmd.Contains("full_state "))
            {
                
                string temp = cmd.Substring(cmd.IndexOf(' ') + 1);
                string[] sheets = Regex.Split(temp, line);             
                foreach(string s in sheets)
                {
                    if (s == string.Empty)
                        break;

                    string[] pairs = s.Split(new char[] { ':' }, 2);
                    if (pairs.Length > 1)
                    {
                        string cell = pairs[0];
                        string value = pairs[1];
                        this.Invoke(new MethodInvoker(() => UpdateCell(cell, value)));
                    }

                }
                cmd = string.Empty;
            }

            // Notification that the contents of <cell_id> is now <cell_contents>, 
            // following an edit, undo, or revert
            if (cmd.Contains("change "))
            {
                string temp = cmd.Substring(cmd.IndexOf(' ') + 1);
                string[] pairs = temp.Split(new char[] { ':' }, 2);
                if (pairs.Length > 1)
                {
                    string cell = pairs[0];
                    string value = pairs[1];
                    this.Invoke(new MethodInvoker(() => UpdateCell(cell, value)));
                }
                
                cmd = string.Empty;

            }

            // Notify clients that a user, represented by a unique string 
            // <user_id> is in the process of editing the specified cell.
            if (cmd.Contains("focus "))
            {
                if (cmd.Contains("unfocus ") != true)
                {
                    string temp = cmd.Substring(cmd.IndexOf(' ') + 1);
                    string[] pairs = temp.Split(':');
                    if (pairs.Length > 1)
                    {
                        string cell = pairs[0];
                        string name = pairs[1];
                        int id = Int32.Parse(name);
                        Program.MainForm.UpdateFocus(cell, id);
                    }
                    cmd = string.Empty;
                }
            }

            // Notify clients that a user, represented by a unique string
            // <user_id> is no longer in the process of
            if (cmd.Contains("un"))
            {
                string temp = cmd.Substring(cmd.IndexOf(' ') + 1);
                int id = Int32.Parse(temp);
                Program.MainForm.Unfocus(id);
                cmd = string.Empty;
            }
        }

        // Called when the client recieves incoming cell changes from the server
        // This function will set the value of the cell to the value provided
        // If an invalid cell is given, the value will result in a formula error.
        public static void UpdateCell(string cell, string value)
        {
            Program.MainForm.UpdateReceivedCellContents(cell, value);
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            connected = false;
            Program.MainForm.Close();
            this.Close();
        }

        private void ChangeFromConnectToLoad()
        {
            this.Invoke(new MethodInvoker(() => ServerLabel.Text = "Connected"));
            this.Invoke(new MethodInvoker(() => IP_TextBox.ReadOnly = true));
            this.Invoke(new MethodInvoker(() => Port_TextBox.ReadOnly = true));
            this.Invoke(new MethodInvoker(() => ConnectButton.Visible = false));
            this.Invoke(new MethodInvoker(() => LoadSpreadsheetButton.Visible = true));
            this.Invoke(new MethodInvoker(() => LoadFileTextBox.Visible = true));
            this.Invoke(new MethodInvoker(() => ListOfSpreadSheetsBox.Visible = true));
            this.Invoke(new MethodInvoker(() => IP_TextBox.TabStop = false));
            this.Invoke(new MethodInvoker(() => Port_TextBox.TabStop = false));
            this.Invoke(new MethodInvoker(() => LoadFileTextBox.Focus()));


            connected = true;
            Program.MainForm.connected = true;
        }

        private void LoadSpreadsheetButton_Click(object sender, EventArgs e)
        {
            if (LoadFileTextBox.Text != string.Empty)
            {
                Model.Model.Load(LoadFileTextBox.Text);
                SetWindowStates();
            }
            else
                MessageBox.Show("Please enter a spreadsheet to open or create.");
        }
        
        public void SetWindowStates()
        {
            if(connected == true)
            {
                Program.MainForm.Enabled = true;
                this.Invoke(new MethodInvoker(() => Program.MainForm.WindowState = FormWindowState.Normal));
                Program.MainForm.Show();
                Program.MainForm.Focus();
                ///
                Program.MainForm.Text = LoadFileTextBox.Text;
                ///
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
                this.Enabled = false;
            }
            if(connected == false)
            {
                this.WindowState = FormWindowState.Normal;
                this.Show();
                this.Enabled = true;
            }

        }

        private void ListOfSpreadSheetsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListOfSpreadSheetsBox.SelectedItem != null)
            {
                string curItem = ListOfSpreadSheetsBox.SelectedItem.ToString();
            
                string curItemTrimmed = curItem.Trim();

                LoadFileTextBox.Text = curItemTrimmed;
            }


        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Model.Model.Unfocus();
            if(ssclosing != true)
                Program.MainForm.Close();
        }
    }
}
