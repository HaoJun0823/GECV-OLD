using GECV_EX.Utils;

namespace GECV_EX_BLZ4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("GECV EX BLZ4 BY RANDERION(HAOJUN0823)");
            Console.WriteLine("https://blog.haojun0823.xyz/");
            Console.WriteLine("https://github.com/HaoJun0823/GECV");


            if(args.Length !=3)
            {

                Console.WriteLine($"Your Need 3 Args:");
                Console.WriteLine("(Unpack):1.{unpack} 2.{blz4 file} 3.{The name of the unzipped file.}");
                Console.WriteLine("(Pack):1.{pack} 2.{original file} 3.{The name of the new blz4 file.}");

            }
            else
            {
                if (args[0].ToLower().Equals("unpack"))
                {
                    Unpack(args[1], args[2]);
                    return;
                }
                if (args[0].ToLower().Equals("pack"))
                {
                    pack(args[1], args[2]);
                    return;
                }

                Console.WriteLine($"What is {args[0]}? input (unpack/pack) please.");

                return;

            }




        }



        public static void Unpack(string blz4,string file)
        {

            byte[] blz4_data = File.ReadAllBytes(blz4);
            byte[] unpack_data;


            unpack_data = BLZ4Utils.UnpackBLZ4Data(blz4_data);


            Console.WriteLine($"Unpack:{blz4}=>{file}.");
            

            File.WriteAllBytes(file, unpack_data);


        }

        public static void pack(string file, string blz4)
        {

            byte[] file_data = File.ReadAllBytes(file);
            byte[] pack_data;


            pack_data = BLZ4Utils.PackBLZ4Data(file_data);


            Console.WriteLine($"Pack:{file}=>{blz4}.");


            File.WriteAllBytes(blz4,pack_data);


        }

    }
}
