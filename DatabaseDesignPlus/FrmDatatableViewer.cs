using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DatabaseDesignPlus
{
    public partial class FrmDatatableViewer : Form
    {
        public FrmDatatableViewer(DataTable dt)
        {
            InitializeComponent();
            superGridControl1.PrimaryGrid.DataSource = dt;
            this.Text = dt.TableName;

        }

    }
}
