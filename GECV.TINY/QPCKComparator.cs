using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GECV.TINY
{
    public static class QPCKComparator
    {



        public static void ExtractAllTargetDifferentData(QPCKExtracter origin,QPCKExtracter target,String directorayPath)
        {

            if (origin.count != target.count) {

                throw new ArgumentException("Sorry GECV Tiny version cannot be compared with these QPCK files beacause they have different numbers of files.");
            
            }

            for (int i = 0; i < origin.count; i++)
            {
                byte[] dataA = origin[i];
                byte[] dataB = target[i];
                Console.Write($"Process:{i}/{origin.count}");
                if (dataA.SequenceEqual(dataB)) {
                    Console.WriteLine($"|N");

                    continue;
                    
                }
                else
                {
                    Console.WriteLine($"|Y");
                    target.Extract(directorayPath,i);
                }
            
            
            }


        }


        

    }
}
