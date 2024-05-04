using GECV_EX.TR2;
using GECV_EX.Utils;
using MiniExcelLibs;
using System.Data;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
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
                ofd.Filter = "(TR2 Editor Supported File)|*.tr2;*.xml";
                ofd.RestoreDirectory = false;


                if (ofd.ShowDialog() == DialogResult.OK)
                {

                    OpenFile(ofd.FileName);

                }



            }


        }


        private void OpenFile(string filename)
        {

            string select_file = filename;


            string ext = Path.GetExtension(select_file);

            input_file_name = Path.GetFileNameWithoutExtension(select_file);

            try
            {


                if (ext.ToLower() == ".xml")
                {



                    System_TR2 = XmlUtils.Load<TR2Reader>(select_file);
                    this.Text = original_title + $" Building {System_TR2.table_name} Table, Please Wait...";
                    BuildDataTable(System_TR2, true);
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
                    BuildDataTable(System_TR2, true);
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



        private void SetMenuStatus(bool status)
        {

            this.MenuItem_Export.Enabled = status;
            this.MenuItem_Import.Enabled = status;
            this.MenuItem_Save.Enabled = status;
            this.MenuItem_SaveTr2.Enabled = status;
            this.MenuItem_Excel_Export.Enabled = status;

        }

        //private bool CheckTR2StringArray(TR2Reader tr2data)
        //{

        //    bool result = false;






        //    return result;
        //}


        private void BuildDataTable(TR2Reader tr2data, bool check = false)
        {
            DataTable dt = new DataTable();

            dt.TableName = tr2data.table_name;



            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Index", typeof(int));
            //dt.Columns.Add("Array", typeof(byte));
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

            List<string> Dulpicate_list = new List<string>();

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


                for (int si = 0; si < tr2data.column_counter.id.Length; si++)
                {
                    Console.WriteLine($"Debug Build Table 76:{tr2data_inf.id}-{tr2data_inf.column_data.column_name}-{si}(Array Length:{tr2data_inf.column_data.column_data_list.Length})");

                    var data_arr = tr2data_inf.column_data;


                    //DataRow dr = dt.NewRow();
                    //dr["Id"] = tr2data_inf.id;
                    //dr["Name"] = tr2data_inf.column_data.column_name;
                    //dr["Type"] = tr2data_inf.column_data.column_type;
                    ////dr["Index"] = ssi;

                    //DataRow dr_hex = dt_hex.NewRow();
                    //dr_hex["Id"] = tr2data_inf.id;
                    //dr_hex["Name"] = tr2data_inf.column_data.column_name;
                    //dr_hex["Type"] = tr2data_inf.column_data.column_type;
                    ////dr_hex["Index"] = ssi;

                    //dt.Rows.Add(dr);
                    //dt_hex.Rows.Add(dr_hex);

                    //StringBuilder sb = new StringBuilder();

                    if (tr2data_inf.column_data.column_data_list[si].DulpicatedObjectOffset != 0)
                    {
                        Dulpicate_list.Add($"{tr2data_inf.id}-{tr2data_inf.column_data.column_name}-{tr2data_inf.column_data.column_type}-{tr2data.column_counter.id[si].ToString()}.");
                    }

                    for (byte ssi = 0; ssi < data_arr.data_76_array_size; ssi++)
                    {
                        Console.WriteLine($"Debug Build Table 76 Data:{tr2data_inf.id}-{tr2data_inf.column_data.column_name}-{si}-{ssi}(Array Length:{data_arr.data_76_array_size})");

                        DataRow dr = GetDatRowFromTable(ref dt, tr2data_inf.id, tr2data_inf.column_data.column_name, tr2data_inf.column_data.column_type, ssi);
                        DataRow dr_hex = GetDatRowFromTable(ref dt_hex, tr2data_inf.id, tr2data_inf.column_data.column_name, tr2data_inf.column_data.column_type, ssi);

                        if (tr2data_inf.column_data.column_data_list[si].IsInVaildOffset || tr2data_inf.column_data.column_data_list[si].column_data[ssi].IsInVaildArrayOffset)
                        {
                            dr[tr2data.column_counter.id[si].ToString()] = "[[GECV-EDITOR::NULL]]";
                            dr_hex[tr2data.column_counter.id[si].ToString()] = "[[GECV-EDITOR::NULL]]";
                        }
                        else
                        {




                            var data_arr_data = tr2data_inf.column_data.column_data_list[si].column_data[ssi];

                            //sb.Append(ssi);
                            //sb.Append(":{{");
                            //sb.Append(data_arr_data.value_string_view.ToString());
                            //sb.Append("}}");






                            dr[tr2data.column_counter.id[si].ToString()] = data_arr_data.value_string_view.ToString();
                            dr_hex[tr2data.column_counter.id[si].ToString()] = data_arr_data.value_hex_view.ToString();



                            //dr["Editor Mode"] = GetModeByDataType(dr["Type"].ToString());

                        }



                    }




                }






            }

            StringBuilder sb = new StringBuilder();

            foreach (var str in Dulpicate_list) { sb.Append(str); sb.Append('\n'); };

            if (check && Dulpicate_list.Count != 0)
            {

                MessageBox.Show($"There are {Dulpicate_list.Count} data is dulpicate object:\n\n{sb.ToString()}\nThe source project will merge the same data to the same address (For optimization and publishing, since the data does not need to be modified again.), which is not possible to edit in the table.\nThe editor will allocate new pointers for each data, even though they all have the same data.\nYou need to know this. ", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }





            System_DataTable = dt;
            System_DataTable_Hex = dt_hex;
        }


        private DataRow GetDatRowFromTable(ref DataTable dt, int id, string column_name, string column_type, byte index)
        {

            //DataRow dr = dt.NewRow();
            //dr["Id"] = tr2data_inf.id;
            //dr["Name"] = tr2data_inf.column_data.column_name;
            //dr["Type"] = tr2data_inf.column_data.column_type;
            ////dr["Index"] = ssi;

            //DataRow dr_hex = dt_hex.NewRow();
            //dr_hex["Id"] = tr2data_inf.id;
            //dr_hex["Name"] = tr2data_inf.column_data.column_name;
            //dr_hex["Type"] = tr2data_inf.column_data.column_type;
            ////dr_hex["Index"] = ssi;

            //dt.Rows.Add(dr);
            //dt_hex.Rows.Add(dr_hex);


            if (dt.Rows != null && dt.Rows.Count != 0)
            {

                foreach (DataRow dr in dt.Rows)
                {

                    if (dr["Id"].ToString() == id.ToString() && dr["Name"].ToString() == column_name.ToString() && dr["Type"].ToString() == column_type.ToString() && dr["Index"].ToString() == index.ToString())
                    {

                        return dr;

                    }


                }
            }

            DataRow dr2 = dt.NewRow();

            dr2["Id"] = id;
            dr2["Name"] = column_name;
            dr2["Type"] = column_type;
            dr2["Index"] = index;

            dt.Rows.Add(dr2);

            return dr2;




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

                //if (current_type_cell.Value.ToString().Equals("UTF-16LE") || current_type_cell.Value.ToString().Equals("UTF-16"))
                if(TR2Reader.IsStringFormat(current_type_cell.Value.ToString()))
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

                        try
                        {

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


                    MessageBox.Show($"Xml has been abandoned as we have the full TR2 converter.\r\nThis part is only available in Debug.", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

            }




        }

        private void MenuItem_Import_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"When your import, you must know all not save data will be lost and update, so if you don't need some data just delete that txt file.", "Attention!", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
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

            MessageBox.Show($"When your export all text, you need open this by right encoding, beacause this editor will write all binary data just file extension is .txt!", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            UpdateTR2Reader();

            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                ExportHexDataTable(fbd.SelectedPath);
            }


        }


        private void ImportHexDataTable(string folderpath)
        {

            DirectoryInfo dir = new DirectoryInfo(folderpath);

            FileInfo[] files = dir.GetFiles("*.txt", SearchOption.TopDirectoryOnly);

            int count = 0;

            foreach (FileInfo file in files)
            {

                string[] name_arr = Path.GetFileNameWithoutExtension(file.FullName).Split('+');


                if (name_arr.Length != 5)
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

                    if (!System_TR2.SetDataByIdNameTypeArrayIndexAndDataIdWithParseBytes(id, name, type, arr_index, data_id, input_data))
                    {


                        MessageBox.Show($"Id:{id},Name:{name},type:{type},arr_index:{arr_index},data_id:{data_id},input_data:{input_data}\nUpdate Error!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }

                    count++;

                }


            }

            BuildDataTable(System_TR2);
            RefreshDataTable();
            MessageBox.Show($"Updated {count} data.", "Done!");


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
                        try
                        {

                            byte[] data = FileUtils.GetBytesByHexString(string_data);



                            File.WriteAllBytes(folderpath + "\\" + output_name, data);
                            count++;
                        }
                        catch (Exception e)
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

            MessageBox.Show($"GECV EX PROJECT BY HAOJUN0823\nhttps://www.haojun0823.xyz/\nemail@haojun0823.xyz\nhttps://www.github.com/haojun0823/gecv\nIf you edit xml file by other editor you need know this editor maybe work wrong!", "About");

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

                    if(File.Exists(sfd.FileName))
                    {
                        File.Delete(sfd.FileName);
                    }

                    if (File.Exists(sfd.FileName+".shadow.xlsx"))
                    {
                        File.Delete(sfd.FileName+".shadow.xlsx");
                    }


                    MiniExcel.SaveAs(sfd.FileName, System_DataTable);
                    MiniExcel.SaveAs(sfd.FileName + ".shadow.xlsx", System_DataTable_Hex);



                    MessageBox.Show($"Because of Microsoft everything will be confusing to convert:\n{sfd.FileName + ".shadow.xlsx"} Is used to view text in different encodings.\n\nThis editor is a two-layer data table:\r\nThe display layer is used for ordinary data and the system can recognize the content.\r\nThe shadow layer is used to store binary data, and most text is modified through this layer.","Micorosft Problem!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }

            }


        }

        private void MenuItem_SaveTr2_Click(object sender, EventArgs e)
        {
            UpdateTR2Reader();



            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "tr2 file(*.tr2)|*.tr2";
                sfd.RestoreDirectory = true;
                sfd.Title = "Export File:";
                sfd.FileName = input_file_name;

                if (sfd.ShowDialog() == DialogResult.OK)
                {




                    TR2Writer writer = new TR2Writer(System_TR2);

                    File.WriteAllBytes(sfd.FileName, writer.GetTr2Data());

                    File.WriteAllLines(sfd.FileName + ".log", writer.GetBookInformation());


                }

            }
        }

        private void Main_DragEnter(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }



        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {


                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);


                if (files.Length > 1)
                {

                    MessageBox.Show($"You drop {files.Length} files!\nOnly open {files[0]}.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }



                OpenFile(files[0]);


            }
            else
            {
                MessageBox.Show($"You drop invaild data.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void importExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This function is not working beacause you can use Excel or other tools to do that.", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            return;


            UpdateTR2Reader();

            using (OpenFileDialog sfd = new OpenFileDialog())
            {
                sfd.Filter = "xlsx file(*.xlsx)|*.xlsx";
                sfd.RestoreDirectory = true;
                sfd.Title = "Import Excel File:";
                sfd.FileName = input_file_name;

                if (sfd.ShowDialog() == DialogResult.OK)
                {


                    DataTable dt = MiniExcel.QueryAsDataTable(sfd.FileName, useHeaderRow: true);


                    MessageBox.Show($"You are import {sfd.FileName} now.\nUTF-16 data cannot import because some data is special font.", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);





                }

            }
        }

        private void DataGridView_Main_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
