using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jiyuu.Common;
namespace IRCLogReader
{
    public static class Files
    {
        static Files()
        {

        }

        public static int Getfile(FileStream fs)
        {
            string FileName = Path.GetFileName(fs.Name);
            //Utils u = new Utils();

            string hash = System.Security.Cryptography.SHA1.Create().ComputeHash(fs).ToHex();
            return 0;
        }
    }
}
