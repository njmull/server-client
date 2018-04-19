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

namespace SpreadsheetGUI
{
    
    public partial class Form2 : Form
    {
        static bool connected = false;
        static Timer TimeoutClock;
        static int timeLeft = 60;
        static Socket theServer;

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
            }
            else
            {
                // If the user ran out of time, stop the timer, show
                // a MessageBox, and fill in the answers.
                TimeoutClock.Stop();
                MessageBox.Show("The server has timed out");
                Model.Model.Disconnect();
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

            theServer = Network.ConnectToServer(FirstContact,IP_TextBox.Text, Convert.ToInt32(Port_TextBox.Text));


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
            TimeoutClock.Start();
            // Change the action that is take when a network event occurs. Now when data is received,
            // the Networking library will invoke ProcessMessage (see below)
            state.callMe = ProcessMessage;

            // If this was the SpaceWars game, there would be one more step of the handshake, and we wouldn't
            // go straight in to ProcessMessage

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

                try
                {
                    this.Invoke(new MethodInvoker(() => OutputLog.AppendText(p)));
                }
                catch { }

                // Then remove the processed message from the SocketState's growable buffer
                //state.ReceiveBuffer.Remove(0, p.Length);

            }

            // Now ask for more data. This will start an event loop.
            Network.GetData(state);
        }

        // This function will determine what a received message is, and deal with
        // it appropriately.
        public static void Receive(string cmd)
        {
            // Acknowledge new client and give list of existing spreadsheets. The list may be empty.
            if (cmd.Contains("connection_accepted "))
            {
                string[] sheets = Regex.Split(cmd, "\n");

                // Do gui stuff to get the sheet

                //foreach (string spreadsheet in sheets)
                //{
                //    ListOfSpreadSheetsBox.Items.Add(spreadsheet);
                //}

                //ChangeFromConnectToLoad();


                // request to load/create the selected sheet
                //Network.Send(theServer,selected)
            }

            // Unable to open or create the requested spreadsheet 
            if (cmd.Contains("file_load_error "))
            {
                MessageBox.Show("There was an error loading the requested file");
            }

            // Disconnect the client receiving the message
            if (cmd.Contains("disconnect "))
            {
                connected = false;
                Disconnect();
            }

            // Server ping to the client
            if (cmd.Contains("ping "))
            {
                Respond();
            }

            // Server response to ping from the client
            if (cmd.Contains("ping_response "))
            {
                //reset our timeout timer, whereever that may be
                timeLeft = 60;
            }

            // The full state of the spreadsheet, with the id and contents of every non-empty cell
            if (cmd.Contains("full_state "))
            {
                string temp = cmd.Substring(cmd.IndexOf(' ') + 1);
                string[] sheets = Regex.Split(temp, "\n");
                foreach(string s in sheets)
                {
                    string[] pairs = s.Split(':');
                    string cell = pairs[0];
                    string value = pairs[1];
                    UpdateCell(cell, value);
                }
            }

            // Notification that the contents of <cell_id> is now <cell_contents>, 
            // following an edit, undo, or revert
            if (cmd.Contains("change "))
            {
                string temp = cmd.Substring(cmd.IndexOf(' ') + 1);
                string[] pairs = temp.Split(':');
                string cell = pairs[0];
                string value = pairs[1];
                UpdateCell(cell, value);

            }

            // Notify clients that a user, represented by a unique string 
            // <user_id> is in the process of editing the specified cell.
            if (cmd.Contains("focus "))
            {
                string temp = cmd.Substring(cmd.IndexOf(' ') + 1);
                string[] pairs = temp.Split(':');
                string cell = pairs[0];
                string value = pairs[1];
                //Focus(cell, value);
            }

            // Notify clients that a user, represented by a unique string
            // <user_id> is no longer in the process of
            if (cmd.Contains("unfocus "))
            {
                string temp = cmd.Substring(cmd.IndexOf(' ') + 1);
                string[] pairs = temp.Split(':');
                string cell = pairs[0];
                string value = pairs[1];
                //unFocus(cell, value);
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
            this.Close();
        }

        private void NewSpreadsheetButton_Click(object sender, EventArgs e)
        {

        }

        private void OutputLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void ChangeFromConnectToLoad()
        {
            ServerLabel.Text = "Connected";
            IP_TextBox.ReadOnly = true;
            Port_TextBox.ReadOnly = true;
            ConnectButton.Visible = false;
            NewSpreadsheetButton.Visible = true;
            LoadSpreadsheetButton.Visible = true;
            LoadFileTextBox.Visible = true;
            
        }

    }
}
