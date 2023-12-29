using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GECV
{
    public static class Define
    {

        /*
         * QPCK
         * INT 四字节魔术码 = 0x37402858
         * INT 文件总数，文件索引数量=文件总数数量=文件数据数量
         * {文件索引} [八字节偏移，八字节大小，四字节哈希]
         * {文件数据} 
         */
        public static readonly int QPCK_MAGIC = 0x37402858;
        public static readonly int PRES_MAGIC = 0x73657250;
        public static readonly int BLZ4_MAGIC = 0x347a6c62;
        

        public static string GetExtension(int magic)
        {
            Dictionary<int, string> extension_dic = new Dictionary<int, string>
            {
                { 0x46534e42, ".bnsf" },
                { 0x6c566d47, ".gmvl" },
                { 0x3272742e, ".tr2" },
                { PRES_MAGIC, ".pres" },
                { BLZ4_MAGIC, ".blz4" },
            };

            string extension_str;
            if (!extension_dic.TryGetValue(magic, out extension_str)) { extension_str = ".bin"; }

            return extension_str;
        }


    }
}
