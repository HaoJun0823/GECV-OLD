using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GECV_EX.Shared
{
    [XmlRoot]
    [Serializable]
    public class GnfFile
    {

        public static readonly int DDS_MAGIC = 0x20534444;

        public List<byte[]> dds_data;



        public GnfFile(byte[] data)
        {
            

            using(MemoryStream ms = new MemoryStream())
            {
                using(BinaryReader br = new BinaryReader(ms))
                {

                    int count = br.ReadInt32();

                    int[] file_size_group = new int[count];

                    for(int i = 0; i < count; i++)
                    {

                        file_size_group[i] = br.ReadInt32();
                        




                    }

                    dds_data = new List<byte[]>();

                    for(int i = 0;i < file_size_group.Length; i++)
                    {

                        var data_file = br.ReadBytes(file_size_group[i]);


                        using (MemoryStream file_ms = new MemoryStream(data_file))
                        {
                            using (BinaryReader reader = new BinaryReader(file_ms))
                            {
                                int dds_magic = reader.ReadInt32();

                                if (dds_magic != DDS_MAGIC)
                                {
                                    throw new FileLoadException($"Gnf Error:{i} Is not dds file.");
                                }
                                else
                                {
                                    dds_data[i] = data_file;
                                }

                            }
                        }

                    }



                }
            }



        }

        

    }
}
