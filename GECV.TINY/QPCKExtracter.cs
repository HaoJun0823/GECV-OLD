using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GECV.TINY
{
    public class QPCKExtracter : IDisposable
    {



        private Stream stream;

        private QPCK qpck;


        public int count => qpck.count;


        public byte[] this[int index]
        {
            get {  return GetData(index); }
        }



        private QPCKExtracter() { }

        public QPCKExtracter(Stream stream)
        {
            this.stream = stream;
            qpck = new QPCK();
            Load();

        }


        private void Load()
        {
            BinaryReader br = new BinaryReader(stream);
            {

                br.BaseStream.Seek(0, SeekOrigin.Begin);

                qpck.magic = br.ReadInt32();

                if (qpck.magic != QPCK.DEFAULT_MAGIC)
                {


                    throw new FileLoadException($"Magic Error:{QPCK.DEFAULT_MAGIC.ToString("X8")} not equals {qpck.magic.ToString("X8")}!");
                }

                int stream_count = br.ReadInt32();


                for (int i = 0; i < stream_count; i++)
                {

                    QPCK.DataIndex di = new QPCK.DataIndex();

                    di.offset = br.ReadInt64();
                    di.hash = br.ReadInt64();
                    di.size = br.ReadInt32();

                    qpck.data.Add(di);

                }

                //Console.WriteLine("Stream:"+stream.CanRead);


            }
        }

        public void Dispose()
        {


            this.stream.Dispose();
            GC.SuppressFinalize(this);
        }

        public void ExtractAll(String directorayPath)
        {
            for (int i = 0; i < qpck.count; i++) { 
            
                QPCK.DataIndex di = qpck.data[i];

                String filename = $"{directorayPath}{i.ToString("D8")}_{di.hash.ToString("X16")}";
                

            }
        }


        public byte[] GetData(int index)
        {
            QPCK.DataIndex di = qpck.data[index];

            stream.Seek(di.offset, SeekOrigin.Begin);

            BinaryReader br = new BinaryReader(stream);
            {
                return br.ReadBytes(di.size);
                
            }

            



        }

        public void Extract(String dictionaryPath,int index)
        {

            byte[] filedata = GetData(index);
            QPCK.DataIndex di = qpck.data[index];

            String filename = $"{dictionaryPath}{index.ToString("D8")}_{di.hash.ToString("X16")}";

            if (!Directory.Exists(dictionaryPath)) { 
            
                Directory.CreateDirectory(dictionaryPath);
            
            }

            if (File.Exists(filename)) { 
            
                File.Delete(filename);
            
            }

            File.WriteAllBytes(filename, filedata);

        }


    }
}
