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
    class ServerView
    {
        private static TcpListener tcpListener;
        private List<ClientView> clients = new List<ClientView>();

        protected internal void AddConnection(ClientView clientView)
        {
            clients.Add(clientView);

            Console.WriteLine($"Number of connected clients: {clients.Count}");
        }

        protected internal void RemoveConnection(string id)
        {
            ClientView client = clients.FirstOrDefault(c => c.Id == id);

            if (client != null)
            {
                clients.Remove(client);
                Console.WriteLine($"Number of connected clients: {clients.Count}");
            }
        }

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

        protected internal void Disconnect()
        {
            tcpListener.Stop(); 

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }            

            Environment.Exit(0);
        }
    }
}
