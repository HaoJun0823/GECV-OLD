﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using zlib;

namespace GECV
{
    public static class BLZ4Utils
    {
        // partially from http://stackoverflow.com/a/6627194/5343630
        public static void CompressData(byte[] inData, out byte[] outData)
        {
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream, zlibConst.Z_DEFAULT_COMPRESSION))
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


        //https://blog.csdn.net/qq_41973169/article/details/125589780
        public static byte[] ZLibDecompress(byte[] data,int size)
        {
            MemoryStream compressed = new MemoryStream(data);
            ZInputStream zIn = new ZInputStream(compressed);
            byte[] result = new byte[size];
            zIn.read(result,0, result.Length);
            return result;
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[1000000];
            int len;
            while ((len = input.Read(buffer, 0, 1000000)) > 0)
            {
                Log.Info($"复制流：目前读取长度{len}");
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }
    }
}
