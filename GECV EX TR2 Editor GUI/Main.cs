using GECV_EX.TR2;
using GECV_EX.Utils;
using MiniExcelLibs;
using System.Data;
using System.Text;

namespace GECV_EX_TR2_Editor_GUI
{
    public partial class Main : Form
    {

        public static DataTable System_DataTable;
        public static DataTable System_DataTable_Hex;
        public static TR2Reader System_TR2;

        public static string original_title;

        public static string input_file_name;

        public Main()
        {
            InitializeComponent();
            original_title = this.Text;
            SetMenuStatus(false);
        }

        private void MenuItem_Open_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog ofd = new OpenFileDialog())
            {

                ofd.RestoreDirectory = true;
                ofd.Multiselect = false;
                ofd.Title = "Open Tr2 Or Xml File:";
                ofd.Filter = "xml (*.xml)|*.xml|tr2 (*.tr2)|*.tr2";
                ofd.RestoreDirectory = false;


                if (ofd.ShowDialog() == DialogResult.OK)
                {


                    string select_file = ofd.FileName;


                    string ext = Path.GetExtension(select_file);

                    input_file_name = Path.GetFileNameWithoutExtension(select_file);

                    try
                    {


                        if (ext.ToLower() == ".xml")
                        {



                            System_TR2 = XmlUtils.Load<TR2Reader>(select_file);
                            this.Text = original_title + $" Building {System_TR2.table_name} Table, Please Wait...";
                            BuildDataTable(System_TR2);
                            RefreshDataTable();

                            this.Text = original_title + " " + select_file;
                            SetMenuStatus(true);
                            return;
                        }
                        else

                        if (ext.ToLower() == ".tr2")
                        {

                            System_TR2 = new TR2Reader(File.ReadAllBytes(select_file));
                            this.Text = original_title + $" Building {System_TR2.table_name} Table, Please Wait...";
                            BuildDataTable(System_TR2);
                            RefreshDataTable();
                            this.Text = original_title + " " + select_file;
                            SetMenuStatus(true);
                            return;
                        }



                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{select_file} Open Error:\n{ex.Message}\n{ex.StackTrace}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        input_file_name = "";
                        this.Text = original_title;
                        SetMenuStatus(false);
                        return;
                    }
                    MessageBox.Show($"{select_file} Is not supported file!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);


                }



            }


        }



        private void SetMenuStatus(bool status)
        {

            this.MenuItem_Export.Enabled = status;
            this.MenuItem_Import.Enabled = status;
            this.MenuItem_Save.Enabled = status;
            this.MenuItem_Excel_Export.Enabled = status;

        }


        private void BuildDataTable(TR2Reader tr2data)
        {
            DataTable dt = new DataTable();

            dt.TableName = tr2data.table_name;

            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Index", typeof(byte));
            //dt.Columns.Add("Editor Mode", typeof(EditorDataModeEnum));





            //DataColumn row_col = new DataColumn("Row ID");
            //dt.Columns.Add(row_col);

            //for (int i = 0; i < tr2data.table_column_infromation.Length; i++)
            //{
            //    var tr2inf = tr2data.table_column_infromation[i];
            //    DataColumn d_col = new DataColumn(tr2inf.GetTableNameForEditor());
            //    dt.Columns.Add(d_col);


            //}


            //for(int i = 0; i < tr2data.column_counter.id.Length; i++)
            //{

            //    DataRow dr =dt.NewRow();








            //}

            foreach (DataColumn dc in dt.Columns)
            {

                //if (dc.ColumnName == "Editor Mode")
                //{
                //    dc.ReadOnly = false;
                //}
                //else
                //{
                //    dc.ReadOnly = true;
                //}

                dc.ReadOnly = true;



            }



            for (int i = 0; i < tr2data.column_counter.id.Length; i++)
            {

                DataColumn id_col = new DataColumn(tr2data.column_counter.id[i].ToString(), typeof(string));


                dt.Columns.Add(id_col);

            }

            DataTable dt_hex = dt.Clone();


            for (int i = 0; i < tr2data.table_column_infromation.Length; i++)
            {
                var tr2data_inf = tr2data.table_column_infromation[i];


                for (int si = 0; si < tr2data_inf.column_data.data_76_array_size; si++)
                {

                    var data_arr = tr2data_inf.column_data.column_data_list[si];
                    DataRow dr = dt.NewRow();
                    dr["Id"] = tr2data_inf.id;
                    dr["Name"] = tr2data_inf.column_data.column_name;
                    dr["Type"] = tr2data_inf.column_data.column_type;
                    dr["Index"] = si;

                    DataRow dr_hex = dt_hex.NewRow();
                    dr_hex["Id"] = tr2data_inf.id;
                    dr_hex["Name"] = tr2data_inf.column_data.column_name;
                    dr_hex["Type"] = tr2data_inf.column_data.column_type;
                    dr_hex["Index"] = si;

                    //StringBuilder sb = new StringBuilder();
                    for (int ssi = 0; ssi < tr2data.column_counter.id.Length; ssi++)
                    {

                        if (tr2data_inf.column_data.column_data_list[ssi].IsInVaildOffset || tr2data_inf.column_data.column_data_list[ssi].column_data[si].IsInVaildArrayOffset)
                        {
                            dr[tr2data.column_counter.id[ssi].ToString()] = "[[GECV-EDITOR::NULL]]";
                            dr_hex[tr2data.column_counter.id[ssi].ToString()] = "[[GECV-EDITOR::NULL]]";
                        }
                        else
                        {


                            var data_arr_data = tr2data_inf.column_data.column_data_list[ssi].column_data[si];

                            //sb.Append(ssi);
                            //sb.Append(":{{");
                            //sb.Append(data_arr_data.value_string_view.ToString());
                            //sb.Append("}}");






                            dr[tr2data.column_counter.id[ssi].ToString()] = data_arr_data.value_string_view.ToString();
                            dr_hex[tr2data.column_counter.id[ssi].ToString()] = data_arr_data.value_hex_view.ToString();



                            //dr["Editor Mode"] = GetModeByDataType(dr["Type"].ToString());

                        }


                    }

                    dt.Rows.Add(dr);
                    dt_hex.Rows.Add(dr_hex);


                }






            }







            System_DataTable = dt;
            System_DataTable_Hex = dt_hex;
        }

        //private static EditorDataModeEnum GetModeByDataType(string type)
        //{

        //    switch (type)
        //    {
        //        case "ASCII":
        //            return EditorDataModeEnum.STRING;
        //        case "UTF-16LE":
        //            return EditorDataModeEnum.HEX;
        //        case "UTF-8":
        //            return EditorDataModeEnum.STRING;
        //        case "UTF-16":
        //            return EditorDataModeEnum.HEX;
        //        case "INT8":
        //            return EditorDataModeEnum.STRING;
        //        case "UINT8":
        //            return EditorDataModeEnum.STRING;
        //        case "INT16":
        //            return EditorDataModeEnum.STRING;
        //        case "UINT16":
        //            return EditorDataModeEnum.STRING;
        //        case "INT32":
        //            return EditorDataModeEnum.STRING;
        //        case "UINT32":
        //            return EditorDataModeEnum.STRING;
        //        case "FLOAT32":
        //            return EditorDataModeEnum.STRING;
        //        default:
        //            throw new InvalidCastException($"What is {type}?");
        //    }


        //}

        private void RefreshDataTable()
        {
            this.DataGridView_Main.DataSource = System_DataTable;
            this.DataGridView_Main.TopLeftHeaderCell.Value = System_DataTable.TableName;


            for (int i = 0; i < this.DataGridView_Main.Rows.Count; i++)
            {

                var current_type_cell = this.DataGridView_Main.Rows[i].Cells[2];

                if (current_type_cell.Value.ToString().Equals("UTF-16LE") || current_type_cell.Value.ToString().Equals("UTF-16"))
                {

                    foreach (DataGridViewCell cell in this.DataGridView_Main.Rows[i].Cells)
                    {
                        cell.ReadOnly = true;
                    }





                }


            }



            //foreach(var i in  this.DataGridView_Main.Columns) {




            //}


            this.DataGridView_Main.Refresh();
        }

        private void DataGridView_Main_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            e.Column.FillWeight = 10;
        }

        private void DataGridView_Main_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            int row = e.RowIndex;
            int col = e.ColumnIndex;



            if (col <= 3 || row < 0)
            {
                return;
            }
            //MessageBox.Show($"{row},{col}");


            var current_type_cell = this.DataGridView_Main.Rows[row].Cells[2];

            if (current_type_cell.Value.ToString().Equals("UTF-16LE") || current_type_cell.Value.ToString().Equals("UTF-16"))
            {


                var editor_dialog = new Form_HexEditor(row, col, current_type_cell.Value.ToString(), true);

                editor_dialog.ShowDialog();



            }
            else
            if (current_type_cell.Value.ToString().Equals("UTF-8") || current_type_cell.Value.ToString().Equals("ASCII"))
            {


                var editor_dialog = new Form_HexEditor(row, col, current_type_cell.Value.ToString(), false);

                editor_dialog.ShowDialog();



            }



        }

        private void UpdateTR2Reader()
        {


            foreach (DataRow row in System_DataTable.Rows)
            {

                int id = Convert.ToInt32(row["Id"].ToString());
                string name = row["Name"].ToString();

                string type = row["Type"].ToString();
                byte arr_index = Convert.ToByte(row["Index"].ToString());
                //NEXT =4;


                Dictionary<int, string> map = new Dictionary<int, string>();

                for (int cell_index = 4; cell_index < row.ItemArray.Length; cell_index++)
                {

                    var cell = row.ItemArray[cell_index];

                    map.Add(Convert.ToInt32(System_DataTable.Columns[cell_index].ColumnName), cell.ToString());



                }


                foreach (var kv in map)
                {

                    if (!type.Equals("ASCII") && !type.Equals("UTF-8") && !type.Equals("UTF-16") && !type.Equals("UTF-16LE")) // So I Can Do This At  string type = row["Type"].ToString();
                    {



                        if (!System_TR2.SetDataByIdNameTypeArrayIndexAndDataIdWithParseStringData(id, name, type, arr_index, kv.Key, kv.Value))
                        {
                            if (MessageBox.Show($"String Converter:Set {id}-{name}-{type}-{arr_index}-{kv.Key}-{kv.Value} Error!\nContinue?", "Error!", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.Cancel)
                            {
                                break;
                            }
                        }
                    }



                }







            }


            foreach (DataRow row in System_DataTable_Hex.Rows)
            {

                int id = Convert.ToInt32(row["Id"].ToString());
                string name = row["Name"].ToString();

                string type = row["Type"].ToString();
                byte arr_index = Convert.ToByte(row["Index"].ToString());
                //NEXT =4;


                Dictionary<int, string> map = new Dictionary<int, string>();

                for (int cell_index = 4; cell_index < row.ItemArray.Length; cell_index++)
                {

                    var cell = row.ItemArray[cell_index];

                    map.Add(Convert.ToInt32(System_DataTable.Columns[cell_index].ColumnName), cell.ToString());



                }


                foreach (var kv in map)
                {

                    if (type.Equals("ASCII") || type.Equals("UTF-8") || type.Equals("UTF-16") || type.Equals("UTF-16LE")) // So I Can Do This At  string type = row["Type"].ToString();
                    {

                        try { 

                        byte[] input_byte = FileUtils.GetBytesByHexString(kv.Value);


                        if (!System_TR2.SetDataByIdNameTypeArrayIndexAndDataIdWithParseBytes(id, name, type, arr_index, kv.Key, input_byte))
                        {
                            if (MessageBox.Show($"Byte Converter:Set {id}-{name}-{type}-{arr_index}-{kv.Key}-{kv.Value} Error!\nContinue?", "Error!", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.Cancel)
                            {
                                break;
                            }
                        }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"String Converter:Set {id}-{name}-{type}-{arr_index}-{kv.Key}-{kv.Value} Error!\nData:{kv.Value}", "Error!", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop);
                        }


                    }



                }







            }




        }

        private void MenuItem_Save_Click(object sender, EventArgs e)
        {

            UpdateTR2Reader();

            var xml_data = System_TR2.SaveAsXml();


            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "xml file(*.xml)|*.xml";
                sfd.RestoreDirectory = true;
                sfd.Title = "Export File:";
                sfd.FileName = input_file_name;

                if (sfd.ShowDialog() == DialogResult.OK)
                {




                    File.WriteAllText(sfd.FileName, xml_data);




                }

            }




        }

        private void MenuItem_Import_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show($"When your import, you must know all not save data will be lost and update, so if you don't need some data just delete that txt file.", "Attention!", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                UpdateTR2Reader();

                FolderBrowserDialog fbd = new FolderBrowserDialog();

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    ImportHexDataTable(fbd.SelectedPath);
                }
            }
        }

        private void MenuItem_Export_Click(object sender, EventArgs e)
        {

            MessageBox.Show($"When your export all text, you need open this by right encoding, beacause this editor will write all binary data just file extension is .txt!", "Attention!", MessageBoxButtons.OK,MessageBoxIcon.Information);

            UpdateTR2Reader();

            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if(fbd.ShowDialog() == DialogResult.OK)
            {
                ExportHexDataTable(fbd.SelectedPath);
            }


        }


        private void ImportHexDataTable(string folderpath)
        {

            DirectoryInfo dir = new DirectoryInfo(folderpath);

            FileInfo[] files = dir.GetFiles("*.txt",SearchOption.TopDirectoryOnly);

            int count = 0;

            foreach (FileInfo file in files)
            {

                string[] name_arr = Path.GetFileNameWithoutExtension(file.FullName).Split('+');


                if (name_arr.Length!=5 )
                {
                    MessageBox.Show($"Load {file.FullName} Error:\nText name must be id+name+type+arr_index+data_id", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {

                    int id = Convert.ToInt32(name_arr[0]);
                    string name = name_arr[1];
                    string type = name_arr[2];
                    byte arr_index = Convert.ToByte(name_arr[3]);
                    int data_id = Convert.ToInt32(name_arr[4]);

                    byte[] input_data = File.ReadAllBytes(file.FullName);

                    System_TR2.SetDataByIdNameTypeArrayIndexAndDataIdWithParseBytes(id,name,type,arr_index,data_id,input_data);

                    count++;

                }


            }

            MessageBox.Show($"Updated {count} data.","Done!");


        }

        private void ExportHexDataTable(string folderpath)
        {

            DirectoryInfo dir = new DirectoryInfo(folderpath);

            int row_number = 0;
            int count = 0;
            foreach (DataRow row in System_DataTable_Hex.Rows)
            {


                string id = row[0].ToString();
                string name = row[1].ToString();
                string type = row[2].ToString();
                string index = row[3].ToString();


                if (type.Equals("ASCII") || type.Equals("UTF-8") || type.Equals("UTF-16") || type.Equals("UTF-16LE")) // So I Can Do This At  string type = row["Type"].ToString();
                {

                    for (int i = 4; i < row.ItemArray.Length; i++)
                    {
                        string output_name = $"{id}+{name}+{type}+{index}+{System_DataTable_Hex.Columns[i].ToString()}.txt";
                        string string_data = System_DataTable_Hex.Rows[row_number][i].ToString();
                        try { 

                        byte[] data = FileUtils.GetBytesByHexString(string_data);

                        

                        File.WriteAllBytes(folderpath + "\\" + output_name, data);
                            count++;
                        }catch (Exception e)
                        {

                            MessageBox.Show($"Save {output_name} Error:\nData:{string_data}\n{e.Message}\n{e.StackTrace}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                    }
                }




                row_number++;

            }

            MessageBox.Show($"Exported {row_number} row {count} data.", "Done!");

        }

        private void MenuItem_Help_Click(object sender, EventArgs e)
        {

            MessageBox.Show($"GECV EX PROJECT BY HAOJUN0823\nhttps://www.haojun0823.xyz/\nemail@haojun0823.xyz\nhttps://www.github.com/haojun0823/gecv", "About");

        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (string.IsNullOrEmpty(input_file_name))
            {
                return;
            }
            else if (MessageBox.Show("Are Your Sure Exit? (You Are Not Save This Data!)", "Are You Sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }



        }

        private void exportExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateTR2Reader();

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "xlsx file(*.xlsx)|*.xlsx";
                sfd.RestoreDirectory = true;
                sfd.Title = "Export Excel File:";
                sfd.FileName = input_file_name;

                if (sfd.ShowDialog() == DialogResult.OK)
                {




                    MiniExcel.SaveAs(sfd.FileName,System_DataTable);
                    MiniExcel.SaveAs(sfd.FileName+".shadow.xlsx", System_DataTable_Hex);




                }

            }


        }
    }
}
