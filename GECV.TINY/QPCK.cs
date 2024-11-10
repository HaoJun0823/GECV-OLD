using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECV.TINY
{
    public class QPCK
    {

        public const int DEFAULT_MAGIC = 0x37402858;

        public int magic;

        public int count => data.Count;

        public IList<DataIndex> data;


        public struct DataIndex
        {
            public long offset;
            public long hash;
            public int size;
        }

        public QPCK()
        {
            this.data = new List<DataIndex>();
        }


    }
}
