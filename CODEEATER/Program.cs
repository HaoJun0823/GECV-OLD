using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using GECV;

namespace CODEEATER
{
    internal class Program
    {

        public static FileInfo SourceFile;
        public static DirectoryInfo TargetDirectory;

        public static LinkedList<string> QPCKFileList = new LinkedList<string>();

        static void Main(string[] args)
        {
            Log.Info("CODE EATER 噬神者资源映射 解包器 BY 兰德里奥（HaoJun0823）");
            Log.Info("https://blog.haojun0823.xyz/");

            Log.Info(string.Format("QPCK文件：{0}", args[0]));
            Log.Info(string.Format("解包目录：{0}", args[1]));

            if (args.Length >= 2)
            {

                if (File.Exists(args[0]))
                {
                    SourceFile = new FileInfo(args[0]);

                        TargetDirectory = new DirectoryInfo(args[1]);




                    TargetDirectory.Create();
                    Directory.CreateDirectory(TargetDirectory.FullName + "_BASE\\");
                    Directory.CreateDirectory(TargetDirectory.FullName + "_EXTRA\\");
                    Directory.CreateDirectory(TargetDirectory.FullName + "_UNPACK\\");
                    Directory.CreateDirectory(TargetDirectory.FullName + "_UNPACK_PRES_REAL\\");
                    Directory.CreateDirectory(TargetDirectory.FullName + "_UNPACK_QPCK_REAL\\");
                    //Directory.CreateDirectory(TargetDirectory.FullName + "_UNPACK_GNF_REAL\\");

                    Console.WriteLine("开始处理文件");

                    ProcessQPCK();
                    ProcessPRES();
                    ProcessBLZ4();
                    ProcessGNF();
                }
                else
                {
                    Log.Error("qpck文件不存在啊！");
                }



            }
            else
            {
                Log.Error("错误，你需要两个输入：1.原始qpck文件，2.解包后目标文件夹地址。");
            }

            Log.Info("按任意键退出……");
            Console.ReadKey();


        }

        static void ProcessGNF()
        {
            DirectoryInfo pres_directory = new DirectoryInfo(TargetDirectory.FullName + "_UNPACK_PRES_REAL\\");
            DirectoryInfo qpck_directory = new DirectoryInfo(TargetDirectory.FullName + "_UNPACK_QPCK_REAL\\");

            var files = pres_directory.GetFiles("*.gnf", SearchOption.AllDirectories);
            Log.Info($"处理解包gnf文件，一共有{files.Length}。");
            for (int i = 0; i < files.Length; i++)
            {

                Log.Info($"正在处理{i}/{files.Length}：");
                GNF(files[i]);

            }

            files = qpck_directory.GetFiles("*.gnf", SearchOption.AllDirectories);
            Log.Info($"处理qpck的gnf文件，一共有{files.Length}。");
            for (int i = 0; i < files.Length; i++)
            {

                Log.Info($"正在处理{i}/{files.Length}：");
                GNF(files[i]);

            }

        }

        static void GNF(FileInfo file) 
        {

            using(BinaryReader br = new BinaryReader(file.OpenRead()))
            {


                int magic = br.ReadInt32();
                int size = br.ReadInt32();

                int dds_magic = br.ReadInt32();

                br.BaseStream.Position = br.BaseStream.Position - 4;

                
                if(dds_magic != 542327876)
                {
                    Log.Info($"     非法的GNF文件：{magic}，DDS存储大小：{size}，DDS魔术码：{dds_magic}。");
                    File.WriteAllText(file.FullName+".log", $"非法的GNF文件：{magic}，DDS存储大小：{size}，DDS魔术码：{dds_magic}（DDS=542327876）。");
                }
                else
                {
                    byte[] bytes = br.ReadBytes(size);
                    File.WriteAllBytes(file.FullName + ".dds", bytes);
                    Log.Info($"     GNF文件魔术码：{magic}，DDS存储大小：{size}，DDS魔术码：{dds_magic}，保存在:{file.FullName}.dds。");
                }


                

                




                


            }



        }

        static void ProcessQPCK()
        {


            using(BinaryReader br = new BinaryReader(SourceFile.Open(FileMode.Open)))
            {

                int magic = br.ReadInt32();
                int count = 0;
                if(magic != Define.QPCK_MAGIC) {
                    Log.Expection();
                    throw new Exception(string.Format("QPCK文件校验错误：头{0}应该是{1}！",magic,Define.QPCK_MAGIC));
                }
                else
                {
                    Log.Pass();
                    Log.Info(string.Format("QPCK文件校验结果：读取结果{0}等于{1}。", magic, Define.QPCK_MAGIC));
                    




                }

                Log.Info(string.Format("文件总数{0}", count = br.ReadInt32()));

                long base_offset = 0;
                for(int i = 0; i < count; i++)
                {

                    long offset = br.ReadInt64();
                    long hash = br.ReadInt64();
                    int size = br.ReadInt32();
                    Log.Info($"第{i+1}/{count}个文件位于偏移{offset}，文件大小：{size}字节，哈希校验：{hash}");

                    base_offset = br.BaseStream.Position; 

                    br.BaseStream.Seek(offset, SeekOrigin.Begin);
                    int type_magic = br.ReadInt32();
                    Log.Info($" 文件魔术码：{type_magic}");
                    
                    br.BaseStream.Seek(-4, SeekOrigin.Current);

                    byte[] bytes = br.ReadBytes(size);
                    string filename = (i + 1).ToString("D8") + "_" + hash.ToString("X16") + Define.GetExtension(type_magic);

                    QPCKFileList.AddLast(filename);

                    File.WriteAllBytes(TargetDirectory.FullName+"_BASE\\"+filename, bytes);

                    



                    Log.Info($" 提取{i+1}/{count}到{TargetDirectory.FullName}_BASE\\{filename}，大小{bytes.LongLength}字节。");

                    br.BaseStream.Position = base_offset;

                }



            }


        }


        static void ProcessPRES()
        {

            DirectoryInfo base_directory = new DirectoryInfo(TargetDirectory.FullName + "_BASE\\");
            DirectoryInfo target_directory = new DirectoryInfo(TargetDirectory.FullName + "_UNPACK\\");


            var files = base_directory.GetFiles("*.pres", SearchOption.AllDirectories);
            Log.Info($"处理PRES文件，一共有{files.Length}。");
            for (int i = 0; i < files.Length; i++)
            {

                Log.Info($"正在处理{i}/{files.Length}：");
                PRES(files[i],target_directory.FullName);

            }


        }


        static void PRES(FileInfo file, string workdirectory)
        {

            Log.Info($"读取PRES文件：{file.FullName},大小：{file.Length}");

            using (BinaryReader br = new BinaryReader(file.Open(FileMode.Open)))
            {

                int magic = br.ReadInt32();

                if (magic == Define.PRES_MAGIC)
                {
                    Log.Info($" PRES文件：{magic}等于{Define.PRES_MAGIC}");
                }
                else
                {
                    Log.Expection();
                    throw new Exception($"错误，{file.Name}不是一个正确的PRES文件，{magic}不等于{Define.PRES_MAGIC},确定这是对的？");

                }

                int magic_1 = br.ReadInt32();
                int magic_2 = br.ReadInt32();
                int magic_3 = br.ReadInt32();
                int offset_data = br.ReadInt32();
                long zerozero = br.ReadInt64();
                int count_set = br.ReadInt32();

                long index_root = br.BaseStream.Position;
                long index_set = 0;
                long index_file = 0;

                Log.Info($" 数据偏移：{offset_data}，集合数量：{count_set}");

                for(int i = 0; i < count_set; i++)
                {

                    if (count_set > 1)
                    {
                        int set_offset = br.ReadInt32();
                        int set_length = br.ReadInt32();
                        index_set = br.BaseStream.Position;
                        br.BaseStream.Seek(set_offset, SeekOrigin.Begin);
                        Log.Info($" 第{i + 1}/{count_set}集合在{set_offset}，集合大小：{set_length}");
                    }
                    else
                    {
                        Log.Info($" 集合数量是1，所以直接读取。");
                    }

                    // Read set info
                    int names_off = br.ReadInt32();
                    int names_elements = br.ReadInt32();
                    int set_unk1 = br.ReadInt32();
                    int set_unk2 = br.ReadInt32();
                    int info_off = br.ReadInt32();
                    int count_file = br.ReadInt32();
                    int set_unk3 = br.ReadInt32();
                    int set_unk4 = br.ReadInt32();
                    int set_unk5 = br.ReadInt32();
                    int set_unk6 = br.ReadInt32();
                    int set_unk7 = br.ReadInt32();
                    int set_unk8 = br.ReadInt32();

                    
                   

                    Log.Info($" 集合{i + 1}/{count_set}的文件偏移在{info_off}，数量为{count_file}");

                    br.BaseStream.Seek(info_off, SeekOrigin.Begin);
                    for (int fi=0; fi<count_file; fi++)
                    {
                        
                        int offset_file = br.ReadInt32();
                        int csize_file = br.ReadInt32();
                        int name_off_file = br.ReadInt32();
                        int name_elements_file = br.ReadInt32();
                        int file_unk1 = br.ReadInt32();
                        int file_unk2 = br.ReadInt32();
                        int file_unk3 = br.ReadInt32();
                        int usize_file = br.ReadInt32();

                        index_file = br.BaseStream.Position;

                        // Get individual file name info
                        br.BaseStream.Seek(name_off_file, SeekOrigin.Begin);
                        int name_off_final = br.ReadInt32();
                        int ext_off_final = br.ReadInt32();
                        int folder_off_final = br.ReadInt32();
                        int complete_off_final = br.ReadInt32();

                        // Get individual file path
                        string str_name_final = "";
                        string str_ext_final = "";
                        string str_folder_final = "";
                        string str_complete_final = "";
                        if (name_elements_file >= 1) { br.BaseStream.Seek(name_off_final, SeekOrigin.Begin); str_name_final = Utils.readNullterminated(br); }
                        if (name_elements_file >= 2) { br.BaseStream.Seek(ext_off_final, SeekOrigin.Begin); str_ext_final = Utils.readNullterminated(br); }
                        if (name_elements_file >= 3) { br.BaseStream.Seek(folder_off_final, SeekOrigin.Begin); str_folder_final = Utils.readNullterminated(br); }
                        if (name_elements_file >= 4) { br.BaseStream.Seek(complete_off_final, SeekOrigin.Begin); str_complete_final = Utils.readNullterminated(br); }


                        Log.Info($"     文件{fi + 1}/{count_file}");


                        string real_offset_file = offset_file.ToString("X8");

                        int real_offset = Convert.ToInt32(real_offset_file.Substring(1),16);

                        Log.Info($"     文件偏移：{real_offset_file}");
                        Log.Info($"     文件类型：{real_offset_file[0]}");
                        Log.Info($"     文件真实偏移原始数据：{real_offset_file.Substring(1)}");
                        Log.Info($"     文件真实偏移：{real_offset}");
                        Log.Info($"     文件名字：{str_name_final}");
                        Log.Info($"     文件扩展名：{str_ext_final}");
                        Log.Info($"     文件夹：{str_folder_final}");
                        Log.Info($"     最终输出：{str_complete_final}");

                       

                        string output_path = str_complete_final;

                        output_path = output_path.Replace( "/", "\\" );

                        Log.Info($"     矫正输出为Windows地址:{output_path}");

                        //int shifted_offset = offset_file & ((1 << (32 - 4)) - 1);
                        int shifted_offset = real_offset;
                        br.BaseStream.Seek(shifted_offset, SeekOrigin.Begin);
                        byte[] data = br.ReadBytes(csize_file);

                        Log.Info($"     文件原始偏移：{offset_file},文件修正偏移：{shifted_offset},文件大小:{csize_file}");
                        Log.Info($"     写出文件：{workdirectory + output_path}");

                        string target_directory = Path.GetDirectoryName( workdirectory+output_path );


                        Log.Info($"     给微软擦屁股:{target_directory}");

                        if (!Directory.Exists(target_directory))
                        {
                            Log.Info($"     创建目录{target_directory}");
                            Directory.CreateDirectory(target_directory);
                        }

                        if (output_path.Length <= 0)
                        {
                            Log.Info($"     这个文件不合法：{fi+1}/{count_file}，位于集合{i+1}/{count_set}，从属于{file.Name}文件，因为没有目录。");
                        }
                        else
                        {


                            if (real_offset_file[0] == 'B')
                            {
                                Log.Info($"     这个文件是虚拟的：{fi + 1}/{count_file}，位于集合{i + 1}/{count_set}，从属于{file.Name}文件，因为头偏移是B不是F，不输出。");
                            }
                            else if(data.Length <= 0)
                            {
                                Log.Info($"     这个文件是空的：{fi + 1}/{count_file}，位于集合{i + 1}/{count_set}，从属于{file.Name}文件，因为没有目录，不输出。");
                            }
                            else
                            {

                                StringBuilder file_magic = new StringBuilder();

                                file_magic.Append((char)data[0] );
                                file_magic.Append((char)data[1]);
                                file_magic.Append((char)data[2]);
                                file_magic.Append((char)data[3]);




                                Log.Info($"     输出文件的魔术码{file_magic.ToString()}");
                                File.WriteAllBytes(workdirectory + output_path, data);
                            }





                            
                        }

                        

                        br.BaseStream.Position = index_file;
                    }



                    br.BaseStream.Position = index_set;

                }




            }
        }

        static void ProcessBLZ4()
        {
            DirectoryInfo base_directory = new DirectoryInfo(TargetDirectory.FullName + "_BASE\\");
            DirectoryInfo unpack_directory = new DirectoryInfo(TargetDirectory.FullName + "_UNPACK\\");

            var files = unpack_directory.GetFiles("*",SearchOption.AllDirectories);
            Log.Info($"处理解包BLZ4文件，一共有{files.Length}。");
            for (int i = 0; i < files.Length; i++)
            {

                Log.Info($"正在处理{i}/{files.Length}：");
                BLZ4(files[i], files[i].FullName.Replace("_UNPACK", "_UNPACK_PRES_REAL"));

            }

            files = base_directory.GetFiles("*.blz4", SearchOption.AllDirectories);
            Log.Info($"处理qpck的BLZ4文件，一共有{files.Length}。");
            for (int i = 0; i < files.Length; i++)
            {

                Log.Info($"正在处理{i}/{files.Length}：");
                BLZ4(files[i], files[i].FullName.Replace("_BASE", "_UNPACK_QPCK_REAL"));

            }
        }


        static void BLZ4(FileInfo file,string targetPath)
        {

            Log.Info($"读取BLZ4文件：{file.FullName},大小：{file.Length}");

            using(BinaryReader br = new BinaryReader(file.Open(FileMode.Open)))
            {

                int magic = br.ReadInt32();

                if(magic == Define.BLZ4_MAGIC)
                {
                    Log.Info($" BLZ4文件：{magic}等于{Define.BLZ4_MAGIC}");
                }
                else
                {
                    Log.Expection();
                    Log.Error($"错误，{file.Name}不是一个正确的BLZ4文件，{magic}不等于{Define.BLZ4_MAGIC},确定这是对的？");
                    return;
                }

                int unpacked_size = br.ReadInt32();
                Log.Info($" 解压后大小:{unpacked_size}B");
                long zerozero = br.ReadInt64();

                byte[] md5 = br.ReadBytes(16);

                Log.Info($"MD5：");
                Utils.PrintByteArray(md5);


                List<byte[]> list = new List<byte[]>();
                LinkedList<byte[]> real_list = new LinkedList<byte[]>();

                int chunk = 0;

                while(br.BaseStream.Position< file.Length )
                {

                    int chunk_size = br.ReadUInt16();
                    byte[] data = new byte[chunk_size];
                    data = br.ReadBytes(chunk_size);


                    
                    Log.Info($"读取了第{chunk}个区块，大小为:{chunk_size}");

                    list.Add(data);
                    chunk++;
                }

                int all_chunk_size = 0;

                for(int i = 0; i < list.Count - 1; i++)
                {
                    real_list.AddLast(list[i]);
                    all_chunk_size += list[i].Length;
                    Log.Info($"添加{i + 1}个区块到真实列表，大小为:{list[i].Length}");
                }

                real_list.AddFirst(list[list.Count - 1]);
                all_chunk_size += list[list.Count-1].Length;
                Log.Info($"添加最后区块到真实列表头部，大小为:{list[list.Count - 1].Length}");

                Log.Info($"总区块大小:{all_chunk_size}");

                
                byte[] all_chunk = new byte[unpacked_size];



                LinkedList<byte[]> file_block_list = new LinkedList<byte[]>();

                foreach (byte[] data in real_list)
                {



                    MemoryStream stream = new MemoryStream(data);
                    byte[] out_file;
                    BLZ4Utils.DecompressData(data, out out_file);

                    file_block_list.AddLast(out_file);


                }
                int index = 0;
                foreach (byte[] data in file_block_list)
                {

                    foreach (byte b in data)
                    {
                        all_chunk[index++] = b;
                    }


                }


                Log.Info($"给微软擦屁股:{Path.GetDirectoryName(targetPath)}");

                if (!Directory.Exists(Path.GetDirectoryName(targetPath)))
                {
                    Log.Info($"创建目录{Path.GetDirectoryName(targetPath)}");
                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                }

                //byte[] result = BLZ4Utils.ZLibDecompress(all_chunk, unpacked_size);

                File.WriteAllBytes(targetPath, all_chunk);

                

                



            }


        }

    }
}
