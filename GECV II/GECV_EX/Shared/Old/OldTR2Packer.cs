using GECV_EX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECV_EX.Shared.Old
{
    public class OldTR2Packer
    {


        BinaryBooker booker = new BinaryBooker();


        private OldTR2File tr2_file_info;

        private DirectoryInfo data_folder;


        public OldTR2Packer(string xml_path)
        {

            tr2_file_info = XmlUtils.Load<OldTR2File>(xml_path);


            data_folder = new DirectoryInfo(Path.GetDirectoryName(xml_path));


            FileInfo[] bin_files = data_folder.GetFiles("*.bin", SearchOption.TopDirectoryOnly);

            string conf_data_name = data_folder.FullName + "\\conf.dat";
            if (!File.Exists(conf_data_name))
            {
                throw new FileNotFoundException(conf_data_name);
            }

            tr2_file_info.conf_data = File.ReadAllBytes(conf_data_name);



            if (bin_files.Length != tr2_file_info.data_info_set.Length)
            {

                throw new InvalidDataException($"{xml_path} Count Is :{tr2_file_info.data_info_set.Length},But Only Exist {bin_files.Length} Files.");
            }


            for (int i = 0; i < bin_files.Length; i++)
            {
                var bin_file = bin_files[i];

                tr2_file_info.data_info_set[i].bin_data = File.ReadAllBytes(bin_file.FullName);
                tr2_file_info.data_info_set[i].csize = (int)bin_file.Length;
                tr2_file_info.data_info_set[i].usize = (int)bin_file.Length;

            }


        }


        public byte[] SaveAsTR2()
        {
            int offset = 0;
            booker.SetBookMark("header", offset);
            offset += 4;
            booker.SetBookMark("magic_1", offset);
            booker.WriteData("header", tr2_file_info.header);
            booker.WriteData("magic_1", tr2_file_info.magic_1);
            offset += 4;

            byte[] header_inf_byte = Encoding.UTF8.GetBytes(tr2_file_info.header_inf);

            if (header_inf_byte.Length > 48)
            {

                throw new OverflowException($"{tr2_file_info.header_inf} bytes over 48 length! Current{header_inf_byte.Length}.");

            }

            Array.Resize(ref header_inf_byte, 48);

            booker.SetBookMark("header_inf", offset);
            booker.WriteData("header_inf", header_inf_byte);
            offset += header_inf_byte.Length;

            booker.SetBookMark("info_offset", offset); // after
            offset += 4;
            booker.SetBookMark("info_count", offset);
            booker.WriteData("info_count", tr2_file_info.data_info_set.Length);
            offset += 4;

            booker.WriteData("info_offset", offset);
            for (int i = 0; i < tr2_file_info.data_info_set.Length; i++)
            {
                var data_inf = tr2_file_info.data_info_set[i];

                booker.SetBookMark($"data_info_set_id_{i}", offset);
                offset += 4;
                booker.SetBookMark($"data_info_set_offset_{i}", offset);
                offset += 4;
                booker.SetBookMark($"data_info_set_magic_{i}", offset);
                offset += 4;
                booker.SetBookMark($"data_info_set_csize_{i}", offset);
                offset += 4;
                booker.SetBookMark($"data_info_set_usize_{i}", offset);
                offset += 4;


                booker.WriteData($"data_info_set_id_{i}", data_inf.id);
                booker.WriteData($"data_info_set_magic_{i}", data_inf.magic);
                booker.WriteData($"data_info_set_csize_{i}", data_inf.csize);
                booker.WriteData($"data_info_set_usize_{i}", data_inf.usize);




            }

            if (tr2_file_info.conf_data.Length % 4 != 0)
            {
                throw new InvalidDataException($"conf data length error, Length:{tr2_file_info.conf_data.Length} % 4 != 0 !");
            }

            int conf_length = tr2_file_info.conf_data.Length / 4;

            booker.SetBookMark("conf_data_length", offset);
            offset += 4;
            booker.WriteData("conf_data_length", conf_length);

            booker.SetBookMark("conf_data_file", offset);
            booker.WriteData("conf_data_file", tr2_file_info.conf_data);
            offset += tr2_file_info.conf_data.Length;


            if (offset % 16 != 0)
            {

                booker.SetBookMark("conf_data_zero_16", offset);

                int offset_zero16 = offset % 16;

                booker.WriteData("conf_data_zero_16", new byte[offset_zero16]);

                offset += offset_zero16;

            }

            for (int i = 0; i < tr2_file_info.data_info_set.Length; i++)
            {
                var data = tr2_file_info.data_info_set[i];

                booker.SetBookMark("file_data_" + i, offset);
                booker.WriteData("file_data_" + i, data.bin_data);

                booker.WriteData($"data_info_set_offset_{i}", offset);

                offset += data.bin_data.Length;


                booker.SetBookMark("file_data_zero_16_" + i, offset);

                int offset_zero16 = offset % 16;

                booker.WriteData("file_data_zero_16_" + i, new byte[offset_zero16]);

                offset += offset_zero16;

            }




            return booker.GetAllData();
        }

        public List<string> GetBookInformation()
        {
            return booker.GetBookInformation();
        }


    }
}
