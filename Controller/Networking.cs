/*
 * Collaborative Spreadsheet Server
 * CS 3505 - Spring 2018 - Peter Jensen
 * 
 * Authors:
 * Emerson Ford (u0407846)
 * Ben Hannah (u0426915)
 * Nicholas Mull (u0669977)
 * 
 * This code gives our Spreadsheet client networking capabilities. It has been coded to be
 * agnostic to the context in which it is being used. This code is heavily based on code
 * that was written for the SpaceWar Project in CS 3500, originally written by Emerson Ford.
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Controller
{
    /// <summary>
    /// The callback function for our network event loops.
    /// </summary>
    /// <param name="state">The socket state on which the function will be called on.</param>
    //public delegate void NetworkAction(SocketState state);

    /// <summary>
    /// A class used to keep track of messages received and the socket for the given ID.
    /// </summary>
    //public class SocketState
    //{
    //    public Socket Socket;
    //    public int ID;
    //    public Boolean Connected = false;

    //    // This is the buffer where we will receive data from the socket
    //    public byte[] MessageBuffer = new byte[1024];

    //    // This is a larger (growable) buffer, in case a single receive does not contain the full message.
    //    public StringBuilder ReceiveBuffer = new StringBuilder();

    //    // This is how the networking library will "notify" users when a connection is made, or when data is received.
    //    public NetworkAction CallbackMethod;

    //    public SocketState(Socket socket, int id)
    //    {
    //        Socket = socket;
    //        ID = id;
    //    }
    //}

    public class Networking
    {
        /// <summary>
        /// Creates a socket connection to the given hostname.
        /// </summary>
        /// <param name="hostName">The hostname to attempt to connect to.</param>
        /// <param name="socket">The resulting socket.</param>
        /// <param name="ipAddress">The IP address we have connected to.</param>
    //    public static void MakeSocket(string hostName, out Socket socket, out IPAddress ipAddress)
    //    {
    //        ipAddress = IPAddress.None;
    //        socket = null;

    //        try
    //        {
    //            // Establish the remote endpoint for the socket.
    //            IPHostEntry ipHostInfo;

    //            // Determine if the server address is a URL or an IP
    //            try
    //            {
    //                ipHostInfo = Dns.GetHostEntry(hostName);
    //                bool foundIPV4 = false;

    //                foreach (IPAddress addr in ipHostInfo.AddressList)
    //                {
    //                    if (addr.AddressFamily != AddressFamily.InterNetworkV6)
    //                    {
    //                        foundIPV4 = true;
    //                        ipAddress = addr;
    //                        break;
    //                    }
    //                }

    //                // Didn't find any IPV4 addresses
    //                if (!foundIPV4)
    //                {
    //                    throw new ArgumentException("Invalid address.");
    //                }
    //            }

    //            // Check if the hostname is an IP address.
    //            catch (Exception)
    //            {
    //                ipAddress = IPAddress.Parse(hostName);
    //            }

    //            // Create a TCP/IP socket.
    //            socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    //            socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

    //            // Disable Nagle's algorithm - can speed things up for tiny messages, 
    //            // such as for a game
    //            socket.NoDelay = true;

    //        }

    //        catch (Exception e)
    //        {
    //            System.Diagnostics.Debug.WriteLine("Unable to create socket. Error occured: " + e);
    //            throw new ArgumentException("Invalid address");
    //        }
    //    }

    //    /// <summary>
    //    /// Attempts to connect to the given hostname, then calls the given delegate.
    //    /// </summary>
    //    /// <param name="callback"></param>
    //    /// <param name="hostname"></param>
    //    /// <param name="port"></param>
    //    /// <returns></returns>
    //    public static SocketState ConnectToServer(NetworkAction callback, string hostname, int port)
    //    {
    //        MakeSocket(hostname, out Socket socket, out IPAddress address);

    //        SocketState ConnectedServer = new SocketState(socket, -1)
    //        {
    //            CallbackMethod = callback
    //        };

    //        ConnectedServer.Socket.BeginConnect(address, port, ConnectedCallback, ConnectedServer);

    //        return ConnectedServer;
    //    }

    //    /// <summary>
    //    /// Complete the socket connection.
    //    /// </summary>
    //    /// <param name="ar"></param>
    //    public static void ConnectedCallback(IAsyncResult ar)
    //    {
    //        SocketState state = (SocketState)ar.AsyncState;

    //        try
    //        {
    //            state.Socket.EndConnect(ar);
    //        }
    //        catch (Exception e)
    //        {
    //            System.Diagnostics.Debug.WriteLine("Unable to connect to server. Error occurred: " + e);
    //            return;
    //        }

    //        state.Connected = true;
    //        state.CallbackMethod(state);
    //    }

    //    /// <summary>
    //    /// GetData is just a wrapper for BeginReceive.
    //    /// This is the public entry point for asking for data.
    //    /// Necessary so that we can separate networking concerns from client concerns.
    //    /// </summary>
    //    /// <param name="ss"></param>
    //    public static void GetData(SocketState ss)
    //    {

    //        try
    //        {
    //            ss.Socket.BeginReceive(ss.MessageBuffer, 0, ss.MessageBuffer.Length, SocketFlags.None, ReceiveCallback, ss);
    //        }
    //        catch (SocketException)
    //        {
    //            ss.Socket.Dispose();
    //            ss.Connected = false;
    //        }
    //    }

    //    /// <summary>
    //    /// This function is "called" by the operating system when data is available on the socket.
    //    /// </summary>
    //    /// <param name="ar">The socket state</param>
    //    public static void ReceiveCallback(IAsyncResult ar)
    //    {
    //        SocketState ss = (SocketState)ar.AsyncState;
    //        int bytesRead = 0;

    //        bytesRead = ss.Socket.EndReceive(ar);

    //        // EndReceive will return 0 bytes if the socket has been closed.
    //        if (bytesRead > 0)
    //        {
    //            string message = Encoding.UTF8.GetString(ss.MessageBuffer, 0, bytesRead);

    //            // Append the received data to the growable buffer.
    //            // It may be an incomplete message, so we need to start building it up piece by piece
    //            ss.ReceiveBuffer.Append(message);
    //            ss.CallbackMethod(ss);
    //        }
    //        else
    //        {
    //            ss.Socket.Dispose();
    //            ss.Connected = false;
    //        }
    //    }

    //    /// <summary>
    //    /// Send the given data out of the given socket.
    //    /// </summary>
    //    /// <param name="socket"></param>
    //    /// <param name="data"></param>
    //    public static void Send(SocketState ss, String data)
    //    {
    //        // Convert the data in bytes.
    //        byte[] messageBytes = Encoding.UTF8.GetBytes(data);

    //        try
    //        {
    //            ss.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, SendCallback, ss);
    //        }
    //        catch (SocketException)
    //        {
    //            ss.Socket.Dispose();
    //            ss.Connected = false;
    //        }
    //    }

    //    /// <summary>
    //    /// A callback invoked when a send operation completes.
    //    /// </summary>
    //    /// <param name="ar">The socket with which we just finished sending data out of.</param>
    //    public static void SendCallback(IAsyncResult ar)
    //    {
    //        SocketState ss = (SocketState)ar.AsyncState;

    //        try
    //        {
    //            ss.Socket.EndSend(ar);

    //        }
    //        catch (SocketException)
    //        {
    //            ss.Socket.Close();
    //            ss.Connected = false;
    //        }
    //    }
    //    public void FirstContact(SocketState state)
    //    {
    //        // Next we're expecting client ID.
    //        //state.CallbackMethod = ReceiveStartup;

    //        // Get the data for the world size.
    //        Networking.GetData(state);
    //    }
    }
}
