using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECV.TINY.QPCK
{
    public class QPCK
    {

        private const int DEFAULT_MAGIC = 0x37402858;

        public int Magic { get { return Magic; } private set { Magic = value; } }

        public int Count;

        private IList<DataIndex> DataList;


        private struct DataIndex
        {
            public long Offset;
            public long Hash;
            public int Size;
        }


        private QPCK(){}

        public QPCK(String Path)
        {
           this.DataList = new List<DataIndex>();

        }
        




    }
}
