using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// Containses implementation of connection new clients, listens to them, broadcasts to them.
    /// </summary>
    class ServerView
    {
        private static TcpListener tcpListener;
        private List<ClientView> clients = new List<ClientView>();
        private string filePath = @"D:\epam-lab\workshopHM.Multithreading\WorkshopHM.Multithreading\Server\messages.txt";

        /// <summary>
        /// Adds a new client.
        /// </summary>
        /// <param name="clientView">A new client.</param>
        protected internal void AddConnection(ClientView clientView)
        {
            clients.Add(clientView);

            Console.WriteLine($"Number of connected clients: {clients.Count}");
        }

        /// <summary>
        /// Removes client connection and counts remaining clients.
        /// </summary>
        /// <param name="id">Cliend Id.</param>
        protected internal void RemoveConnection(string id)
        {
            ClientView client = clients.FirstOrDefault(c => c.Id == id);

            if (client != null)
            {
                clients.Remove(client);
                Console.WriteLine($"Number of connected clients: {clients.Count}");
            }
        }

        /// <summary>
        /// Waits tcp-client to work with him/her (bot).
        /// </summary>
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("The server is waiting for client response...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientView clientView = new ClientView(tcpClient, this);
                    Task task = new Task(clientView.Process);
                    task.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        /// <summary>
        /// Broadcasts last messages to new clients.
        /// </summary>
        /// <param name="message">Input message.</param>
        /// <param name="id">Client Id.</param>
        protected internal void BroadcastForNewUser(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);

            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id == id)
                {
                    clients[i].Stream.Write(data, 0, data.Length);
                }
            }
        }

        /// <summary>
        /// Broadcasts each message from the first client to the second client via the server.
        /// </summary>
        /// <param name="message">Input message.</param>
        /// <param name="id">Client Id.</param>
        protected internal void Broadcast(string message, string id)
        { 
            byte[] data = Encoding.Unicode.GetBytes(message);

            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id)
                {
                    clients[i].Stream.Write(data, 0, data.Length);
                }                
            }
        }

        /// <summary>
        /// Closes connection and removes file from the file path.
        /// </summary>
        protected internal void Disconnect()
        {
            FileInfo fileInf = new FileInfo(filePath);

            if (fileInf.Exists)
            {
                fileInf.Delete();
            }

            tcpListener.Stop(); 

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }

            Environment.Exit(0);
        }
    }
}
