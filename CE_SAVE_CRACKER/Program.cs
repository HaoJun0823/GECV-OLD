using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE_SAVE_CRACKER
{
    internal class Program
    {
        static void Main(string[] args)
        {

            if (args.Length < 4)
            {
                System.Console.WriteLine("BY Randerion(HaoJun0823)\nUseage:1.Input save file.\n2.Output save file.\n3.Part 1 Key.\n4.Part 2 Key.");
            }


            uint part1Key = Convert.ToUInt32(args[2], 16);
            uint part2Key = Convert.ToUInt32(args[3], 16);


            // 从文件加载加密数据
            byte[] encrypted = File.ReadAllBytes(args[0]);
            // 使用正确的密钥初始化（这些值必须提供）
            //var decryptor = new CustomDecryptor(
            //    keyPart1: 0xA282BE88,  // 替换为实际的key1
            //    keyPart2: 0x1967C285   // 替换为实际的key2
            //);

            var decryptor = new CustomDecryptor(
                keyPart1: part1Key,  // 替换为实际的key1
                keyPart2: part2Key   // 替换为实际的key2
            );

            // 执行解密
            byte[] decrypted = decryptor.Decrypt(encrypted);
            // 保存解密结果
            File.Delete(args[1]);
            File.WriteAllBytes(args[1], decrypted);

        }
    }
}
