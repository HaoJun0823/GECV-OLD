using GECV_EX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECV_EX.PC
{
    public class PresFileData
    {

        public int original_offset_file;
        public int real_offset_file;
        public int csize_file;
        public int name_off_file;
        public int name_elements_file;
        public int file_unk1; 
        public int file_unk2;
        public int file_unk3;
        public int usize_file; //uncompressd

        public string[] name_list;

        public byte[] file_data;



        public bool IsVirtualFile;
        public bool IsCompressed;



        public PresFileData(BinaryReader br)
        {

            original_offset_file = br.ReadInt32();
            csize_file = br.ReadInt32();
            name_off_file = br.ReadInt32();
            name_elements_file = br.ReadInt32();
            file_unk1 = br.ReadInt32();
            file_unk2 = br.ReadInt32();
            file_unk3 = br.ReadInt32();
            usize_file = br.ReadInt32();

            if (csize_file == usize_file)
            {
                this.IsCompressed = false;
            }
            else
            {
                this.IsCompressed = true;
            }

            br.BaseStream.Seek(name_off_file,SeekOrigin.Begin);

            int[] name_element_address = new int[name_elements_file];
            name_list = new string[name_elements_file];

            for(int i = 0; i < name_element_address.Length; i++)
            {

                name_element_address[i] = br.ReadInt32();
            }

            
            for(int i = 0;i < name_element_address.Length; i++)
            {
                br.BaseStream.Seek(name_element_address[i],SeekOrigin.Begin);

                name_list[i] = StreamUtils.readNullterminated(br);
            }

            GetRealOffset();

            br.BaseStream.Seek(this.real_offset_file, SeekOrigin.Begin);

            this.file_data = br.ReadBytes(csize_file);


        }


        private void GetRealOffset()
        {

            int temp = original_offset_file;
            string hex_temp = temp.ToString("X8");
            real_offset_file = Convert.ToInt32(hex_temp.Substring(1),16);


            this.IsVirtualFile = hex_temp[0].Equals('B') ? true:false ;
            


        }

        


    }
}
