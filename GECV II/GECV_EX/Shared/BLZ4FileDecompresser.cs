using GECV_EX.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace GECV_EX.Shared
{
    
    public class BLZ4FileDecompresser : IDisposable
    {

        

        int magic = BLZ4Utils.BLZ4_HEADER; //4
        int unpacked_size; //8
        long zerozero; //16
        byte[] md5; //32
        bool is_uncompressd = false;

        List<byte[]> block_list = new List<byte[]>();
        private bool disposedValue;

        public BLZ4FileDecompresser(byte[] input)
        {


            if (input.Length <= 32 + 2)
            {
                throw new FileLoadException($"DecompressBLZ4:input.length:{input.Length}<=32+2");
            }

            using (MemoryStream input_ms = new MemoryStream(input))
            {

                using (BinaryReader input_br = new BinaryReader(input_ms))
                {

                    int magic = input_br.ReadInt32();

                    if (magic != this.magic)
                    {
                        throw new FileLoadException($"DecompressBLZ4:magic:{magic.ToString("X8")}!={BLZ4Utils.BLZ4_HEADER.ToString("X8")}");
                    }

                    this.unpacked_size = input_br.ReadInt32();
                    this.zerozero = input_br.ReadInt64();
                    this.md5 = input_br.ReadBytes(16);


                    while (input_br.BaseStream.Position < input.Length)
                    {
                        int chunk_size = input_br.ReadUInt16();
                        if (chunk_size == 0)
                        {

                            block_list.Add(input_br.ReadBytes(Convert.ToInt32(input.LongLength - input_br.BaseStream.Position)));
                            this.is_uncompressd = true;
                            
                        }
                        else
                        {

                            

                            block_list.Add(input_br.ReadBytes(chunk_size));
                            


                        }

                    }


                    if (block_list.Count <= 0)
                    {
                        throw new FileLoadException($"DecompressBLZ4:data_block.count:{block_list.Count}<=0");
                    }


                }


            }


        }



        public byte[] GetByteResult()
        {

            if(is_uncompressd)
            {
                return block_list[0];
            }
            else
            {
                LinkedList<byte[]> real_list = new LinkedList<byte[]>();


                for(int i = 1; i < block_list.Count; i++)
                {
                    real_list.AddLast(block_list[i]);
                }
                real_list.AddLast(block_list[0]);

                LinkedList<byte[]> real_block_list = new LinkedList<byte[]>();

                foreach(var i in real_list)
                {

                    byte[] out_file;
                    BLZ4Utils.DecompressData(i, out out_file);
                    real_block_list.AddLast(out_file);

                }

                LinkedList<byte> result_list = new LinkedList<byte>();

                foreach(var i in real_block_list)
                {
                    foreach(var si in i)
                    {
                        result_list.AddLast(si);
                    }
                }
                
                return result_list.ToArray<byte>();
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                  
                }


                disposedValue = true;
            }
        }

        public void Dispose()
        {

            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
