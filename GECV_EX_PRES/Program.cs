using GECV_EX.PC;
using GECV_EX.PC.Packer;
using GECV_EX.Utils;
using System.Collections.Generic;

namespace GECV_EX_PRES
{
    internal class Program
    {


        static DirectoryInfo ToDir;
        static DirectoryInfo RootDir;
        


        static void Main(string[] args)
        {


            Console.WriteLine("GECV EX PRES BY RANDERION(HAOJUN0823)");
            Console.WriteLine("https://blog.haojun0823.xyz/");
            Console.WriteLine("https://github.com/HaoJun0823/GECV");

            foreach(var i in args)
            {
                Console.WriteLine($"Args:{i}");
            }


            if(args.Length == 1)
            {

                if (Path.Exists(args[0]))
                {
                    RootDir = new DirectoryInfo(args[0]);
                }

                var files = RootDir.GetFiles("*.pres",SearchOption.TopDirectoryOnly);

                List<string> list = new List<string>();

                foreach(var f in files)
                {
                    try
                    {
                        byte[] b = File.ReadAllBytes(f.FullName);

                        Console.WriteLine($"Read{f.FullName}.Length:{b.Length}");

                        PresPC pres = new PresPC(b);
                        list.Add(f.FullName+",PASS");
                    }catch (Exception e)
                    {
                        Console.WriteLine($"Error:{f.FullName}:{e.Message}");
                        list.Add(f.FullName + $",ERROR:{e.Message}");
                    }
                }


                File.WriteAllLines(RootDir.FullName+"\\gecv_pres_vaild.log", list);

                

                Console.WriteLine("Press Any Key To Exit.");
                Console.ReadKey();
            }


            if (args.Length <3) {

                Console.WriteLine("You Need 3 Args:\n0.{pack}Or{unpack}\n1.{Pres File}\n2.{Target Directory}");
                Console.WriteLine("VAILD MODE:You Need 1 Args:\n1.{Target Directory}");



            }
            else
            {

                //FromDir = new DirectoryInfo(args[1]);
                //ToDir = new DirectoryInfo(args[2]);
                

                if (args[0].Equals("unpack"))
                {

                    if (Path.Exists(args[1]))
                    {
                        string path = args[1];

                        RootDir = new DirectoryInfo(Path.GetFullPath(args[2]) + '\\' + Path.GetFileNameWithoutExtension(path));

                        if(RootDir.Exists) { 
                        
                        RootDir.Delete(true);
                        }

                        RootDir.Create();

                        ToDir =  new DirectoryInfo(RootDir.FullName + "\\Data");
                        ToDir.Create();


                        Decode(new FileInfo(path));

                        


                    }


                }

                if (args[0].Equals("pack"))
                {

                    if (Path.Exists(args[2]))
                    {

                        string path = args[2];

                        RootDir = new DirectoryInfo(path);

                        PresPacker pk = new PresPacker(RootDir.FullName);

                        byte[] result = pk.GetPresFileBytes();


                        
                        File.WriteAllBytes(args[1], result);

                        File.WriteAllLines(RootDir.FullName + "\\register_list.log", pk.GetRegisterText() );
                        File.WriteAllLines(RootDir.FullName + "\\gecv_book_list.log", pk.GetBookInformation());

                        PresPC pres = new PresPC(result);

                    }



                }


            }
            
            Console.WriteLine("Press Any Key To Exit.");
            Console.ReadKey();

        }


        static void Decode(FileInfo file)
        {
            byte[] b = File.ReadAllBytes(file.FullName);

            Console.WriteLine($"Read{file.FullName}.Length:{b.Length}");

            PresPC pres = new PresPC(b);

            pres.Unpack(ToDir.FullName);

            //FileUtils.CreateSymbolLinkDosCommandFile(pres.symbol_map,RootDir.FullName);

            string mod_path = RootDir.FullName + "\\Mod\\";

            if (Path.Exists(mod_path))
            {
                File.Delete(mod_path);
            }

            Directory.CreateDirectory(mod_path);

            foreach (var kv in pres.symbol_map)
            {
                string target = mod_path+ kv.Value;


                Directory.CreateDirectory(Path.GetDirectoryName(target));

                //File.CreateSymbolicLink(target,kv.Key);


                if (String.IsNullOrEmpty(kv.Value))
                {
                    continue;
                }

                if (!File.Exists(target))
                {
                    File.Copy(kv.Key, target,true);
                    File.Copy(Path.GetDirectoryName(kv.Key) + "\\" + Path.GetFileNameWithoutExtension(kv.Key) + ".xml", Path.GetDirectoryName(target) + "\\" + Path.GetFileNameWithoutExtension(target) + ".xml", true);
                }


                string r_o_p = Path.GetRelativePath(ToDir.FullName,kv.Key);
                string r_t_p = Path.GetRelativePath (mod_path,target);

                File.AppendAllText(target + ".ini", $"{r_o_p}={r_t_p}\n");



            }




        }

    }
}
