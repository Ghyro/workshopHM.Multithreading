using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class ClientView
    {
        // Unique (for each a new user)
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        private string username;
        private TcpClient client;
        private ServerView server;
        // File path (you can change it)
        private string filePath = @"D:\epam-lab\workshopHM.Multithreading\WorkshopHM.Multithreading\Server\messages.txt";

        public ClientView(TcpClient tcpClient, ServerView serverView)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverView;
            serverView.AddConnection(this);
        }

        public void Process()
        {       
            try
            {
                Stream = client.GetStream();
                string message = GetMessage();

                username = message;
                message = username + " is online.";


                // Broadcast all last messages for new User
                Thread.Sleep(500);
                try
                {
                    using (StreamReader sr = new StreamReader(filePath, Encoding.Default))
                    {
                        string text;
                        while ((text = sr.ReadLine()) != null)
                        {
                            text = string.Concat(text, '\n');

                            server.BroadcastForNewUser(text, this.Id);
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Server: File is empty.");
                }
                    // Send to all users the user is online
                server.Broadcast(message, this.Id);                

                while (true)
                {
                    try
                    {               
                        message = GetMessage();                        
                        message = string.Format($"{username}: {message}");
                        Console.WriteLine(message);

                        // Write message to /file.txt/
                        using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.Default))
                        {
                            sw.WriteLine($"Last message: {message}");
                        }

                        server.Broadcast(message, this.Id);    
                    }
                    catch
                    {
                        message = string.Format($"{username} has left the chat.".ToUpper());
                        Console.WriteLine(message);
                        server.Broadcast(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder stringBuilder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));

            }
            while (Stream.DataAvailable);

            return stringBuilder.ToString();
        }

        protected internal void Close()
        {          
            if (Stream != null)
            {
                Stream.Close();
            }
                
            if (client != null)
            {
                client.Close();
            }                
        }
    }
}
