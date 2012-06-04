using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UTFUnifier
{
    class Program
    {
        public static Encoding hebrew = Encoding.GetEncoding(1255);
        public static Encoding UTF = Encoding.UTF8;
        static void Main(string[] args)
        {

            string[] files = Directory.GetFileSystemEntries(".", "*.log");

           
            int i=0;
            foreach (var file in files)
            {
                i++;
                Console.WriteLine("Unifying {0} - [{1}/{2}]",file,i.ToString(),files.Length.ToString());
                UnifyFile(file);
            }


            Console.ReadLine();
            

        }

        static string numeralsstring = "0123456789";
        public static void UnifyFile(string filename)
        {
            FileStream fs = File.OpenRead(filename);


            StringBuilder convertedFile = new StringBuilder();
            string line = null;
            while ((line = ReadLineFromStream(fs)) != null)
            {
                if (line.Length>0 && line[0] == '')
                {
                    line = line.Substring(3);
                }
                convertedFile.AppendLine(line);
            }

            File.WriteAllText("Unified\\" + filename, convertedFile.ToString());
        }



        static byte[] lineBytesBuffer = new byte[10240];

        public static string ReadLineFromStream(Stream stream)
        {
            byte b;
            int tempInt;
            int p = 0;
            while ((tempInt = stream.ReadByte()) != -1)
            {
                b = (byte)tempInt;
                if (b == 183 )
                {
                    b = 58;
                }
                lineBytesBuffer[p++] = b;

                
                if (b == 10)
                {
                    //lineBytes.RemoveRange(lineBytes.Count - 2, 2);
                    break;
                }
            }
            if (tempInt==-1 && p == 0)
                return null;

            bool isUTF = false;
            byte[] finalBytes = new byte[p-2];
            Array.Copy(lineBytesBuffer, finalBytes, p - 2);

            string result = decodeUTF8(finalBytes, out isUTF);
            if (isUTF)
                return result;//_Connection.getWriteEncoding().GetString(Encoding.Convert(Encoding.UTF8, _Connection.getWriteEncoding(), Encoding.UTF8.GetBytes(result)));
            else
                return hebrew.GetString(finalBytes); //_Connection.getWriteEncoding().GetString(Encoding.Convert(_Connection._ReadEncoding, _Connection.getWriteEncoding(), finalBytes));



        }

        private static String decodeUTF8(byte[] data, out bool isUTF)
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
                        if (data.Length < i + 2)
                        {
                            isUTF = false;
                            return string.Empty;
                        }
                        b = data[i + 1];
                        if ((b & 0xc0) == 0x80)
                        {
                            ret.Append((char)(((a & 0x1F) << 6) | (b & 0x3F)));
                            i++;
                        }
                        else
                        {
                            isUTF = false;
                            return string.Empty;
                            //throw new UTFDataFormatException("Illegal 2-byte group");
                        }
                    }
                    else if ((a & 0xf0) == 0xe0)
                    {
                        if (data.Length < i + 3)
                        {
                            isUTF = false;
                            return string.Empty;
                        }
                        b = data[i + 1];
                        c = data[i + 2];
                        if (((b & 0xc0) == 0x80) && ((c & 0xc0) == 0x80))
                        {
                            ret.Append((char)(((a & 0x0F) << 12) | ((b & 0x3F) << 6) | (c & 0x3F)));
                            i += 2;
                        }
                        else
                        {
                            isUTF = false;
                            return string.Empty;
                            //throw new UTFDataFormatException("Illegal 3-byte group");
                        }
                    }
                    else if (((a & 0xf0) == 0xf0) || ((a & 0xc0) == 0x80))
                    {
                        isUTF = false;
                        return string.Empty;
                        //throw new UTFDataFormatException("Illegal first byte of a group");
                    }
                }

                catch
                {
                    isUTF = false;
                    return string.Empty;
                }
            }
            isUTF = true;
            return ret.ToString();
        }

    }
}
