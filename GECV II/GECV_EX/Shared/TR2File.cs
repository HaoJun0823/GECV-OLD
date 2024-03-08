using GECV_EX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GECV_EX.Shared
{

    [XmlRoot]
    [Serializable]
    public class TR2File
    {

        [XmlIgnore]
        public static readonly int TR2_HEADER = 0x3272742E;


        [XmlAttribute]
        public int header;

        [XmlAttribute]
        public int magic_1; //0x07DF0002

        [XmlElement]
        public string header_inf; //byte[48]

        //[XmlIgnore]
        //public int info_offset;

        

        [XmlArray]
        public TR2FileSetInformation[] data_info_set;


        [XmlRoot]
        [Serializable]
        public struct TR2FileSetInformation
        {

            [XmlElement]
            public int id;
            [XmlIgnore]
            public int offset; //Running Data
            [XmlElement]
            public int magic;
            [XmlIgnore]
            public int csize;
            [XmlIgnore]
            public int usize;

            [XmlIgnore]
            public byte[] bin_data;


            [XmlElement]
            public string name;


        }

        [XmlIgnore]
        public byte[] conf_data;




        private TR2File() { }


        public TR2File(byte[] tr2)
        {

            using(MemoryStream ms = new MemoryStream(tr2))
            {

                using( BinaryReader br = new BinaryReader(ms))
                {


                    header = br.ReadInt32();

                    if(header != TR2_HEADER) { 
                    
                        throw new FileLoadException($"This is Not TR2 File!");
                    
                    }

                    magic_1 = br.ReadInt32();

                    long origin_offset = br.BaseStream.Position;


                    header_inf =  StreamUtils.readNullterminated( br );
                    Console.WriteLine($"Read Header Information {header_inf}.");

                    br.BaseStream.Seek( origin_offset, SeekOrigin.Begin );

                    br.BaseStream.Seek(48,SeekOrigin.Current);

                    int info_offset = br.ReadInt32();

                    int info_count = br.ReadInt32();
                    Console.WriteLine($"Read Info Count {info_count}.");
                    br.BaseStream.Seek(info_offset, SeekOrigin.Begin );

                    data_info_set = new TR2FileSetInformation[info_count];
                

                    for(int i=0; i< data_info_set.Length; i++)
                    {

                        var tr2_fsi = new TR2FileSetInformation();

                        tr2_fsi.id = br.ReadInt32();
                        tr2_fsi.offset = br.ReadInt32();
                        tr2_fsi.magic = br.ReadInt32();
                        tr2_fsi.csize = br.ReadInt32();
                        tr2_fsi.usize = br.ReadInt32();

                        data_info_set[i] = tr2_fsi;

                        Console.WriteLine($"TS2 DATA INFORMATION:\nID:{tr2_fsi.id}\noffset:{tr2_fsi.offset}\nmagic:{tr2_fsi.magic}\ncsize:{tr2_fsi.csize}\nusize:{tr2_fsi.usize}");
                        Console.WriteLine($"Current Cursor:{br.BaseStream.Position.ToString("X")}");
                    }

                    

                    int conf_length = br.ReadInt32() * 4;

                    conf_data = br.ReadBytes(conf_length);


                    for(int i=0; i< data_info_set.Length; i++)
                    {

                        var tr2_fsi = data_info_set[i];

                        br.BaseStream.Seek(tr2_fsi.offset,SeekOrigin.Begin);

                        data_info_set[i].bin_data = br.ReadBytes(tr2_fsi.csize);

                        using(MemoryStream ms2 = new MemoryStream(data_info_set[i].bin_data))
                        {
                            using(BinaryReader br2 = new BinaryReader(ms2)) {


                                data_info_set[i].name = StreamUtils.readNullterminated(br2);
                            
                            
                            }
                        }


                    }




                };


            }





            

        }





        public string SaveAsXml()
        {

            return XmlUtils.Save<TR2File>(this);

        }


    }
}
