using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meebey.SmartIrc4net;
using System.Collections;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
namespace SimpleBot
{
    class Program
    {



        public static IrcClient irc = new IrcClient();

        // this method we will use to analyse queries (also known as private messages)
        public static void OnQueryMessage(object sender, IrcEventArgs e)
        {
            switch (e.Data.MessageArray[0].ToLower())
            {
                // debug stuff
                case "dump_channel":
                    string requested_channel = e.Data.MessageArray[1];
                    // getting the channel (via channel sync feature)
                    Channel channel = irc.GetChannel(requested_channel);

                    // here we send messages
                    irc.SendMessage(SendType.Message, e.Data.Nick, "<channel '" + requested_channel + "'>");

                    irc.SendMessage(SendType.Message, e.Data.Nick, "Name: '" + channel.Name + "'");
                    irc.SendMessage(SendType.Message, e.Data.Nick, "Topic: '" + channel.Topic + "'");
                    irc.SendMessage(SendType.Message, e.Data.Nick, "Mode: '" + channel.Mode + "'");
                    irc.SendMessage(SendType.Message, e.Data.Nick, "Key: '" + channel.Key + "'");
                    irc.SendMessage(SendType.Message, e.Data.Nick, "UserLimit: '" + channel.UserLimit + "'");

                    // here we go through all users of the channel and show their
                    // hashtable key and nickname 
                    string nickname_list = "";
                    nickname_list += "Users: ";
                    foreach (DictionaryEntry de in channel.Users)
                    {
                        string key = (string)de.Key;
                        ChannelUser channeluser = (ChannelUser)de.Value;
                        nickname_list += "(";
                        if (channeluser.IsOp)
                        {
                            nickname_list += "@";
                        }
                        if (channeluser.IsVoice)
                        {
                            nickname_list += "+";
                        }
                        nickname_list += ")" + key + " => " + channeluser.Nick + ", ";
                    }
                    irc.SendMessage(SendType.Message, e.Data.Nick, nickname_list);

                    irc.SendMessage(SendType.Message, e.Data.Nick, "</channel>");
                    break;
                case "gc":
                    GC.Collect();
                    break;
                // typical commands
                case "join":
                    irc.RfcJoin(e.Data.MessageArray[1]);
                    break;
                case "part":
                    irc.RfcPart(e.Data.MessageArray[1]);
                    break;
                case "die":
                    Exit();
                    break;
                case "!log":
                    ThreadPool.QueueUserWorkItem(new WaitCallback(handleLogRequest_FromQueue), new object[] { e.Data, false });
                    break;
                case "!plog":
                    ThreadPool.QueueUserWorkItem(new WaitCallback(handleLogRequest_FromQueue), new object[] { e.Data, true });

                    break;
            }
        }

        private static string postToPastebin(string logToPost)
        {
            //HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.Create("http://pastebin.com/api/api_post.php");
            //wr.getre

            WebClient wc = new WebClient();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("api_option={0}&", "paste");
            sb.AppendFormat("api_dev_key={0}&", "4502b1658a6014fbc35835b7256ed024");
            sb.AppendFormat("api_paste_code={0}&", HttpUtility.UrlEncode(logToPost));
            sb.AppendFormat("api_paste_private={0}&", "0");
            sb.AppendFormat("api_paste_name={0}&", HttpUtility.UrlEncode(DateTime.Now.ToString()));
            sb.AppendFormat("api_paste_expire_date={0}&", "10M");
            sb.AppendFormat("api_paste_format={0}", "mirc");
            wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");


            return Encoding.ASCII.GetString(wc.UploadData("http://pastebin.com/api/api_post.php", "POST", Encoding.UTF8.GetBytes(sb.ToString())));

        }

        private static void handleLogRequest_FromQueue(object state)
        {
            object[] fields = (object[])state;
            handleLogRequest((IrcMessageData)fields[0], (bool)fields[1]);
        }

        private static volatile Dictionary<string, DateTime> logRequest = new Dictionary<string, DateTime>();

        static volatile int count = 8;
        private static void handleLogRequest(IrcMessageData Data, bool sendLink)
        {

            if (!sendLink){
                if (logRequest.ContainsKey(Data.Nick) && logRequest[Data.Nick].AddMinutes(1) >= DateTime.Now)
                {
                    irc.SendMessage(SendType.Message, "#dumdum", string.Format("{0} is the spamming garbovan", Data.Nick));
                    return;
                }
                logRequest[Data.Nick] = DateTime.Now;
            }
            int messagesToGet = 15;

            try
            {
                if (Data.MessageArray.Length > 1)
                    messagesToGet = int.Parse(Data.MessageArray[1]);
            }
            catch { }

            if (messagesToGet > MAXLOGMESSAGES) messagesToGet = MAXLOGMESSAGES;

            string[] messages = new string[0];
            lock (messageQueue)
            {
                messages = messageQueue.ToArray();
            }
            messages = messages.Reverse().Take(messagesToGet).Reverse().ToArray();

            if (!sendLink)
            {
                foreach (string msg in messages)
                {
                    count--;
                    if (count <= 0)
                    {
                        Thread.Sleep(4500);
                        count = 10;
                    }
                    irc.SendMessage(SendType.Message, Data.Nick, msg);
                    Thread.Sleep(500);
                }
            }
            else
            {
                irc.SendMessage(SendType.Message, Data.Nick, postToPastebin(String.Join("\r\n", messages)));
            }

        }

        // this method handles when we receive "ERROR" from the IRC server
        public static void OnError(object sender, ErrorEventArgs e)
        {
            System.Console.WriteLine("Error: " + e.ErrorMessage);
            Exit();
        }

        // this method will get all IRC messages
        public static void OnRawMessage(object sender, IrcEventArgs e)
        {
            System.Console.WriteLine("Received: " + e.Data.RawMessage);
        }

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.GetEncoding(1255) ;
            //string a = "\r\n";
            //foreach (byte b in System.Text.Encoding.ASCII.GetBytes(a))
            //{
            //    Console.Write(b.ToString() + "|");
            //}
            //Console.Write(a);
            //foreach (byte b in System.Text.Encoding.UTF8.GetBytes(a))
            //{
            //    Console.Write(b.ToString() + "|");
            //}


            Thread.CurrentThread.Name = "Main";

            // UTF-8 test
            irc.ReadEncoding = System.Text.Encoding.GetEncoding(1255);// System.Text.Encoding.GetEncoding(1255);
            irc.WriteEncoding = System.Text.Encoding.UTF8;// System.Text.Encoding.GetEncoding(1255);
            irc.DetectUTF = true;
            // wait time between messages, we can set this lower on own irc servers
            irc.SendDelay = 200;
            // we use channel sync, means we can use irc.GetChannel() and so on
            irc.ActiveChannelSyncing = true;

            // here we connect the events of the API to our written methods
            // most have own event handler types, because they ship different data
            irc.OnQueryMessage += new IrcEventHandler(OnQueryMessage);
            irc.OnChannelMessage += new IrcEventHandler(irc_OnChannelMessage);
            irc.OnError += new ErrorEventHandler(OnError);
            irc.OnRawMessage += new IrcEventHandler(OnRawMessage);
            irc.OnQuit += new QuitEventHandler(irc_OnQuit);
            string[] serverlist;
            // the server we want to connect to, could be also a simple string
            serverlist = new string[] { "irc.rizon.net" };
            int port = 6667;
            string channel = "#dumdum";
            while (true)
            {
                try
                {
                    // here we try to connect to the server and exceptions get handled
                    irc.Connect(serverlist, port);
                }
                catch (ConnectionException e)
                {
                    // something went wrong, the reason will be shown
                    System.Console.WriteLine("couldn't connect! Reason: " + e.Message);
                    //Exit();
                }

                try
                {
                    // here we logon and register our nickname and so on 
                    irc.Login("SmartyBot", "SmartIrc4net Test Bot");
                    // join the channel
                    irc.RfcJoin(channel);

                    //irc.SendMessage(SendType.Message, channel, "בלה בלה בלה יוניקוד");
                    //for (int i = 0; i < 3; i++)
                    //{
                    //    // here we send just 3 different types of messages, 3 times for
                    //    // testing the delay and flood protection (messagebuffer work)
                    //    irc.SendMessage(SendType.Message, channel, "test message (" + i.ToString() + ")");
                    //    irc.SendMessage(SendType.Action, channel, "thinks this is cool (" + i.ToString() + ")");
                    //    irc.SendMessage(SendType.Notice, channel, "SmartIrc4net rocks (" + i.ToString() + ")");
                    //}

                    // spawn a new thread to read the stdin of the console, this we use
                    // for reading IRC commands from the keyboard while the IRC connection
                    // stays in its own thread
                    new Thread(new ThreadStart(ReadCommands)).Start();

                    // here we tell the IRC API to go into a receive mode, all events
                    // will be triggered by _this_ thread (main thread in this case)
                    // Listen() blocks by default, you can also use ListenOnce() if you
                    // need that does one IRC operation and then returns, so you need then 
                    // an own loop 
                    irc.Listen();

                    // when Listen() returns our IRC session is over, to be sure we call
                    // disconnect manually
                    irc.Disconnect();
                }
                catch (ConnectionException)
                {
                    // this exception is handled because Disconnect() can throw a not
                    // connected exception
                    //Exit();
                }
                catch (Exception e)
                {
                    // this should not happen by just in case we handle it nicely
                    System.Console.WriteLine("Error occurred! Message: " + e.Message);
                    System.Console.WriteLine("Exception: " + e.StackTrace);
                    //Exit();
                }
                Thread.Sleep(10000);
            }
        }

        static void irc_OnQuit(object sender, QuitEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static System.Text.Encoding getEncoding(string rawName)
        {
            switch (rawName.ToLower())
            {
                case "utf8":
                    return System.Text.Encoding.UTF8;
                default:
                    return System.Text.Encoding.GetEncoding(rawName);
            }

        }

        private static System.Text.Encoding getURLEncoding(string url)
        {
            HttpWebResponse rs = (HttpWebResponse)HttpWebRequest.Create(url).GetResponse();
            Encoding encode = getEncoding(rs.CharacterSet);
            // Pipes the stream to a higher level stream reader with the required encoding format. 
            System.IO.Stream receiveStream = rs.GetResponseStream();
            //System.IO.StreamReader readStream = new System.IO.StreamReader(receiveStream,encode);
            byte[] read = new byte[256];
            int count = receiveStream.Read(read, 0, 256);
            StringBuilder sb = new StringBuilder();
            Match m = null;
            while (count > 0 && !sb.ToString().Contains("</title>"))
            {
                // Dumps the 256 characters on a string and displays the string to the console.
                String str = encode.GetString(read);
                count = receiveStream.Read(read, 0, 256);
                sb.Append(str);
                if (m == null)
                {
                    m = Regex.Match(sb.ToString(), "charset *= *(?<Charset>[^\";]+?)[;\"]", RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        return getEncoding(m.Groups["Charset"].Value);
                        //readStream = new System.IO.StreamReader(receiveStream, encode);
                    }
                    else
                        m = null;
                }

            }

            return Encoding.UTF8;
        }

        private static string getURLTitle(string url)
        {
            HttpWebRequest rq = (HttpWebRequest)HttpWebRequest.Create(url);
            rq.Timeout = 5000;
            HttpWebResponse rs = (HttpWebResponse)rq.GetResponse();
            Encoding baseEncode = Encoding.UTF8; //getEncoding(rs.CharacterSet);
            Encoding encode = baseEncode; //getEncoding(rs.CharacterSet);
            // Pipes the stream to a higher level stream reader with the required encoding format. 
            System.IO.Stream receiveStream = rs.GetResponseStream();
            //System.IO.StreamReader readStream = new System.IO.StreamReader(receiveStream,encode);
            byte[] read = new byte[256];
            int count = receiveStream.Read(read, 0, 256);
            StringBuilder sb = new StringBuilder();
            Match m = null;
            List<byte> bytes = new List<byte>();
            do
            {
                // Dumps the 256 characters on a string and displays the string to the console.
                bytes.AddRange(read.Take(count));
                sb.Append(baseEncode.GetString(read, 0, count));

                
                if (m == null)
                {
                    m = Regex.Match(sb.ToString(), "charset *= *(?<Charset>[^\";]+?)[;\"]", RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        encode = getEncoding(m.Groups["Charset"].Value);
                        //receiveStream.Close();
                        //rs.Close();
                        //rs = (HttpWebResponse)HttpWebRequest.Create(url).GetResponse();
                        //sb = new StringBuilder();
                        //receiveStream = rs.GetResponseStream();
                        //read = new byte[256];
                        //count = receiveStream.Read(read, 0, 256);
                        //readStream = new System.IO.StreamReader(receiveStream, encode);
                    }
                    else
                        m = null;
                }

                count = receiveStream.Read(read, 0, 256);

            }
            while (count > 0 && !sb.ToString().Contains("</title>"));
            //string source = ;

            return HttpUtility.HtmlDecode(Regex.Match(encode.GetString(bytes.ToArray()), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value.Replace("\n", "").Replace("\r", ""));

        }

        static List<string> messageQueue = new List<string>();

        public class UTFDataFormatException : Exception
        {
            public UTFDataFormatException(string msg)
                : base(msg)
            {

            }
        }

        private String decodeUTF8(byte[] data, bool gracious)
        {
            byte a, b, c;
            StringBuilder ret = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                try
                {
                    a = data[i];
                    if ((a & 0x80) == 0)
                        ret.Append((char)a);
                    else if ((a & 0xe0) == 0xc0)
                    {
                        b = data[i + 1];
                        if ((b & 0xc0) == 0x80)
                        {
                            ret.Append((char)(((a & 0x1F) << 6) | (b & 0x3F)));
                            i++;
                        }
                        else
                        {
                            throw new UTFDataFormatException("Illegal 2-byte group");
                        }
                    }
                    else if ((a & 0xf0) == 0xe0)
                    {
                        b = data[i + 1];
                        c = data[i + 2];
                        if (((b & 0xc0) == 0x80) && ((c & 0xc0) == 0x80))
                        {
                            ret.Append((char)(((a & 0x0F) << 12) | ((b & 0x3F) << 6) | (c & 0x3F)));
                            i += 2;
                        }
                        else
                        {
                            throw new UTFDataFormatException("Illegal 3-byte group");
                        }
                    }
                    else if (((a & 0xf0) == 0xf0) || ((a & 0xc0) == 0x80))
                    {
                        throw new UTFDataFormatException("Illegal first byte of a group");
                    }
                }
                catch (UTFDataFormatException udfe)
                {
                    if (gracious)
                        ret.Append("?");
                    else
                        throw udfe;
                }
                catch (Exception aioobe)
                {
                    if (gracious)
                        ret.Append("?");
                    else
                        throw new UTFDataFormatException("Unexpected EOF");
                }
            }

            return ret.ToString();
        }

        private static void handleChannelMessage(object state)
        {
            IrcEventArgs e = (IrcEventArgs)state;
            if (e.Data.Message.StartsWith("http://") || e.Data.Message.StartsWith("www"))
            {
                try
                {
                    irc.SendMessage(SendType.Message, e.Data.Channel, getURLTitle(e.Data.MessageArray[0]));
                }
                catch { }

            }
            else if (e.Data.Message == "!log")
            {
                handleLogRequest(e.Data, false);
            }
        }

        static void irc_OnChannelMessage(object sender, IrcEventArgs e)
        {
            logMessage(e);
            ThreadPool.QueueUserWorkItem(new WaitCallback(handleChannelMessage), e);
        }
        private static int MAXLOGMESSAGES = 100;
        private static void logMessage(IrcEventArgs e)
        {
            lock (messageQueue)
            {
                if (messageQueue.Count > MAXLOGMESSAGES)
                    messageQueue.RemoveAt(0);

                messageQueue.Add(string.Format("[{0}]<{1}> {2}", DateTime.Now.ToString("hh:mm:ss"), e.Data.Nick, e.Data.Message));
            }
        }

        public static void ReadCommands()
        {
            // here we read the commands from the stdin and send it to the IRC API
            // WARNING, it uses WriteLine() means you need to enter RFC commands
            // like "JOIN #test" and then "PRIVMSG #test :hello to you"
            while (true)
            {
                string cmd = System.Console.ReadLine();
                if (cmd.StartsWith("/list"))
                {
                    int pos = cmd.IndexOf(" ");
                    string channel = null;
                    if (pos != -1)
                    {
                        channel = cmd.Substring(pos + 1);
                    }

                    IList<ChannelInfo> channelInfos = irc.GetChannelList(channel);
                    Console.WriteLine("channel count: {0}", channelInfos.Count);
                    foreach (ChannelInfo channelInfo in channelInfos)
                    {
                        Console.WriteLine("channel: {0} user count: {1} topic: {2}",
                                          channelInfo.Channel,
                                          channelInfo.UserCount,
                                          channelInfo.Topic);
                    }
                }
                else
                {
                    irc.WriteLine(cmd);
                }
            }
        }

        public static void Exit()
        {
            // we are done, lets exit...
            System.Console.WriteLine("Exiting...");
            System.Environment.Exit(0);
        }

    }
}
