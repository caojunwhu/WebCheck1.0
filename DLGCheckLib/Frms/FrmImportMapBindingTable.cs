using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DLGCheckLib.Frms
{
    public partial class FrmImportMapBindingTable : Form
    {
        public FrmImportMapBindingTable()
        {
            InitializeComponent();
        }
        string sMapBindingTablePath = "";
        string sPostGISConnection = "";

        public string SPostGISConnection
        {
            get
            {
                return sPostGISConnection;
            }

            set
            {
                sPostGISConnection = value;
            }
        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "ShapeFile|*.shp";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                sMapBindingTablePath = opf.FileName;
                tb_mapbindingtable.Text = sMapBindingTablePath;
            }
        }
        public void ImportShapeFileToPostGIS(string sShapeFilePath)
        {
            //截取文件路径
            string directory = Path.GetPathRoot(sMapBindingTablePath);
            string sLayerName = Path.GetFileNameWithoutExtension(sMapBindingTablePath);

        }
        private void tb_import_Click(object sender, EventArgs e)
        {
            FrmImportShapeFile2PostGIS frmImportShapeFile2PostGIS = new FrmImportShapeFile2PostGIS();
            frmImportShapeFile2PostGIS.ShapeFilePath = sMapBindingTablePath;
            frmImportShapeFile2PostGIS.PgConnectionID = SPostGISConnection;

            if(frmImportShapeFile2PostGIS.ShowDialog()==DialogResult.OK)
            {
                this.Hide();
                this.Dispose();
            }

        }
    }
}
