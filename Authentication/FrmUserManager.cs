using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Authentication
{
    public partial class FrmUserManager : Form
    {
        public FrmUserManager()
        {
            InitializeComponent();
            //LoadUsers();
        }
        string databaseconnection = "";

        public string Databaseconnection
        {
            get { return databaseconnection; }
            set { databaseconnection = value; }
        }

        DatabaseDesignPlus.DatabaseReaderWriter dbreader = null;

        public void LoadUsers()
        {
            if (dbreader == null)
            {
                dbreader = new DatabaseDesignPlus.ClsPostgreSql(databaseconnection);
            }
            string usertablename = "用户表";
            //已授权用户列表
            string userauthorized = string.Format("select username,ipaddress,macaddress,createtime from {0} where authorized = '1' order by username asc ", usertablename);
            DataTable table1 = dbreader.GetDataTableBySQL(userauthorized);
            //未授权用户列表
            string usernotauthorized = string.Format("select username,ipaddress,macaddress,createtime from {0} where authorized = '0'  order by username asc ", usertablename);
            DataTable table2 = dbreader.GetDataTableBySQL(usernotauthorized);

            dataGridView1.DataSource = table2;
            dataGridView2.DataSource = table1;
        }

        private void 授权该用户ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 0)
            {
                MessageBox.Show("请选择某个用户，行选！");
                return;
            }
            if (dataGridView1.SelectedRows.Count == 1)
            {
                string username = "";
                username = dataGridView1.SelectedRows[0].Cells[0].Value as string;
                string macaddress = dataGridView1.SelectedRows[0].Cells[2].Value as string;
                string athorizeduser = string.Format("update {0} set authorized ='1' where username = '{1}' and macaddress = '{2}'", "用户表", username, macaddress);
                if (dbreader.ExecuteSQL(athorizeduser) > 0)
                {
                    MessageBox.Show("成功授权！");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void 删除用户ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 0)
            {
                MessageBox.Show("请选择某个用户，行选！");
                return;
            }
            if (dataGridView1.SelectedRows.Count == 1)
            {
                string username = "";
                username = dataGridView1.SelectedRows[0].Cells[0].Value as string;
                string macaddress = dataGridView1.SelectedRows[0].Cells[2].Value as string;
                string athorizeduser = string.Format("delete  from {0} where username = '{1}' and macaddress = '{2}'", "用户表", username, macaddress);
                if (dbreader.ExecuteSQL(athorizeduser) > 0)
                {
                    MessageBox.Show("成功移除用户！");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void 解除用户授权ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count <= 0)
            {
                MessageBox.Show("请选择某个用户，行选！");
                return;
            }
            if (dataGridView2.SelectedRows.Count == 1)
            {
                string username = "";
                username = dataGridView2.SelectedRows[0].Cells[0].Value as string;
                if (username == "管理员")
                {
                    MessageBox.Show("不能移除管理员！");
                    return;
                }
                string macaddress = dataGridView2.SelectedRows[0].Cells[2].Value as string;
                string athorizeduser = string.Format("update {0} set authorized ='0' where username = '{1}' and macaddress = '{2}'", "用户表", username, macaddress);
                if (dbreader.ExecuteSQL(athorizeduser) > 0)
                {
                    MessageBox.Show("已解除授权！");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

    }
}
