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
    public partial class FrmAuthorizeApply : Form
    {
        public FrmAuthorizeApply()
        {
            InitializeComponent();

            tbApplytime.Text = DateTime.Now.ToShortDateString();
            tbIPAddress.Text = IPMac.GetIP();
            tbMacAddress.Text = IPMac.GetEternetPhysicalAddress();
        }
        string sDatabaseConnectionstring = "";

        public string SDatabaseConnectionstring
        {
            get { return sDatabaseConnectionstring; }
            set { sDatabaseConnectionstring = value; }
        }


        private void btnApply_Click(object sender, EventArgs e)
        {
            string ipaddress = IPMac.GetIP();
            string macaddress = IPMac.GetEternetPhysicalAddress();
            string createtime = DateTime.Now.ToShortDateString();
            string authorized = "0";

            if (tbApplytime.Text == "" ||
                tbIPAddress.Text == "" ||
                tbMacAddress.Text == "" ||
                tbCompany.Text == ""||
                tbPassword.Text == "" ||
                tbPasswordRepeat.Text == "" ||
                tbUsername.Text == "")
            {
                MessageBox.Show("请将信息补充完整！");
                return;
            }
            if(tbPasswordRepeat.Text != tbPassword.Text)
            {
                MessageBox.Show("两次密码输入不一致！");
                return;
            }
            string usertablename = "用户表";
            string queryusername = string.Format("select username from {0} where username = '{1}' and macaddress = '{2}'", usertablename, tbUsername.Text,macaddress);
            DatabaseDesignPlus.DatabaseReaderWriter dbreader = new DatabaseDesignPlus.ClsPostgreSql(sDatabaseConnectionstring) ;
            //dbreader.DatatbaseConnectionID = sDatabaseConnectionstring;
            object username =  dbreader.GetScalar(queryusername) as string ;
            if (username != "" && username!=null)
            {
                MessageBox.Show("该用户名已经在本机上使用授权，请更换用户名；或者申请密码找回！");
                return;
            }

            if (username == null)
            {
                string queryuserid = string.Format("select max(cast(userid as integer))+1 from {0}", usertablename);
                object userid = dbreader.GetScalar(queryuserid);

                //IPMac ipmac = new IPMac();

                string insertnewuser = string.Format("insert into {0} (userid ,username,password,ipaddress,macaddress,createtime,authorized,company)values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}') ", usertablename, userid, tbUsername.Text, tbPassword.Text, ipaddress, macaddress, createtime, authorized, tbCompany.Text);

                if (dbreader.ExecuteSQL(insertnewuser) >= 0)
                {
                    MessageBox.Show("授权申请提交成功，请等候审核！");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
