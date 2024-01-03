using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GECV
{
    public static class Utils
    {




        public static string readNullterminated(BinaryReader reader)
        {
            var char_array = new List<byte>();
            string str = "";
            if (reader.BaseStream.Position == reader.BaseStream.Length)
            {
                byte[] char_bytes2 = char_array.ToArray();
                str = Encoding.UTF8.GetString(char_bytes2);
                return str;
            }
            byte b = reader.ReadByte();
            while ((b != 0x00) && (reader.BaseStream.Position != reader.BaseStream.Length))
            {
                char_array.Add(b);
                b = reader.ReadByte();
            }
            byte[] char_bytes = char_array.ToArray();
            str = Encoding.UTF8.GetString(char_bytes);
            return str;
        }

        public static string PrintByteArray(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("X2"));
                sb2.Append(bytes[i].ToString("X2"));
                if (i < bytes.Length - 1)
                {
                    sb.Append(" ");
                }
            }
            Log.Info(sb.ToString());
            return sb2.ToString();

        }


    }
}
