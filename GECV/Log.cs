using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GECV
{
    public class Log
    {

        public static List<string> LogRecord = new List<string>();

        public static string Info(string str)
        {
            string str2 = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}][线程：{Thread.CurrentThread.ManagedThreadId}]{str}";
            Console.WriteLine(str2);
            LogRecord.Add(str2);
            return str2;

            

        }


        public static void Error(string str)
        {
            Console.Error.WriteLine(str);
            LogRecord.Add(str);
        }

        public static void Pass()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("【通过】");
            Console.ResetColor();
        }

        public static void Expection()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("【异常】");
            Console.ResetColor();
        }

        


    }
}
