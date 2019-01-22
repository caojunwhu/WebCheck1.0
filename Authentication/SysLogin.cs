using System;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Npgsql;
using DatabaseDesignPlus;
using CSoftAutoUpdater;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace Authentication
{
    public partial class SysLogin : Form
    {

        string ipaddress = "127.0.0.1";
        string dbname = "hbdlgqgeoinfodb";
        string username = "hbdlgqinfouser";
        string password = "87788106";
        string macaddress = "";

        string loginusername = "";

        public string Loginusername
        {
            get { return loginusername; }
            set { loginusername = value; }
        }
       
          // 定义参数数据库连接对象及文件地址
        public NpgsqlConnection _genericParamDbConnection = null;
        public string _genericParamDbConnectionString = "";
        string usertablename = "用户表";
        string lastlogintablename = "上次登录";
        string lastloginusername = "";
        DatabaseReaderWriter dbReadWriter = null;
        ToolTip tooltip = new ToolTip();

        public string DBConnectionID
        {
            get
            {
                ipaddress = System.Configuration.ConfigurationManager.AppSettings["Host"];
                return string.Format("host={0};Port=5432;Database={1};uid={2};password={3};", ipaddress, dbname, username, password);
            }
        }
        void CheckUpdate()
        {
            SoftUpdate softupdate = new SoftUpdate();
            softupdate.SSourceDbConnectionID = this.DBConnectionID;
            SoftItem.SSourceDbConnectionID = this.DBConnectionID;

            if (softupdate.CheckUpdate(Application.StartupPath) == true)
            {
                string softdownloader = "CSoftDownload.exe";
                if (File.Exists(softdownloader))
                {
                    System.Diagnostics.Process.Start(softdownloader);
                }
                else
                {
                    MessageBox.Show("找不到更新模块，软件无法自动检查更新，请与管理员联系。");
                }

                //return;
                MessageBox.Show("主程序已更新，请更新！");
                this.Close();
                Application.Exit();
            }
        }

        public SysLogin()
        {
            InitializeComponent();
            this.BackgroundImageLayout = ImageLayout.Stretch;
            try
            {
                this.BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, @"Images\" + System.Configuration.ConfigurationManager.AppSettings["LoginImage"]));
                //System.Configuration.ConfigurationSettings.AppSettings["LoginImage"]));
            }
            catch { }
            try
            {
                this.Text = System.Configuration.ConfigurationManager.AppSettings["SystemName"] + "——登录";

            }
            catch 
            {
            }
            
            ReadDbConnection();
        } 

        public void ReadDbConnection()
        {
            try
            {
                _genericParamDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
                dbReadWriter = new ClsPostgreSql(_genericParamDbConnectionString);

            }
            catch (System.IO.FileNotFoundException fe)
            {
                MessageBox.Show("配置文件错误！");
                Application.Exit();
            }
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            TimeSpan span = DateTime.Now.Subtract(DateTime.Parse("2016-09-16 00:00:00"));
            if (span.TotalDays >0)
            {
                MessageBox.Show("登录失败！");
                return;
            }

            string username = tb_username.Text;
            string password = tb_password.Text;

            loginusername = username;

            string queryuser_sql = string.Format("select * from {0} where username = '{1}' and password = '{2}' and macaddress='{3}'", usertablename, username, password,macaddress);
            //_genericParamDbConnection = new NpgsqlConnection(_genericParamDbConnectionString);
            //NpgsqlCommand cmd = new NpgsqlCommand(queryuser_sql, _genericParamDbConnection);

            //_genericParamDbConnection.Open();
            try
            {
                DataTable usertable = dbReadWriter.GetDataTableBySQL(queryuser_sql);
                //object id = cmd.ExecuteScalar();

                //if (id == null)
                //    throw new InvalidOperationException();
                //2015-7-16
                if (usertable.Rows.Count <= 0)
                    throw new InvalidOperationException();

                DataRow dr = usertable.Rows[0];
                string authorized = Convert.ToString(dr["authorized"]);
                if (authorized != "1")
                {
                    MessageBox.Show("您当前用户名在本机还未授权，请申请授权或等待管理员授权！");
                    return;
                }
                // 打开主窗体，并延时关闭此登陆窗体

                this.Hide();
                //SurveryProductCheckMainForm mf = new SurveryProductCheckMainForm(root.Functions, root, username);
               // FrmLogin mf = new FrmLogin();
                //mf.Show();

                CheckUpdate();

                //FrmDLGQInfoLook frmdlgqinfolook = new FrmDLGQInfoLook();
                //frmdlgqinfolook.DLGQInfoDbPath = this.DBConnectionID;
                //frmdlgqinfolook.Loginusername = loginusername;
                //frmdlgqinfolook.GetDbReader();
                //frmdlgqinfolook.LoadInfoClass();

                string login = string.Format("insert into {0}(username,logintime,macaddress,ipaddress,logtype)values('{1}','{2}','{3}','{4}','{5}')", lastlogintablename, tb_username.Text, DateTime.Now.ToString(), macaddress, ipaddress,  "login");
                dbReadWriter.ExecuteSQL(login);

                //if (frmdlgqinfolook.ShowDialog() == DialogResult.OK)
                //{
                //    this.Close();
                //}
                
               // _genericParamDbConnection.Close();

            }
            catch (System.InvalidOperationException e1)
            {
                //MessageBox.Show(e.Message);
                //未找到对应的用户名，请检查输入；
                MessageBox.Show("未找到对应本机授权的用户名和密码，请检查输入是否正确！");

            }

           // _genericParamDbConnection.Close();

        }

        private void btn_quit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("警告：是否退出该系统？", "退出系统", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                this.Close();
            else
                return;
        }
        private void SysLogin_Load(object sender, EventArgs e)
        {
            //登陆界面载入时从login数据库中“上次登录”中取得上次登录的用户名和密码，填入到输入框中
            //2015-7-16 获取MAC地址
            ipaddress = IPMac.GetIP();
            macaddress = IPMac.GetEternetPhysicalAddress();
            string queryuser_sql = string.Format("select * from {0} where macaddress='{1}' order by logintime desc", lastlogintablename, macaddress);
            //_genericParamDbConnection = new NpgsqlConnection(_genericParamDbConnectionString);
            //NpgsqlCommand cmd = new NpgsqlCommand(queryuser_sql, _genericParamDbConnection);

            try
            {
               // _genericParamDbConnection.Open();
                //object username = cmd.ExecuteReader();
                DataTable usertable = dbReadWriter.GetDataTableBySQL(queryuser_sql);

                if (usertable.Rows.Count == 0)
                    throw new InvalidOperationException();
                DataRow dr = usertable.Rows[0];

                tb_username.Text = Convert.ToString(dr["username"]);
                lastloginusername = tb_username.Text;
                string lastlogintime = Convert.ToString(dr["logintime"]);

                //////////////////////////////////////////////////////////////
                //2015-7-16 查询该MAC地址下是否授权
                string usertablename = "用户表";
                string queryauthorized = string.Format("select authorized from {0} where macaddress = '{1}'", usertablename, macaddress);
                object authorizedobj = dbReadWriter.GetScalar(queryauthorized);

                string authorized = "";
                if(authorizedobj==null) authorized = "未授权";
                else authorized = Convert.ToInt32(authorizedobj) == 1 ? "已授权" : "未授权";

                string authorizeinfo = string.Format("本机MAC地址是{0}，{1}，上次登陆时间是{2}。", macaddress, authorized, lastlogintime);
                labelAuthorizedInfo.Text = authorizeinfo;
               
            }
            catch (System.InvalidOperationException e1)
            {
                //MessageBox.Show(e.Message);
                //未找到对应的用户名，请检查输入；
                //MessageBox.Show("！");     
                string authorizeinfo = string.Format("本机MAC地址是{0}，{1}.", macaddress, "未授权");
                labelAuthorizedInfo.Text = authorizeinfo;
            }

            tooltip.SetToolTip(tb_password, GetUpdateInfoString());
            
        }

        private void SysLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            //2015-7-16登陆成功记录username,logintime,macaddress,ipaddress
            if (lastloginusername != "")
            {
                string logout = string.Format("insert into {0}(username,logintime,macaddress,ipaddress,logtype)values('{1}','{2}','{3}','{4}','{5}')", lastlogintablename, tb_username.Text, DateTime.Now.ToString(), macaddress, ipaddress, "logout");

                //string updatalastlogin = string.Format("update {0} set username = '{1}',logintime = '{2}',macaddress='{3}',ipaddress='{4}' where username = '{5}' and macaddress='{6}'", lastlogintablename, tb_username.Text, DateTime.Now.ToShortTimeString(), macaddress,ipaddress,lastloginusername,macaddress);

                _genericParamDbConnection = new NpgsqlConnection(_genericParamDbConnectionString);
                NpgsqlCommand cmd = new NpgsqlCommand(logout, _genericParamDbConnection);

                _genericParamDbConnection.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                    _genericParamDbConnection.Close();
                }
                catch (System.InvalidOperationException e1)
                {
                    _genericParamDbConnection.Close();
                }
            }
        }

        private void linkLabelAuthorizeApply_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmAuthorizeApply faap = new FrmAuthorizeApply();
            faap.SDatabaseConnectionstring = _genericParamDbConnectionString;
            if (faap.ShowDialog() == DialogResult.OK)
            {

            }

        }

        private void linkLabelForgetPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //throw new NotImplementedException();
            MessageBox.Show("请联系系统管理员！");
        }

        private void linkLabelHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
           // throw new NotImplementedException();
            string helpdoc = Application.StartupPath + "\\" + 
                System.Configuration.ConfigurationManager.AppSettings["HelpDocument"];
            if (File.Exists(helpdoc))
            {
                Process process = new Process();
                process.StartInfo.FileName = helpdoc;
                process.Start();
            }

        }

        private void linkLabelAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutBox1 aboutbox = new AboutBox1();
            if (aboutbox.ShowDialog() == DialogResult.OK)
            {
            }
        }

        private void tb_password_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btn_login_Click(null, null);
            }
        }

        public string GetUpdateInfoString()
        {
            string updateinfostring = "";
            string getupdateinfostring = string.Format("select * from 更新日志 order by 更新日期 desc");
            DataTable updateinfotable = dbReadWriter.GetDataTableBySQL(getupdateinfostring);
            foreach (DataRow dr in updateinfotable.Rows)
            {
                updateinfostring += dr["模块"]+":于";
                updateinfostring += Convert.ToDateTime(dr["更新日期"]).ToShortDateString();
                updateinfostring += dr["更新内容"]+ ",由"; 
                updateinfostring += dr["贡献者"]+"根据";
                updateinfostring += dr["来源"]+"整理\r";
               
            }

            return updateinfostring;
        }
    }
}
