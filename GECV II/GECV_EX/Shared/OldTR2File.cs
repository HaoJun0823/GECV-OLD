using GECV_EX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GECV_EX.Shared
{

    [XmlRoot]
    [Serializable]
    public class OldTR2File
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



            [XmlElement]
            public string type;

            [XmlElement]

            public TR2FileBinData bin_data_data;

            [XmlArray]
            public TR2FileDataArray[] data_arr;

            [XmlElement]
            public int LastMark;


        }

        [XmlRoot]
        [Serializable]
        public struct TR2FileDataArray
        {
            [XmlAttribute]
            public bool NotDeprecatedObject;

            [XmlArray]
            public string[] data_arr_arr;

            [XmlArray]
            public string[] data_arr_arr_bin;

        }

        [XmlRoot]
        [Serializable]
        public struct TR2FileBinData
        {
            [XmlElement]

            public byte[] data_30_3F_16;


            [XmlAttribute]
            public bool NotDeprecatedObject;


            [XmlElement]
            public byte data_74;

            [XmlElement]
            public byte data_75;

            [XmlElement]
            public byte data_76_arr_arr_size;

            [XmlElement]
            public byte data_77;

            [XmlElement]
            public int data_78_7B_4;



        }


        [XmlIgnore]
        public byte[] conf_data;




        private OldTR2File() { }


        public OldTR2File(byte[] tr2)
        {

            using (MemoryStream ms = new MemoryStream(tr2))
            {

                using (BinaryReader br = new BinaryReader(ms))
                {


                    header = br.ReadInt32();

                    if (header != TR2_HEADER)
                    {

                        throw new FileLoadException($"This is Not TR2 File!");

                    }

                    magic_1 = br.ReadInt32();

                    long origin_offset = br.BaseStream.Position;


                    header_inf = StreamUtils.readNullterminated(br);
                    Console.WriteLine($"Read Header Information {header_inf}.");

                    br.BaseStream.Seek(origin_offset, SeekOrigin.Begin);

                    br.BaseStream.Seek(48, SeekOrigin.Current);

                    int info_offset = br.ReadInt32();

                    int info_count = br.ReadInt32();
                    Console.WriteLine($"Read Info Count {info_count}.");
                    br.BaseStream.Seek(info_offset, SeekOrigin.Begin);

                    data_info_set = new TR2FileSetInformation[info_count];


                    for (int i = 0; i < data_info_set.Length; i++)
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


                    for (int i = 0; i < data_info_set.Length; i++)
                    {

                        var tr2_fsi = data_info_set[i];

                        br.BaseStream.Seek(tr2_fsi.offset, SeekOrigin.Begin);

                        data_info_set[i].bin_data = br.ReadBytes(tr2_fsi.csize);

                        string[] editor = FileUtils.GetHexEditorStyleString(data_info_set[i].bin_data);

                        foreach (var ed in editor)
                        {
                            Console.WriteLine(ed);
                        }

                        using (MemoryStream ms2 = new MemoryStream(data_info_set[i].bin_data))
                        {
                            using (BinaryReader br2 = new BinaryReader(ms2))
                            {

                                Console.WriteLine($"Deep Current Cursor i:{br2.BaseStream.Position.ToString("X")}");
                                data_info_set[i].name = StreamUtils.readNullterminated(br2);

                                Console.WriteLine($"Deep Data Name:{data_info_set[i].name}.");



                                br2.BaseStream.Seek(0x40, SeekOrigin.Begin);
                                data_info_set[i].type = StreamUtils.readNullterminated(br2);
                                Console.WriteLine($"Deep Data Type:{data_info_set[i].type}.");


                                if (true)
                                {  //Bin Data Very Hard Beacause We Cannot Know Everything Data.

                                    data_info_set[i].bin_data_data = new TR2FileBinData();

                                    br2.BaseStream.Seek(0x30, SeekOrigin.Begin);

                                    data_info_set[i].bin_data_data.data_30_3F_16 = br2.ReadBytes(16);



                                    br2.BaseStream.Seek(0x74, SeekOrigin.Begin);

                                    data_info_set[i].bin_data_data.data_74 = br2.ReadByte();

                                    br2.BaseStream.Seek(0x75, SeekOrigin.Begin);

                                    data_info_set[i].bin_data_data.data_75 = br2.ReadByte();

                                    br2.BaseStream.Seek(0x77, SeekOrigin.Begin);

                                    data_info_set[i].bin_data_data.data_77 = br2.ReadByte();

                                    br2.BaseStream.Seek(0x76, SeekOrigin.Begin);

                                    data_info_set[i].bin_data_data.data_76_arr_arr_size = br2.ReadByte();
                                    Console.WriteLine($"Deep Data Arr Arr (Byte):{data_info_set[i].bin_data_data.data_76_arr_arr_size}.");

                                    br2.BaseStream.Seek(0x78, SeekOrigin.Begin);

                                    data_info_set[i].bin_data_data.data_78_7B_4 = br2.ReadInt32();


                                    data_info_set[i].data_arr = new TR2FileDataArray[br2.ReadInt32()]; //7C-7F
                                    Console.WriteLine($"Deep Data Arr :{data_info_set[i].data_arr.Length}.");
                                    Console.WriteLine($"Deep Current Cursor i:{br2.BaseStream.Position.ToString("X")}");

                                    //if (data_info_set[i].type != "ASCII" || data_info_set[i].type != "UTF-16" || data_info_set[i].type != "UTF-8")
                                    //{
                                    //    Console.WriteLine($"{data_info_set[i].type} Is Not Implemented!");
                                    //    continue;
                                    //}


                                    switch (data_info_set[i].type)
                                    {
                                        case "ASCII":
                                            break;
                                        case "UTF-16":
                                            break;
                                        case "UTF-16LE":
                                            break;
                                        case "UTF-8":
                                            break;
                                        default:
                                            Console.WriteLine($"{data_info_set[i].type} Is Not Implemented!");
                                            continue;
                                    }


                                    for (int si = 0; si < data_info_set[i].data_arr.Length; si++)
                                    {
                                        data_info_set[i].data_arr[si] = new TR2FileDataArray();
                                        data_info_set[i].data_arr[si].data_arr_arr = new string[data_info_set[i].bin_data_data.data_76_arr_arr_size];
                                        data_info_set[i].data_arr[si].data_arr_arr_bin = new string[data_info_set[i].bin_data_data.data_76_arr_arr_size];
                                        Console.WriteLine($"Deep Current Cursor si:{br2.BaseStream.Position.ToString("X")}");

                                        int arr_data_offset = br2.ReadInt32();
                                        long next_data_offset = br2.BaseStream.Position;
                                        Console.WriteLine($"Deep Next Cursor:{br2.BaseStream.Position.ToString("X")}");
                                        br2.BaseStream.Seek(arr_data_offset, SeekOrigin.Begin);

                                        for (int ssi = 0; ssi < data_info_set[i].data_arr[si].data_arr_arr.Length; ssi++)
                                        {

                                            Console.WriteLine($"Deep Current Cursor ssi:{br2.BaseStream.Position.ToString("X")}");


                                            int readbyte_count = ReadCountByTr2Type(data_info_set[i].type);
                                            Console.WriteLine($"Deep Data Read Size:{data_info_set[i].type}:{readbyte_count}.");
                                            byte[] arr_data;

                                            Console.WriteLine($"Deep Current Cursor Read Arr Arr:{br2.BaseStream.Position.ToString("X")}");
                                            if(readbyte_count == -2 ) {
                                                arr_data = StreamUtils.readWideDataterminated(br2);
                                                Console.WriteLine($"UTF-16 Get Length:{arr_data.Length}");
                                            }
                                            else

                                            if (readbyte_count == -1)
                                            {
                                                arr_data = StreamUtils.readZeroterminated(br2);
                                            }
                                            else
                                            {

                                                if (readbyte_count == 1)
                                                {
                                                    arr_data = new byte[1];
                                                    arr_data[0] = br2.ReadByte();
                                                }
                                                else
                                                {
                                                    arr_data = br2.ReadBytes(readbyte_count);
                                                }
                                            }


                                            Console.WriteLine($"Deep Data Read Result:Length:{arr_data.Length},Data:{FileUtils.GetByteArrayString(arr_data)},OriginHex:{FileUtils.GetByteArrayString(arr_data)}");


                                            data_info_set[i].data_arr[si].data_arr_arr[ssi] = BuildBinaryDataByTr2Type(data_info_set[i].type, arr_data);
                                            data_info_set[i].data_arr[si].data_arr_arr_bin[ssi] = FileUtils.GetByteArrayString(arr_data);

                                            Console.WriteLine($"Byte[] To {data_info_set[i].type}:{data_info_set[i].data_arr[si].data_arr_arr[ssi]}");

                                            //br2.BaseStream.Seek(arr_data_offset,SeekOrigin.Begin);

                                        }


                                        br2.BaseStream.Seek(next_data_offset, SeekOrigin.Begin);




                                    }
                                    Console.WriteLine($"Last Mark Poistion:{br2.BaseStream.Position.ToString("X")}");
                                    data_info_set[i].LastMark = br2.ReadInt32();

                                }
                            }
                        }


                    }




                };


            }







        }


        //ASCII
        //UTF-8
        //UTF-16
        //UINT8
        //UINT16
        //UINT32
        //FLOAT32
        //INT8
        //INT16
        //INT32

        public static string BuildBinaryDataByTr2Type(string type, byte[] data)
        {

            switch (type)
            {
                case "ASCII":
                    return Encoding.ASCII.GetString(data);
                case "UTF-16LE":
                    return Encoding.Unicode.GetString(data);
                case "UTF-8":
                    return Encoding.UTF8.GetString(data);
                case "UTF-16":
                    return Encoding.Unicode.GetString(data); //Encoding.BigEndianUnicode //?
                case "INT8":
                    //return Convert.ToSByte(data).ToString();
                    return ((sbyte)data[0]).ToString();

                case "UINT8":
                    //return Convert.ToByte(data).ToString();
                    return (data[0]).ToString();
                case "INT16":
                    //return Convert.ToInt16(data).ToString();
                    return BitConverter.ToInt16(data).ToString();
                case "UINT16":
                    //return Convert.ToUInt16(data).ToString();
                    return BitConverter.ToUInt16(data).ToString();
                case "INT32":
                    //return Convert.ToInt32(data).ToString();
                    return BitConverter.ToInt32(data).ToString();
                case "UINT32":
                    //return Convert.ToUInt32(data).ToString();
                    return BitConverter.ToUInt32(data).ToString();
                case "FLOAT32":
                    //return Convert.ToSingle(data).ToString();
                    return BitConverter.ToSingle(data).ToString();
                default:
                    throw new InvalidCastException($"What is {type}?");
            }


        }

        public static int ReadCountByTr2Type(string type)
        {
            switch (type)
            {
                case "ASCII":
                    return -1;
                case "UTF-16LE":
                    return -2;
                case "UTF-8":
                    return -1;
                case "UTF-16":
                    return -2;
                case "INT8":
                    return 1;
                case "UINT8":
                    return 1;
                case "INT16":
                    return 2;
                case "UINT16":
                    return 2;
                case "INT32":
                    return 4;
                case "UINT32":
                    return 4;
                case "FLOAT32":
                    return 4;
                default:
                    throw new InvalidCastException($"What is {type}?");
            }
        }


        public string SaveAsXml()
        {

            return XmlUtils.Save<OldTR2File>(this);

        }


    }
}
