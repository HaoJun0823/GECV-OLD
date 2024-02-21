using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace GECV
{
    public class Log
    {

        //public static List<string> LogRecord = new List<string>();

        public enum LogLevel
        {
            info, warning, error, fatal
        }

        public static Dictionary<LogLevel, Stack<String>> LogRecord = new Dictionary<LogLevel, Stack<String>>();

        public static Dictionary<LogLevel, TextWriter> LogWriter = new Dictionary<LogLevel, TextWriter>();

        private static DirectoryInfo LogDir = null;

        static Log() { initLog();
        
            
        
        
        }


        private static void initLog()
        {

            LogRecord = new Dictionary<LogLevel, Stack<string>>();

            LogRecord.Add(LogLevel.info, new Stack<string>());
            LogRecord.Add(LogLevel.warning, new Stack<string>());
            LogRecord.Add(LogLevel.error, new Stack<string>());

            

        }

        public static void WriteLog(LogLevel level)
        {

            string file = LogDir.FullName + "\\" + Assembly.GetExecutingAssembly().GetName().Name +"."+ level.ToString() + ".log";

            var stack = LogRecord[level];

            lock (LogWriter)
            {

                if (!LogWriter.ContainsKey(level) || !File.Exists(file))
                {
                    LogWriter.Add(level, File.CreateText(file));
                }

            }

            

            var writer = LogWriter[level];

            

            while (stack.Count != 0)
            {
                string data = null;

                try
                {
                    lock (stack)
                    {
                        data = stack.Count > 0 ? stack.Pop():null;
                    }


                    
                    

                }
                catch (Exception e) { }

                if (data != null)
                {
                    lock (writer)
                    {
                        writer.WriteLine(data);
                        writer.Flush();
                    }

                    
                }
            }

            
            


        }


        public static void SetLogFolder(DirectoryInfo dir)
        {
            LogDir = dir;
            dir.Create();




        }


        public static String Info(string str)
        {




            string str2 = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}][线程：{Thread.CurrentThread.ManagedThreadId}]{str}";
            Console.WriteLine(str2);
            LogRecord[LogLevel.info].Push(str2);


            if (LogDir != null && LogRecord[LogLevel.info].Count >= 1000)
            {
                

                WriteLog(LogLevel.info);
                WriteLog(LogLevel.warning);
                WriteLog(LogLevel.error);


            }

            return str2;

            

        }

        public static void flush()
        {
            if (LogDir != null)
            {


                WriteLog(LogLevel.info);
                WriteLog(LogLevel.warning);
                WriteLog(LogLevel.error);


            }
        }

        public static void Error(string str)
        {
            
            LogRecord[LogLevel.error].Push(Info(str));
        }

        public static void Warning(string str)
        {
            LogRecord[LogLevel.warning].Push(Info(str));
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

