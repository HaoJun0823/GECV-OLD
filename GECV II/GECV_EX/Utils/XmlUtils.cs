using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GECV_EX.Utils
{
    public static class XmlUtils
    {

        public static string Save<T>(T obj)
        {

            using(StringWriter  sw = new StringWriter())
            {

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                xmlSerializer.Serialize(sw,obj);
                return sw.ToString();
            }

        }

        public static T Load<T>(StreamReader sr)
        {

            XmlSerializer xmlSerializer = new XmlSerializer (typeof(T));

             return (T)xmlSerializer.Deserialize(sr);


        }

        public static T Load<T>(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    return  XmlUtils.Load<T>(sr);
                }

            }
        }

    }
}
