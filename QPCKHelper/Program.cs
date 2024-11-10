using GECV.TINY;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace QPCKHelper
{
    internal class Program
    {

        static FileInfo fileA, fileB;
        static DirectoryInfo dirC;


        struct TaskData
        {
            public string origin;

            public string target;

            public string save;
        }

        static void Main(string[] args)
        {

            Info("CODE EATER 噬神者资源映射 QPCK对比器 BY 兰德里奥（HaoJun0823）");
            Info("https://blog.haojun0823.xyz/");
            Info("https://github.com/HaoJun0823/GECV");
            Info("参考：https://github.com/mhvuze/GEUndub/ By mhvuze");
            Info(@"License: MiniExcel,Zlib.Net,Protobuf-net,Protobuf-net-data");

            Info("You Need 3 Args:1.origin game data dir,2.modify game data dir,3.extract game data dir.");

            fileA = new FileInfo(args[0]);
            fileB = new FileInfo(args[1]);
            dirC = new DirectoryInfo(args[2]);

            TaskAll();

        }

        static void Info(string msg) { 
        
        Console.WriteLine(msg);
        }


        static void TaskAll()
        {
            Stream streamA = fileA.OpenRead();
            Stream streamB = fileB.OpenRead();

            QPCKExtracter qpckA = new QPCKExtracter(streamA);
            QPCKExtracter qpckB = new QPCKExtracter(streamB);

            Console.WriteLine("StreamA:" + streamA.CanRead);
            Console.WriteLine("StreamB:" + streamA.CanRead);
            if (!dirC.Exists) { 
            
                dirC.Create();

            }

            QPCKComparator.ExtractAllTargetDifferentData(qpckA,qpckB,dirC.FullName+"\\");



        }


        


    }
}
