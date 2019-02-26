using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// Creates a new thread (task) for server and starts it.
    /// </summary>
    class Program
    {
        private static ServerView server;
        private static Task listenTask;

        static void Main(string[] args)
        {
            try
            {
                server = new ServerView();
                listenTask = new Task(server.Listen);
                listenTask.Start();
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}
