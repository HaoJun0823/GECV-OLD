using DotNetGraph.Compilation;
using DotNetGraph.Core;
using DotNetGraph.Extensions;
using System.Data;
using static GECV.Log;
using static GERDP.DGMLWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GERDP
{
    internal class Program
    {

        static FileInfo SystemRES;
        static FileInfo SystemUpdateRES;
        static FileInfo DataRDP;
        static FileInfo PackageRDP;
        static FileInfo PatchRDP;


        public static byte[] System;
        public static byte[] SystemUpdate;
        public static byte[] Data;
        public static byte[] Package;
        public static byte[] Patch;


        public static DirectoryInfo TargetDirectiory;
        static DirectoryInfo SourceDirectiory;

        public static DGMLWriter DGML = new DGMLWriter();
        public static DotGraph dot_graph;

        static bool IsPS4;

        static async Task Main(string[] args)
        {
            Info("CODE EATER 噬神者 RDP 解包器 BY 兰德里奥（HaoJun0823）");
            Info("https://blog.haojun0823.xyz/");
            Info("https://github.com/HaoJun0823/GECV");
            if (args.Length < 3)
            {
                Info($"你输入的参数数量不对：第一个参数：原始数据文件夹，第二个参数：解包文件夹，第三个参数：ps4/psv");
            }


            SourceDirectiory = new DirectoryInfo(args[0]);
            TargetDirectiory = new DirectoryInfo(args[1]);

            if (args[2].ToLower().Equals("ps4"))
            {
                IsPS4 = true;
            }
            else if (args[2].ToLower().Equals("psv"))
            {
                IsPS4 = false;
            }
            else
            {
                Info($"{args[2]}是个什么玩意啊？请输入ps4或者psv");
                return;
            }

            SystemRES = new FileInfo(SourceDirectiory.FullName + "\\system.res");
            SystemUpdateRES = new FileInfo(SourceDirectiory.FullName + "\\system_update.res");
            DataRDP = new FileInfo(SourceDirectiory.FullName + "\\data.rdp");
            PackageRDP = new FileInfo(SourceDirectiory.FullName + "\\package.rdp");
            PatchRDP = new FileInfo(SourceDirectiory.FullName + "\\patch.rdp");

            Info($"=====");
            PrintFileStatus(SystemRES);
            PrintFileStatus(SystemUpdateRES);
            PrintFileStatus(DataRDP);
            PrintFileStatus(PackageRDP);
            PrintFileStatus(PatchRDP);
            Info($"=====");
            PrintFileStatus(SourceDirectiory);
            PrintFileStatus(TargetDirectiory);
            Info($"=====");
            Info($"当前数据类型：{(IsPS4 ? "PS4" : "PSV")}");
            Info("请核实这些数据，以免发生意外，按任意键开始解包！");
            Console.ReadKey();
            Parallel.Invoke(() => { System = ReadAllBytes(SystemRES); }, () => { SystemUpdate = ReadAllBytes(SystemUpdateRES); }, () => { Data = ReadAllBytes(DataRDP); }, () => { Package = ReadAllBytes(PackageRDP); }, () => { Patch = ReadAllBytes(PatchRDP); });
            Info($"关联文件读取完毕！解包需要清空，所以再次输入任意按键删除{TargetDirectiory.FullName}。");
            Console.ReadKey();
            TargetDirectiory.Delete(true);

            TargetDirectiory.Create();


            //Parallel.Invoke(() => {
            //    if (SystemRES.Exists)
            //    {
            //        Res SR = new Res("system", System, IsPS4);


            //        SR.SetDecoderSaveFolder(TargetDirectiory.FullName);

            //        SR.DecodeAll();
            //    }


            //}, () => {

            //    if (SystemUpdateRES.Exists)
            //    {
            //        Res SUR = new Res("system_update", SystemUpdate, IsPS4);

            //        SUR.SetDecoderSaveFolder(TargetDirectiory.FullName);

            //        SUR.DecodeAll();
            //    }


            //});

            

            Info($"请输入选项，输入1处理system.res，输入2处理system_update.res，如果输入错误程序会退出请重新打开再进行。\n注意新的解包会覆盖日志和原始文件，你需要注意这一点！");
            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    Res SR = new Res("system", System, IsPS4); SR.SetDecoderSaveFolder(TargetDirectiory.FullName); dot_graph = new DotGraph().WithIdentifier("system"); SR.DecodeAll();
                    break;
                case "2":
                    Res SUR = new Res("system_update", SystemUpdate, IsPS4); SUR.SetDecoderSaveFolder(TargetDirectiory.FullName); dot_graph = new DotGraph().WithIdentifier("system_update"); SUR.DecodeAll();
                    break;
                default:
                    Info($"你还真就输错了！{input}是什么？");
                    break;
            }


            Info($"完成！日志系统关闭！");


            GECV.Utils.WriteListToFile(GECV.Log.LogRecord, TargetDirectiory.FullName + "\\GERDP.log");
            GECV.Utils.WriteListToFile(ExperimentalDecoder.LogList, TargetDirectiory.FullName + "\\GERDP.ExperimentalDecoder.error.log");
            GECV.Utils.WriteListToFile(ExperimentalDecoder.warningLogList, TargetDirectiory.FullName + "\\GERDP.ExperimentalDecoder.warning.log");


            ExperimentalDecoder.LogList.Clear();
            ExperimentalDecoder.warningLogList.Clear();

            

            DGML.Serialize(TargetDirectiory.FullName+"\\graph.dgml");

            await using var writer = new StringWriter();
            var context = new CompilationContext(writer, new CompilationOptions());
            DGML.BuildDotGraph(dot_graph);
            await dot_graph.CompileAsync(context);

            var result = writer.GetStringBuilder().ToString();

            // Save it to a file
            File.WriteAllText(TargetDirectiory.FullName+"\\graph.dot", result);

            Console.WriteLine("处理完毕！");
            Console.ReadKey();



        }
        static byte[] ReadAllBytes(FileInfo file)
        {

            byte[] result;
            if (file.Exists)
            {
                result = File.ReadAllBytes(file.FullName);
            }
            else
            {
                result = new byte[0];
            }

            Info($"{file.Name}读取完毕！读取到的大小:{result.Length}");
            return result;
        }


        static void PrintFileStatus(FileSystemInfo info)
        {

            if (info.Exists)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Info($"数据项：{info.FullName}，状态：{(info.Exists ? "存在" : "缺失")}");

            Console.ResetColor();

        }


        string GetRelativePath(string root, string file)
        {



            if (root[root.Length - 1] == '\\')
            {
                return file.Substring(root.Length);
            }
            else
            {
                return file.Substring(root.Length + 1);
            }





        }

    }
}
