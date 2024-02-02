using GECV;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RDPFUCKER
{




    internal class Program
    {

        enum ReaderStatus
        {

            BLANK, PRES, BLZ4

        }




        static byte[] rdp_file;

        static byte[] pres_header = { 0x50, 0x72, 0x65, 0x73 };
        static byte[] blz4_header = { 0x62, 0x6c, 0x7a, 0x34 };

        static int pres_header_count = 0;
        static int blz4_header_count = 0;

        static ReaderStatus global_status = ReaderStatus.BLANK;
        static long global_count = 0;

        static List<int> pres_tier = new List<int>();

        static DirectoryInfo target_dir;

        static void Main(string[] args)
        {

            if (args.Length != 2)
            {


                throw new ArgumentException($"需要两个参数：第一个，rdp文件，第二个，解压的文件夹。");
            }


            FileInfo file = new FileInfo(args[0]);
            target_dir = new DirectoryInfo(args[1]);

            if (!target_dir.Exists)
            {
                target_dir.Create();
            }



            if (file.Length % 16 != 0)
            {
                Log.Info($"RDP长度可能不对！：{file.Length}无法被16整除！要不你自己补几个0？");
            }

            rdp_file = File.ReadAllBytes(args[0]);

            Log.Info($"已经读取：{rdp_file.Length}，开始解包。");
            Work();

            Console.ReadKey();
        }


        static void WriteFile(List<byte> data, string path)
        {

            if (File.Exists(path))
            {
                File.Delete(path);
            }


            File.WriteAllBytes(path, data.ToArray());

            Log.Info($"已经写入{data.Count}大小的文件于:{path}");
            global_count++;
        }

        static void Work()
        {

            Log.Info("主线程启动！");
            // 0x50,0x72,0x65,0x73 0x20 0x00 0x00 0x00 Pres
            //status_map.Add(new byte[] {0x50,0x72,0x65,0x73 },0);

            //BLZ4
            //status_map.Add(new byte[] { 0x62,0x6c,0x7a,0x34 }, 0);



            using (MemoryStream ms = new MemoryStream(rdp_file))
            {


                using (BinaryReader br = new BinaryReader(ms))
                {


                    do
                    {
                        byte b = br.ReadByte();
                        //Log.Info($"读取到了{b}。");
                        global_status = GetReaderStatus(b);


                        if (global_status == ReaderStatus.BLANK)
                        {

                        }


                        if (global_status == ReaderStatus.BLZ4)
                        {
                            //br.BaseStream.Seek(-4, SeekOrigin.Current);
                            GetTier0BLZ4(br);






                        }

                        if(global_status == ReaderStatus.PRES)
                        {
                            //global_status = ReaderStatus.BLANK;
                            GetPresFileCount(br);
                           global_status = ReaderStatus.BLANK;



                        }

                    } while (br.BaseStream.Position < rdp_file.LongLength);




                }






            }




        }


        //status是BLZ4，切回BLANK，直到下一个状态出现为止

        static int GetPresFileCount(BinaryReader br)
        {

            //记录基础地址

            br.BaseStream.Seek(-4, SeekOrigin.Current);

            long original_address = br.BaseStream.Position;
            int magic = br.ReadInt32();
            int magic_space = br.ReadInt32();
            int magic_1 = br.ReadInt32();
            int magic_2 = br.ReadInt32();
            int magic_3 = br.ReadInt32();
            long offset_data = br.ReadInt32();
            long zerozero = br.ReadInt64();
            int count_set = br.ReadInt32();

            long index_root = br.BaseStream.Position;
            long index_set = 0;
            long index_file = 0;

            Log.Info($"魔法码1:{magic_1}");
            Log.Info($"魔法码2:{magic_2}");
            Log.Info($"魔法码3:{magic_3}");
            Log.Info($"偏移数据:{offset_data}");
            Log.Info($"zerozero:{zerozero}");

            Log.Info($"集合数量:{count_set}");
            //offset_data +=  original_address;
            //Log.Info($"修正偏移数据:{offset_data}");

            for (int i = 0; i < count_set; i++)
            {

                int set_offset = 0;
                int set_length = 0;

                if (count_set > 1)
                {
                    set_offset = br.ReadInt32();
                    set_length = br.ReadInt32();
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

                //3 real
                int info_off = br.ReadInt32();
                int count_file = br.ReadInt32();


                int set_unk3 = br.ReadInt32();
                int set_unk4 = br.ReadInt32();

                int set_unk5 = br.ReadInt32();
                int set_unk6 = br.ReadInt32();

                int set_unk7 = br.ReadInt32();
                int set_unk8 = br.ReadInt32();

                //7 src
                int set_unk9 = br.ReadInt32();
                int set_unk10 = br.ReadInt32();



                int set_unk11 = br.ReadInt32();
                int set_unk12 = br.ReadInt32();




                Log.Info($" 集合{i + 1}/{count_set}的文件A区是{set_unk9}，B区是{set_unk10}");





                br.BaseStream.Seek(set_unk9, SeekOrigin.Begin);


                for (int fi = 0; fi < set_unk10; fi++)
                {

                    int set_data_7_address = Convert.ToInt32(br.BaseStream.Position);

                    int set_data_7_data_offset = br.ReadInt32();
                    int set_data_7_data_offset_real = Convert.ToInt32(set_data_7_data_offset.ToString("X8").Substring(1), 16);
                    int set_data_7_data_length = br.ReadInt32();
                    int set_data_7_header_offset = br.ReadInt32();
                    int set_data_7_header_data = br.ReadInt32();

                    Log.Info($"     偏移：{set_data_7_data_offset.ToString("X8")}");
                    Log.Info($"     真实偏移：{set_data_7_data_offset_real}");
                    Log.Info($"     长度：{set_data_7_data_length}");
                    Log.Info($"     头偏移：{set_data_7_header_offset}");
                    Log.Info($"     头数据？：{set_data_7_header_data}");

                    //if (set_data_7_data_offset > file.Length || set_data_7_header_offset > file.Length)
                    //{
                    //    Log.Info($"{file.Name}的第七区偏移大于整个文件，这是有效的文件？");
                    //    continue;
                    //}

                    index_file = br.BaseStream.Position;

                    br.BaseStream.Seek(set_data_7_header_offset, SeekOrigin.Begin);

                    int set_data_7_header_offset_offset = br.ReadInt32();
                    br.BaseStream.Seek(set_data_7_header_offset_offset, SeekOrigin.Begin);
                    string set_data_7_header_offset_offset_data = Utils.readNullterminated(br);

                    br.BaseStream.Seek(set_data_7_data_offset_real, SeekOrigin.Begin);

                    //StringBuilder strbuild = new StringBuilder();

                    //for (int si = 0; si < set_data_7_data_length; si++)
                    //{
                    //    strbuild.Append(Convert.ToChar(br.ReadByte()));
                    //}

                    //string set_data_7_data =strbuild.ToString();

                    byte[] utf_bytes = new byte[set_data_7_data_length];

                    for (int si = 0; si < utf_bytes.Length; si++)
                    {
                        utf_bytes[si] = br.ReadByte();
                    }

                    string set_data_7_data = new UTF8Encoding().GetString(utf_bytes);


                    Log.Info($"     数据{fi + 1}/{count_file}");



                    Log.Info($"     偏移：{set_data_7_data_offset.ToString("X8")}");
                    Log.Info($"     真实偏移：{set_data_7_data_offset_real}");
                    Log.Info($"     长度：{set_data_7_data_length}");
                    Log.Info($"     数据：{set_data_7_data}");

                    Log.Info($"     头偏移：{set_data_7_header_offset}");
                    Log.Info($"     头数据？：{set_data_7_header_data}");

                    Log.Info($"     头偏移的偏移：{set_data_7_header_offset_offset}");
                    Log.Info($"     头偏移的偏移的数据：{set_data_7_header_offset_offset_data}");



                    br.BaseStream.Position = index_file;
                    br.BaseStream.Seek(16, SeekOrigin.Current);
                }
                br.BaseStream.Position = index_set;




            }

            return 0;




        }

        static void GetTier0BLZ4(BinaryReader br)
        {

            global_status = ReaderStatus.BLANK;

            List<byte> list = new List<byte>();

            list.Add(blz4_header[0]);
            list.Add(blz4_header[1]);
            list.Add(blz4_header[2]);
            list.Add(blz4_header[3]);

            long base_address = br.BaseStream.Position;
            byte b;
            do
            {
                b = br.ReadByte();
                list.Add(b);


                if (list.Count % 1024 == 0)
                {
                    Log.Info($"【BLZ4】目前读到了：{list.Count / 1024}。");
                }
                global_status = GetReaderStatus(b);
            } while (global_status == ReaderStatus.BLANK);

            Log.Info($"【BLZ4】读到了其他文件，状态为：{global_status}，文件长度：{list.Count}。");
            list.RemoveAt(list.Count-1);
            list.RemoveAt(list.Count-1);
            list.RemoveAt(list.Count-1);
            list.RemoveAt(list.Count-1);

            Log.Info($"【BLZ4】去头，文件长度：{list.Count}。");
            //因为BLZ4是有块长度的所以末尾根本不需要考虑00的问题。

            if (list.Count % 16 != 0)
            {
                Log.Info($"警告，这个长度不是16的倍数，可能有问题？");
            }

            br.BaseStream.Seek(-4, SeekOrigin.Current);

            global_status = ReaderStatus.BLANK;
            WriteFile(list, $"{target_dir}{global_count.ToString("X8")}_{base_address.ToString("X8")}_{br.BaseStream.Position.ToString("X8")}_{list.Count.ToString("X8")}.blz4");


        }

        static ReaderStatus GetReaderStatus(byte b)
        {

            if (global_status != ReaderStatus.BLANK)
            {
                return global_status;
            }


            if (b == pres_header[pres_header_count])
            {
                pres_header_count++;
            }
            else
            {
                pres_header_count = 0;
            }

            if (b == blz4_header[blz4_header_count])
            {
                blz4_header_count++;
            }
            else
            {
                blz4_header_count = 0;
            }


            if (pres_header_count == pres_header.Length)
            {
                pres_header_count = 0;
                return ReaderStatus.PRES;
            }

            if (blz4_header_count == blz4_header.Length)
            {
                blz4_header_count = 0;
                return ReaderStatus.BLZ4;
            }






            return ReaderStatus.BLANK;


        }





    }
}
