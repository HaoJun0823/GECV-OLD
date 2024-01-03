using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GECV;

namespace GODVEIN
{
    internal class Program
    {


        static DirectoryInfo RootDirectory;
        static DirectoryInfo BaseDirectory;
        static DirectoryInfo ExtraDirectory;
        static DirectoryInfo QpckBlz4Directory;


        static void Main(string[] args)
        {

            Log.Info("GOD VEIN 噬神者资源映射 打包器 BY 兰德里奥（HaoJun0823）");
            Log.Info("https://blog.haojun0823.xyz/");
            Log.Info("https://github.com/HaoJun0823/GECV");


            if (args.Length != 0)
            {

                RootDirectory = new DirectoryInfo(args[0]);
                BaseDirectory = new DirectoryInfo(RootDirectory.FullName+"\\_BASE");
                ExtraDirectory = new DirectoryInfo(RootDirectory.FullName + "\\_EXTRA");
                QpckBlz4Directory = new DirectoryInfo(RootDirectory.FullName + "\\_UNPACK_QPCK_REAL");


                int input = -1;
                while (input != 0)
                {
                    Log.Info($"工作目录：{RootDirectory.FullName}");
                    Log.Info($"1.打包QPCK，优先打包{ExtraDirectory.FullName},其次打包{BaseDirectory.FullName}，生成文件到{RootDirectory}\\pack.qpck");
                    Log.Info($"2.压缩BLZ4，压缩{QpckBlz4Directory.FullName}，生成文件到{ExtraDirectory.FullName}");

                    try
                    {
                        input = Convert.ToInt32(Console.ReadLine());
                    }
                    catch (Exception ex)
                    {
                        input = -1;
                        Log.Error(ex.ToString());
                        Log.Error("你输入了个啥？");
                    }




                    switch (input)
                    {
                        case 1:
                            QPCK();
                            break;
                        case 2:
                            ProcessQPCKBLZ4();
                            break;
                        case 0:
                            input = 0;
                            break;
                        default:
                            input = -1;
                            break;
                    }



                }



            }
            else
            {
                throw new Exception("请带一个文件夹参数。");
            }




        }


        public static void ProcessQPCKBLZ4()
        {
            var files = QpckBlz4Directory.GetFiles("*.*", SearchOption.AllDirectories);

            foreach(var i in files)
            {

                string name = i.Name;

                byte[] data = BLZ4(i.FullName);

                File.WriteAllBytes(ExtraDirectory + "\\" + name, data);

            }



        }


        public static void QPCK()
        {

            var base_files = BaseDirectory.GetFiles("*.*", SearchOption.AllDirectories);
            var extra_files = ExtraDirectory.GetFiles("*.*", SearchOption.AllDirectories);

            Dictionary<string, string> map = new Dictionary<string, string>();


            FileInfo qpck = new FileInfo(RootDirectory.FullName + "\\pack.qpck");

            if (qpck.Exists)
            {

                qpck.Delete();

            }


            foreach(var i in base_files)
            {
                map.Add(i.Name, i.FullName);
                Log.Info($"原文件:{i.Name},地址：{i.FullName}");
            }

            foreach(var i in extra_files)
            {
                if (map.ContainsKey(i.Name))
                {
                    map[i.Name] = i.FullName; 
                    Log.Info($"新文件:{i.Name},地址：{i.FullName}，劫持！");
                }
                else
                {
                    throw new FileNotFoundException($"没找到{i.FullName}，你确定这个东西是游戏的一部分吗？");
                }


            }

            //long offset = br.ReadInt64();
            //long hash = br.ReadInt64();
            //int size = br.ReadInt32();
            //8header+8+8+4

            int files_count = base_files.Length;
            int info_chunk_size = files_count * 20;
            int data_chunk_size = 0;
            int header_size = 8;



            Log.Info($"虚拟文件：信息块大小：{info_chunk_size}");

            using (BinaryWriter bw = new BinaryWriter(qpck.OpenWrite()))
            {
                bw.Write(Define.QPCK_MAGIC);
                bw.Write(files_count);

                List<FileInfo> files = new List<FileInfo>();

                foreach (var kv in map)
                {
                    FileInfo file = new FileInfo(kv.Value);
                    files.Add(file);
                    long file_offset = header_size + info_chunk_size + data_chunk_size;
                    string str_hash = file.Name.Split('.')[0].Split('_')[1];
                    long int_hash = Convert.ToInt64(str_hash, 16);
                    int file_length = Convert.ToInt32(file.Length);
                    Log.Info($"虚拟文件{kv.Value}：头信息-推算偏移：{file_offset}，哈希：{int_hash}:{str_hash}，大小：{file_length}");

                    bw.Write(file_offset);
                    bw.Write(int_hash);
                    bw.Write(file_length);
                    data_chunk_size+= file_length;
                    


                }

                foreach (var i in files) {

                    byte[] bytes = File.ReadAllBytes(i.FullName);
                    Log.Info($"写入数据：{bytes.Length}，来自文件:{i.FullName}");
                    bw.Write(bytes);
                    


                }


            }


        }



        public static byte[] BLZ4(string path)
        {

            using (MemoryStream ms = new MemoryStream())
            {

                using (BinaryWriter bw = new BinaryWriter(ms))
                {

                    byte[] file_data = File.ReadAllBytes(path);

                    bw.Write(Define.BLZ4_MAGIC);

                    Log.Info($"写入魔术码{Define.BLZ4_MAGIC}");

                    bw.Write(Convert.ToInt32(file_data.Length));
                    Log.Info($"写入文件大小{file_data.Length}");
                    bw.Write(0L);
                    Log.Info($"写入0L");
                    System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    byte[] retVal = md5.ComputeHash(file_data);
                    Log.Info($"写入MD5:{CryptUtils.GetMD5HashFromBytes(file_data)}");
                    bw.Write(retVal);


                    var split_data = BLZ4Utils.SplitBytes(file_data, 65536);
                    Log.Info($"即将写入压缩数据：总块数：{split_data.Count}");

                    byte[] compress;

                    if (split_data.Count > 1)
                    {
                        BLZ4Utils.CompressData(split_data[split_data.Count - 1], out compress);

                        bw.Write(compress.Length);
                        bw.Write(compress);
                        Log.Info($"写入最后一块到头部：大小：{split_data[split_data.Count - 1].Length}");

                        for (int i = 0; i < split_data.Count - 1; i++)
                        {


                            BLZ4Utils.CompressData(split_data[i], out compress);

                            bw.Write(compress.Length);
                            bw.Write(compress);
                            Log.Info($"顺序写入区块，大小：{split_data[i].Length}");

                        }
                    }
                    else
                    {
                        for (int i = 0; i < split_data.Count; i++)
                        {


                            BLZ4Utils.CompressData(split_data[i], out compress);

                            bw.Write(compress.Length);
                            bw.Write(compress);
                            Log.Info($"顺序写入区块，大小：{split_data[i].Length}");

                        }
                    }






                    byte[] result = ms.GetBuffer();
                    Log.Info($"返回数据:{result.Length}，压缩比率：{(double)result.Length / (double)file_data.Length}");
                    return result;


                }
            }

        }


    }
}
