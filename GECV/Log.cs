﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GECV
{
    public class Log
    {



        public static void Info(string str)
        {
            Console.WriteLine(str);
        }



        public static void Error(string str)
        {
            Console.Error.WriteLine(str);
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
