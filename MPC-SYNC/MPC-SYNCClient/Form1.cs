using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;

namespace MPC_SYNCClient
{
    public partial class Form1 : Form
    {
        static TcpClient client;

        public Form1()
        {
            InitializeComponent();
            client = new TcpClient("127.0.0.1", 1313);

            new Thread(processMsgs).Start();
            


        }
        public static void processMsgs()
        {
            while (true)
            {
                NetworkStream ns;
                int tmp=0;
                byte lastMsg = 0;
                if (client.Available > 0)
                { 
                    ns=client.GetStream();
                    while (client.Available > 0 && (tmp = ns.ReadByte())!=-1)
                    {
                        if (tmp != -1)
                            lastMsg = (byte)tmp;
                    }
                    MessageBox.Show("msg" + lastMsg.ToString());
                }
                Thread.Sleep(10);
            }
        }
    }
}
