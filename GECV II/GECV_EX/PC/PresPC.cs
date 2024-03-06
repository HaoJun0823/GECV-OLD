using GECV_EX.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECV_EX.PC
{




    internal class PresPC
    {




        public static readonly int PRES_MAGIC = 0x73657250;

        private int magic_0; // vaild this = PRES_MAGIC
        private int magic_1;
        private int magic_2;
        private int magic_3;

        private int offset_data; //HeaderBinaryLength

        private long zerozero;



        private PresCountry[] countries;








        public PresPC(byte[] pres_data)
        {

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryReader br = new BinaryReader(ms))
                {

                    magic_0 = br.ReadInt32();

                    if (magic_0 != PRES_MAGIC)
                    {
                        throw new FileLoadException($"Pres Header Error:{magic_0.ToString("X8")}!={PRES_MAGIC.ToString("X8")}.");
                    }

                    magic_1 = br.ReadInt32();
                    magic_2 = br.ReadInt32();
                    magic_3 = br.ReadInt32();

                    offset_data = br.ReadInt32();
                    zerozero = br.ReadInt64();

                    var count_set = br.ReadInt32();

                    if (count_set <= 0)
                    {
                        throw new FileLoadException($"Pres Count Set Error:{count_set}<=0.");
                    }
                    else
                    {
                        countries = new PresCountry[count_set];
                    }
                    
                    if(count_set > 1)
                    {

                        for (int i = 0; i < countries.Length; i++)
                        {
                            countries[i] = new PresCountry();
                            countries[i].offset = br.ReadInt32();
                            countries[i].length = br.ReadInt32();

                        }
                    }
                    else
                    {
                        countries[0] = new PresCountry();
                        countries[0].offset = (int)br.BaseStream.Position;
                        countries[0].length = 0;
                    }

                    
                    for(int i = 0;i < countries.Length;i++)
                    {
                        PresSet[] pres_set = new PresSet[8];    
                        BuildPresSet(pres_data, br.BaseStream.Position, ref pres_set);
                        
                    }







                }




            }





        }

        private void BuildPresSet(byte[] pres_data,long reader_cursor,ref PresSet[] pres_data_set)
        {

            using(MemoryStream ms = new MemoryStream(pres_data))
            {
                using(BinaryReader br =new BinaryReader(ms))
                {

                    br.BaseStream.Seek(reader_cursor, SeekOrigin.Begin);

                    for (int i = 0; i < pres_data_set.Length; i++)
                    {


                        PresSet ps = new PresSet();
                        ps.offset = br.ReadInt32();
                        int count = br.ReadInt32();
                        

                        switch (i)
                        {
                            case 6:
                                ps.mul = br.ReadInt32();
                                break;
                            default:
                                break;
                        }
                        pres_data_set[i] = ps;

                    }
                    


                }
            }


        }


        public bool IsNoCountryPres()
        {


            return countries.Length <= 1 ? true : false;


        }










    }
}

