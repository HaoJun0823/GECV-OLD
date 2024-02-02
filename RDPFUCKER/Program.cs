using GECV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDPFUCKER
{




    internal class Program
    {


        static LinkedList<byte[]> block_list = new LinkedList<byte[]>();

        static void Main(string[] args)
        {

            if(args.Length != 2) {


                throw new ArgumentException($"需要两个参数：第一个，rdp文件，第二个，解压的文件夹。");
            }
           

            FileInfo file = new FileInfo(args[0]);
            DirectoryInfo dir = new DirectoryInfo(args[1]);

            if(!dir.Exists )
            {
                dir.Create();
            }

            if(file.Length < 800)
            {
                throw new Exception($"你这是假文件！你这文件大小还没到800呢：{file.Length}");
            }
            else
            {
                Log.Info($"文件长度：10:{file.Length},16:{file.Length.ToString("X8")}");
            }

            if(file.Length% 16 !=0 )
            {
                throw new Exception($"RDP长度可能不对！：{file.Length}无法被16整除！要不你自己补几个0？");
            }

            using(FileStream fs = file.OpenRead())
            {
                using(BinaryReader br = new BinaryReader(fs))
                {


                    
                    while(br.BaseStream.Position != file.Length)
                    {
                        byte[] bytes = br.ReadBytes(800);

                        block_list.AddLast(bytes);





                    }



                    Log.Info($"总区块数量：{block_list.Count}");


                    //br.BaseStream.Position = 0;


                    //int file_count = 0;
                    //long start_address = br.BaseStream.Position;

                    //do
                    //{



                    //    uint header = br.ReadUInt32();
                    //    br.BaseStream.Seek(-4, SeekOrigin.Current);
                    //    Log.Info($"当前指针：{br.BaseStream.Position}，读到的四字节长度头：{header.ToString("X8")}");

                    //    if (header == 0x0)
                    //    {
                    //        Log.Info("是空文件。");

                    //        for(int i = 0; i <800; i++)
                    //        {

                    //        }

                    //        file_count++;

                    //    }
                    //    else

                    //    //Pres
                    //    if (header == 0x73657250)
                    //    {
                    //        Log.Info("是PRES。");
                    //    }
                    //    else

                    //    //Blz4
                    //    if (header == 0x347a6c62)
                    //    {
                    //        Log.Info("是BLZ4。");
                    //    }
                    //    else

                    //    {
                    //        throw new Exception($"错误的文件：{header.ToString("X8")}不是个范围以内的头！");
                    //    }






                    //}
                    //while (br.BaseStream.Position + 800 < file.Length);



                }
            }


            Console.ReadKey();
        }


        static void WriteFile(List<byte> data,string path)
        {

            if(File.Exists(path))
            {
                File.Delete(path);
            }


            File.WriteAllBytes(path,data.ToArray());

            Log.Info($"已经写入{data.Count}大小的文件于:{path}");

        }

        




    }
}
