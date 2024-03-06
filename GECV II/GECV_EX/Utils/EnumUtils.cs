using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECV_EX.Utils
{
    public static class EnumUtils
    {



        public static string GetEnumStringName(Type type,int number)
        {


            return Enum.GetName(type, number);

        }


    }
}
