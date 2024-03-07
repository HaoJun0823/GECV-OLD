using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GECV_EX.Utils
{
    public static class FileUtils
    {


        public static string GetOrderName(int order,string name)
        {

            return order.ToString().PadLeft(8, '0')+'_'+name;

        }


        

        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }


        public static void CreateSymbolLinkDosCommandFile(Dictionary<string,string> map,string root_dir)
        {

            HashSet<string> list = new HashSet<string>();


            list.Add("@echo off");
            list.Add("cd %~dp0");

            list.Add("rd .\\Symbol-Link /s /q");

            list.Add("mkdir .\\Symbol-Link");

            //List<string> dir_list = new List<string>();

            foreach(var kv in map)
            {
                string origin = ".\\"+Path.GetRelativePath(root_dir,kv.Key);

                if (String.IsNullOrEmpty(kv.Value))
                {
                    continue;
                }

                string target = ".\\Symbol-Link\\" + kv.Value;


                string target_dir = Path.GetDirectoryName(target);
                list.Add($"mkdir {target_dir}");
                //if (!dir_list.Contains(target_dir))
                //{
                //    list.Add($"mkdir {target_dir}");
                //    dir_list.Add(target_dir);
                //}



                list.Add($"echo {origin}={target}");

                list.Add($"mklink \"{target}\" \"{origin}\"");

            }
            list.Add("pause");

            File.WriteAllLines(root_dir+"\\mklink.bat",list);

            
        }

        


    }
}
