using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;


namespace TankGame.Network
{
    /*  Purpose of Communicator.cs
     * -------------------------
     * 
     *  Communicator.cs reads messages from the server and directs them to the client after converting into 
     *  an understandable form and processes resonses from the client and send them to the server.
     *  All network handling tasks are done by Messaenger.cs
     * 
     */

    /*  This class simulates the internals of a TCP client who is capable of connecting to a remote server
     *  and communicating with it in both directions.
     * 
     * 
     */
    class Communicator
    {
        private string SERVER_IP; 
        private int CLIENT_PORT;
                
        private TcpClient client;
        private TcpListener server;
        private Socket conn;
        private NetworkStream client_stream; // from server to client
        private NetworkStream server_stream; // from client to server

        public Communicator() 
        {
            this.SERVER_IP = ConfigData.SERVER_IP;
            this.CLIENT_PORT = ConfigData.CLIENT_PORT;
           
        }


        #region client_activity

        public string read_from_server()
        {
            /*  This method reads replies from the server. 
             *  Messages can be  variable length. So we need a way to accept these variable length messages
             *  we cannot use byte[] for this purpose because we needa to specify the length of the array at the declaration.
             *  Therefore List<> class is used
             */
            try
            {
                server = new TcpListener(IPAddress.Parse(SERVER_IP),ConfigData.CLIENT_PORT);
                server.Start();
                conn= server.AcceptSocket();                  
            }
            catch (Exception e) {
                Console.WriteLine("CAN'T CONNECT TO SERVER"); 
            }


            int i = 0; // initial value for the number of bytes read from  the server
            server_stream = new NetworkStream(conn); // initialize a network stream
            
            List<byte> reply_from_server=new List<byte>(); // an empty list to hold data tram
            
            while (i != -1) // -1 indicated the end of the stream
            { 
                // read from the server
                try
                {
                    i = server_stream.ReadByte();
                    reply_from_server.Add((byte)i);
                }
                catch(System.IO.IOException e){
                    Console.WriteLine("ERROR OCCURED WHILE RECIEVING SERVER RESPONSE");                
                }
            }
            
            // convert the byte list into a string
            String server_reply = System.Text.Encoding.ASCII.GetString(reply_from_server.ToArray());
            
            server_stream.Close();
            server.Stop();
            return server_reply;
        }

        public void write_to_server(String msg) {

            try
            {
                /*
                 * Connect to the server via given IP and CLIENT_PORT and  
                 */
                client = new TcpClient();
                client.Connect(IPAddress.Parse(SERVER_IP),ConfigData.SERVER_PORT);
                client_stream = client.GetStream();
                
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR OCCURED WHILE WRITING"); 
            }
            if (client.Connected)
            {
                byte[] msg_in_bytes = System.Text.Encoding.ASCII.GetBytes(msg);
                client_stream.Write(msg_in_bytes, 0, msg_in_bytes.Length);
                client.Close();
                client_stream.Close();
            }
            else {
                Console.WriteLine("SERVER UNREACHABLE");
            } 
        }
        
        #endregion


       
    }

    
}
