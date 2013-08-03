using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace TankGame.Communicator
{
    /*  Purpose of Messenger.cs
     * -------------------------
     * 
     *  Messenger.cs reads messages from the server and directs them to the client after converting into 
     *  an understandable form and processes resonses from the client and send them to the server.
     *  All network handling tasks are done by Messaenger.cs
     * 
     */

    /*  This class simulates the internals of a TCP client who is capable of connecting to a remote server
     *  and communicating with it in both directions.
     * 
     * 
     */
    class Messenger
    {
        private string CLIENT_IP; 
        private int CLIENT_PORT;

        private TcpClient client;
        private TcpListener my_server;

        public Messenger(string c_ip,int c_port) 
        {
            this.CLIENT_IP = c_ip;
            this.CLIENT_PORT = c_port;
            

            client = new TcpClient();
            this.connect_client();
            this.talk_to_server();
        }

        #region client_activity

        public void connect_client()
        {
            try
            {
                client.Connect(IPAddress.Parse(CLIENT_IP), CLIENT_PORT);
                //Console.WriteLine("Client connected to server on port " + CLIENT_PORT);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void talk_to_server()
        {
            int i = 1;
            NetworkStream client_stream = client.GetStream();
            //Console.WriteLine("Client>>Type your message to the server\n");
            
            while (i != 0)
            {
                #region write_to_server

                    // code should go here

                #endregion

                #region read_from_server

                    // read from the server
                    byte[] reply_from_server = new byte[1024]; // read 1 kB at a time, none of the messages exceed 1 kByte
                    i = client_stream.Read(reply_from_server, 0, reply_from_server.Length);

                    String server_reply = System.Text.Encoding.ASCII.GetString(reply_from_server);
                    //Console.WriteLine("Client>> Server responded with the following message\n" + server_reply);

                #endregion
            }
        }

        #endregion
    }
}
