using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jiyuu.Common;
namespace DuplicateFilesFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files = Directory.GetFileSystemEntries(".", "*.log");

            Dictionary<string, string> cachedFiles = new Dictionary<string, string>();

            List<string> Duplicates = new List<string>();

            int i = 0;
            foreach (var file in files)
            {
                i++;
                Console.WriteLine("Finding Duplicates {0} - [{1}/{2}]", file, i.ToString(), files.Length.ToString());
                FileStream fs = File.Open(file, FileMode.Open);
                string fileHash = System.Security.Cryptography.SHA1.Create().ComputeHash(fs).ToHex();
                fs.Close();
                string fileName = Path.GetFileName(file);

                if (cachedFiles.ContainsKey(fileHash))
                    Duplicates.Add(file);
                else
                    cachedFiles.Add(fileHash, fileName);
            }
            bool delete = false;
            if (args.Length > 0 && args[0] == "d")
                delete = true;
            Console.WriteLine("==========================================================================");
            Console.WriteLine("==========================================================================");
            Console.WriteLine("==========================================================================");
            Console.WriteLine("==========================================================================");
            Console.WriteLine("==========================================================================");
            foreach (var item in Duplicates)
            {
                if (delete)
                {
                    File.Delete(item);
                    Console.WriteLine("Duplicate file Deleted:{0}", item);
                }
                else
                    Console.WriteLine("Duplicate file:{0}", item);
            }

            Console.WriteLine("==========================================================================");
            Console.WriteLine("==========================================================================");
            Console.WriteLine("==========================================================================");
            Console.WriteLine("==========================================================================");
            Console.WriteLine("==========================================================================");
            Console.WriteLine("total Duplicates: {0}",Duplicates.Count);
        }
    }
}
