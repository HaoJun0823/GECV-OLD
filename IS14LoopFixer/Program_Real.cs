using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS14LoopFixer
{
    internal class Program_Real
    {

        static Dictionary<string, string> map = new Dictionary<string, string>();

        static DataTable dt;

        static void Main(string[] args)
        {

            string[] filespath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory+"\\IS14\\","*.*");


            dt = MiniExcel.QueryAsDataTable(AppDomain.CurrentDomain.BaseDirectory + "\\at9_data.xlsx", useHeaderRow: true);


            Console.WriteLine($"You have {filespath.Length} is14 files");

            foreach (string s in filespath) {


                map.Add(Path.GetFileName(s), s);
            
            
            }



            foreach(DataRow r in dt.Rows)
            {

                string filename = r["Name"].ToString();


                Console.WriteLine($"Does {filename} alive? {map.ContainsKey(filename)}");

                if (map.ContainsKey(filename))
                {

                    byte[] data = File.ReadAllBytes(map[filename]);


                    if (BitConverter.ToInt32(data,0x28) == 1886351212)
                    {
                        Console.WriteLine("But this is loop file!");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Make this to loop file!");
                    }


                    using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(AppDomain.CurrentDomain.BaseDirectory + "\\Build\\" + Path.GetFileName(map[filename]))))
                    {

                        bw.Write(data.Take(0x28).ToArray());

                        bw.Seek(0x04, SeekOrigin.Begin);

                        bw.Write(BitConverter.GetBytes(data.Length + 0x10).Reverse().ToArray());

                        bw.Seek(0x20,SeekOrigin.Begin);

                        bw.Write(0x0);

                        bw.Seek(0x28, SeekOrigin.Begin);


                        byte[] unk = { 0x6c, 0x6f, 0x6f, 0x70, 0x00, 0x00, 0x00, 0x08 };

                        bw.Write(unk);

                        bw.Write(Convert.ToInt32(r["Start"].ToString(),16));
                        bw.Write(Convert.ToInt32(r["End"].ToString(),16));

                        bw.Write(data.Skip(0x28).ToArray());

                        bw.Flush();
                    }
                    


                }

            }
            Console.WriteLine("Done");

            Console.ReadKey();

        }

    }
}
