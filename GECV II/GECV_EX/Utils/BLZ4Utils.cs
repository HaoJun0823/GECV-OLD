using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using zlib;

namespace GECV_EX.Utils
{
    public static class BLZ4Utils
    {
        public static readonly int BLZ4_HEADER = 0x347a6c62;


        public static bool IsBLZ4(byte[] data)
        {

            if (data.Length < 4)
            {
                return false;
            }

            using (MemoryStream input_ms = new MemoryStream(data))
            {

                using (BinaryReader input_br = new BinaryReader(input_ms))
                {
                    int magic = input_br.ReadInt32();

                    if (magic != BLZ4_HEADER)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }


        // partially from http://stackoverflow.com/a/6627194/5343630
        public static void CompressData(byte[] inData, out byte[] outData)
        {
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream, zlibConst.Z_BEST_COMPRESSION))
            using (Stream inMemoryStream = new MemoryStream(inData))
            {
                CopyStream(inMemoryStream, outZStream);
                outZStream.finish();
                outData = outMemoryStream.ToArray();
                
            }
        }

        public static void DecompressData(byte[] inData, out byte[] outData)
        {
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream))
            using (Stream inMemoryStream = new MemoryStream(inData))
            {
                CopyStream(inMemoryStream, outZStream);
                outZStream.finish();
                outData = outMemoryStream.ToArray();
            }
        }


        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[1000000];
            int len;
            while ((len = input.Read(buffer, 0, 1000000)) > 0)
            {
                
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }


        public static List<byte[]> SplitBytes(byte[] input,int size)
        {

            
            List<byte[]> list = new List<byte[]>();

            List<byte> slist = new List<byte>();

            int first_size = input.Length % size; 

            


            for(int i = 0; i < first_size; i++)
            {
                slist.Add(input[i] );

            }

            list.Add(slist.ToArray());
            
            slist.Clear();


            for (int i = first_size; i < input.Length; i++)
            {

                slist.Add(input[i]);

                if(slist.Count == size)
                {
                    list.Add(slist.ToArray());
                    
                    slist.Clear();
                }

            }





            return list;
        }



    }
}
