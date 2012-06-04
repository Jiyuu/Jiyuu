using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace MPC_SYNC
{
    class Program
    {
        public static List<TcpClient> clients = new List<TcpClient>();

        static void Main(string[] args)
        {
            new Thread(proccessMsgs).Start();
            // Create an instance of the TcpListener class.
            TcpListener tcpListener = null;
            IPAddress ipAddress = Dns.GetHostEntry("127.0.0.1").AddressList[0];
            try
            {
                // Set the listener on the local IP address
                // and specify the port.
                tcpListener = new TcpListener(ipAddress, 1313);
                tcpListener.Start();
                Console.WriteLine("Waiting for a connection...");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            while (true)
            {
                // Always use a Sleep call in a while(true) loop
                // to avoid locking up your CPU.
                Thread.Sleep(10);
                // Create a TCP socket.
                // If you ran this server on the desktop, you could use
                // Socket socket = tcpListener.AcceptSocket()
                // for greater flexibility.
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Console.WriteLine("got new connection from: " + tcpClient.Client.RemoteEndPoint.ToString());

                lock (clients)
                {
                    clients.Add(tcpClient);
                }
            }

        }


        public static void proccessMsgs()
        {
            while (true)
            {
                lock (clients)
                {
                    byte lastMsg = 0;
                    int tmp = 0;
                    NetworkStream ns;
                    List<TcpClient> removalClients = new List<TcpClient>();
                    foreach (var client in clients)
                    {
                        if (!client.Connected)
                        {
                            removalClients.Add(client);
                            continue;
                        }

                        if (client.Available > 0)
                        {
                            ns = client.GetStream();
                            while (client.Available > 0 && ((tmp = ns.ReadByte()) != -1))
                            {
                                if (tmp != -1)
                                    lastMsg = (byte)tmp;
                            }
                        }
                    }
                    ns = null;
                    if (lastMsg != 0)
                    {
                        foreach (var client in clients)
                        {
                            try
                            {
                                ns = client.GetStream();
                                ns.WriteByte(lastMsg);
                                ns.Flush();
                            }
                            catch { }
                        }
                    }
                }

                Thread.Sleep(10);
            }
        }
    }
}
