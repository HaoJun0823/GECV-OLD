﻿namespace GECV_EX_TR2_Editor_GUI
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            MenuSet_Main = new MenuStrip();
            MenuItem_Open = new ToolStripMenuItem();
            MenuItem_Save = new ToolStripMenuItem();
            MenuItem_Import = new ToolStripMenuItem();
            MenuItem_Export = new ToolStripMenuItem();
            MenuItem_Excel_Export = new ToolStripMenuItem();
            MenuItem_Help = new ToolStripMenuItem();
            MenuStatus_Main = new StatusStrip();
            DataGridView_Main = new DataGridView();
            MenuItem_SaveTr2 = new ToolStripMenuItem();
            MenuSet_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridView_Main).BeginInit();
            SuspendLayout();
            // 
            // MenuSet_Main
            // 
            MenuSet_Main.Items.AddRange(new ToolStripItem[] { MenuItem_Open, MenuItem_Save, MenuItem_SaveTr2, MenuItem_Import, MenuItem_Export, MenuItem_Excel_Export, MenuItem_Help });
            MenuSet_Main.Location = new Point(0, 0);
            MenuSet_Main.Name = "MenuSet_Main";
            MenuSet_Main.Size = new Size(800, 25);
            MenuSet_Main.TabIndex = 0;
            MenuSet_Main.Text = "MenuSet_Main";
            // 
            // MenuItem_Open
            // 
            MenuItem_Open.Name = "MenuItem_Open";
            MenuItem_Open.Size = new Size(52, 21);
            MenuItem_Open.Text = "Open";
            MenuItem_Open.Click += MenuItem_Open_Click;
            // 
            // MenuItem_Save
            // 
            MenuItem_Save.Name = "MenuItem_Save";
            MenuItem_Save.Size = new Size(47, 21);
            MenuItem_Save.Text = "Save";
            MenuItem_Save.Click += MenuItem_Save_Click;
            // 
            // MenuItem_Import
            // 
            MenuItem_Import.Name = "MenuItem_Import";
            MenuItem_Import.Size = new Size(88, 21);
            MenuItem_Import.Text = "Import Text";
            MenuItem_Import.Click += MenuItem_Import_Click;
            // 
            // MenuItem_Export
            // 
            MenuItem_Export.Name = "MenuItem_Export";
            MenuItem_Export.Size = new Size(86, 21);
            MenuItem_Export.Text = "Export Text";
            MenuItem_Export.Click += MenuItem_Export_Click;
            // 
            // MenuItem_Excel_Export
            // 
            MenuItem_Excel_Export.Name = "MenuItem_Excel_Export";
            MenuItem_Excel_Export.Size = new Size(91, 21);
            MenuItem_Excel_Export.Text = "Export Excel";
            MenuItem_Excel_Export.Click += exportExcelToolStripMenuItem_Click;
            // 
            // MenuItem_Help
            // 
            MenuItem_Help.Name = "MenuItem_Help";
            MenuItem_Help.Size = new Size(55, 21);
            MenuItem_Help.Text = "About";
            MenuItem_Help.Click += MenuItem_Help_Click;
            // 
            // MenuStatus_Main
            // 
            MenuStatus_Main.Location = new Point(0, 428);
            MenuStatus_Main.Name = "MenuStatus_Main";
            MenuStatus_Main.Size = new Size(800, 22);
            MenuStatus_Main.TabIndex = 2;
            MenuStatus_Main.Text = "MenuStatus_Main";
            // 
            // DataGridView_Main
            // 
            DataGridView_Main.AllowUserToAddRows = false;
            DataGridView_Main.AllowUserToDeleteRows = false;
            DataGridView_Main.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridView_Main.Dock = DockStyle.Fill;
            DataGridView_Main.Location = new Point(0, 25);
            DataGridView_Main.Name = "DataGridView_Main";
            DataGridView_Main.Size = new Size(800, 403);
            DataGridView_Main.TabIndex = 1;
            DataGridView_Main.VirtualMode = true;
            DataGridView_Main.CellDoubleClick += DataGridView_Main_CellDoubleClick;
            DataGridView_Main.ColumnAdded += DataGridView_Main_ColumnAdded;
            // 
            // MenuItem_SaveTr2
            // 
            MenuItem_SaveTr2.Name = "MenuItem_SaveTr2";
            MenuItem_SaveTr2.Size = new Size(70, 21);
            MenuItem_SaveTr2.Text = "Save Tr2";
            MenuItem_SaveTr2.Click += MenuItem_SaveTr2_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(DataGridView_Main);
            Controls.Add(MenuStatus_Main);
            Controls.Add(MenuSet_Main);
            MainMenuStrip = MenuSet_Main;
            Name = "Main";
            Text = "GECV EX TR2 Editor GUI";
            FormClosing += Main_FormClosing;
            MenuSet_Main.ResumeLayout(false);
            MenuSet_Main.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridView_Main).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip MenuSet_Main;
        private ToolStripMenuItem MenuItem_Open;
        private ToolStripMenuItem MenuItem_Save;
        private StatusStrip MenuStatus_Main;
        private DataGridView DataGridView_Main;
        private ToolStripMenuItem MenuItem_Import;
        private ToolStripMenuItem MenuItem_Export;
        private ToolStripMenuItem MenuItem_Help;
        private ToolStripMenuItem MenuItem_Excel_Export;
        private ToolStripMenuItem MenuItem_SaveTr2;
    }
}