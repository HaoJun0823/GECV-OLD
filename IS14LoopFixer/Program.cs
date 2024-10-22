using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS14LoopFixer
{

    struct AT9Loop
    {
        public int start;
        public int end;
    }

    internal class Program
    {


        static Dictionary<string, AT9Loop> AT9Map = new Dictionary<string, AT9Loop>();

        static void Main_Fake(string[] args)
        {

            File.Delete(AppDomain.CurrentDomain.BaseDirectory+"\\info.csv");

            LoadAT9LoopInformation();


            String[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\IS14\\", "*.is14");

            Dictionary<string,string> IS14Map = new Dictionary<string,string>();

            foreach(var i in files)
            {
                IS14Map.Add(Path.GetFileNameWithoutExtension(i), i);
            }
            

            


            foreach(var kv in AT9Map)
            {
                BuildLoopIS14(kv.Value, IS14Map[kv.Key]);
            }



           //MiniExcel.SaveAs(AppDomain.CurrentDomain.BaseDirectory + "\\info.csv",AT9Map);


        }



        static void LoadAT9LoopInformation()
        {

            String[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\AT9\\", "*.at9");


            foreach (String file in files) {
            
                AT9Loop loop = new AT9Loop();

                using(BinaryReader br = new BinaryReader(File.OpenRead(file)))
                {

                    br.BaseStream.Seek(0x90, SeekOrigin.Begin);
                    loop.start = br.ReadInt32();
                    loop.end = br.ReadInt32();

                }
            
            
                AT9Map.Add(Path.GetFileNameWithoutExtension(file), loop);
            
            }
            


        }


        static void BuildLoopIS14(AT9Loop at9,string is14)
        {

            byte[] file = File.ReadAllBytes(is14);

            byte[] header = file.Take(0x28).ToArray();

            byte[] body = file.Skip(0x28).ToArray();



            int unk1 = BitConverter.ToInt32(file,0x4) + 0x10;

            int unk2 = 0x0;

            byte[] unk3 = { 0X6C, 0X6F, 0X6F, 0X70, 0X0, 0X0, 0X0, 0X08 };

            byte[] unk4 = BitConverter.GetBytes(at9.start).Reverse().ToArray();

            byte[] unk5 = BitConverter.GetBytes(at9.end).Reverse().ToArray();

            int mask = BitConverter.ToInt32(file,0x28);

            if (mask == 1886351212)
            {
                return;
            }



            byte[] fix_header;

            MemoryStream ms = new MemoryStream();

            using(BinaryWriter bw = new BinaryWriter(ms))
            {

                bw.Write(header);

                bw.Seek(0,SeekOrigin.Begin);


                bw.Seek(0x04,SeekOrigin.Begin);
                bw.Write(unk1);
                bw.Flush();
                bw.Seek(0x10, SeekOrigin.Begin);
                bw.Write(unk2);
                bw.Flush();

                bw.Seek(0x28, SeekOrigin.Begin);
                bw.Write(unk3);
                bw.Flush();
                bw.Write(unk4);
                bw.Flush();
                bw.Write(unk5);
                bw.Flush();

                fix_header = ms.ToArray();
            }


            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(AppDomain.CurrentDomain.BaseDirectory+"\\Build\\"+Path.GetFileNameWithoutExtension(is14)+".is14")))
            {
                bw.Write(fix_header);
                bw.Write(body);
                bw.Flush();
            }





        }
        

    }




    
}
