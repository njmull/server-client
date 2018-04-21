using Controller;
using SS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using static Controller.Network;

namespace Model
{
    public class Model
    {
        //Terminating character for server messages
        private static char ctrlc = (char)3;
        public static SocketState ss;

        // The functionse below are used to handle client -> server messages

        // Initiate the handshake with the server
        public static void Register(SocketState state)
        {
            ss = state;
            string msg = "register ";
            msg += ctrlc;
            Network.Send(ss.theSocket, msg);
        }

        // Disconnect the client from the server 
        public static void Disconnect()
        {
            string msg = "disconnect ";
            msg += ctrlc;
            Network.Send(ss.theSocket, msg);
            ss.theSocket.Close();
        }

        // Load the spreadsheet with the name <spreadsheet_name> 
        // if it already exists on the server, otherwise create it 
        public static void Load(string sheet)
        {
            string msg = "load ";
            msg += sheet;
            msg += ctrlc;
            Network.Send(ss.theSocket, msg);
        }

        // Pings the server
        public static void Ping()
        {
            string msg = "ping ";
            msg += ctrlc;
            Network.Send(ss.theSocket, msg);
        }

        // Responds to server pings
        public static void Respond()
        {
            string msg = "ping_response ";
            msg += ctrlc;
            Network.Send(ss.theSocket, msg);
        }

        // Request the contents of <cell_id> be set to <cell_contents> 
        public static void Edit(string cell, string content)
        {
            string msg = "edit ";
            msg += cell;
            msg += ":";
            msg += content;
            msg += ctrlc;
            Network.Send(ss.theSocket, msg);
        }

        // Inform the server that this client is editing a specific cell
        public static void Focus(string cell)
        {
            string msg = "focus ";
            msg += cell;
            msg += ctrlc;
            Network.Send(ss.theSocket, msg);
        }

        // Inform the server that the client has left a cell
        public static void Unfocus()
        {
            string msg = "unfocus ";
            msg += ctrlc;
            Network.Send(ss.theSocket, msg);
        }

        // Initiate an undo command on the server
        public static void Undo()
        {
            string msg = "undo ";
            msg += ctrlc;
            Network.Send(ss.theSocket, msg);
        }

        // Initiate a revert command on the server
        public static void Revert(string cell)
        {
            string msg = "revert ";
            msg += cell;
            msg += ctrlc;
            Network.Send(ss.theSocket, msg);
        }
    }
}
