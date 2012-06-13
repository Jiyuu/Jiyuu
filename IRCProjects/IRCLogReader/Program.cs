using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRCLogReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files = Directory.GetFileSystemEntries(".", "*.log");


            int i = 0;
            foreach (var file in files)
            {
                i++;
                Console.WriteLine("Unifying {0} - [{1}/{2}]", file, i.ToString(), files.Length.ToString());
                ReadLogToDB(file);
            }

        }

        string[] timeStampFormats = new string[] { ".:HH:mm:ss:.", "[HH:mm]", "[HH:mm:ss]" };
        string[] sessionStartFormats = new string[] { "Session Start: ddd MMM dd HH:mm:ss yyyy" };
        public static void ReadLogToDB(string file)
        {
            string[] lines = File.ReadAllLines(file);
            //IRCLogReader.Caches.Files.Getfile(File.Open(file, FileMode.Open));
            //Caches.Files.Getfile();
            int fileID = Files.Getfile(File.Open(file, FileMode.Open));
            string line;
            for (int i = 0; i < lines.Length; i++)
            {
                line = lines[i];

            }
        }

        public class IrcLine
        {
            public DateTime timestamp{get;set;}
            public string RawText {get;set; }
            public int LineSrc { get; set; }
            public int LogFile { get; set; }
        }
        public class IrcTextLine:IrcLine
        {
            public string Msg { get; set; }
            public int NickID { get; set; }
            public string Nick { get; set; }
        }

        public class IrcAction:IrcLine
        {
            public IrcActionTypeEnum ActionType { get; set; }
        }

        public enum IrcActionTypeEnum
        { 
            ACTION,
            JOIN,
            QUIT,
            TOPIC_IS,
            TOPIC_CHANGE,
            CHANNEL,
            KICK,
            MODE,
            PART,
            NICK,

        }

    }

    
}
