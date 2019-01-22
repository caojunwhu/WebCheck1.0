using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DatabaseDesignPlus;
using System.Collections;
using System.Security.Cryptography;
using System.IO;

namespace DatabaseDesignPlus
{

    public partial class FrmPostgresSetting : Form
    {
        public FrmPostgresSetting()
        {
            InitializeComponent();
        }

        public FrmPostgresSetting(DatabaseParas dp)
        {
            InitializeComponent();

            textBox2.Text = dp.HostName;
            comboBox1.Text= dp.DatabaseName  ;
            comboBox2.Text= dp.UserName ;
            textBox5.Text= dp.Password ;
        }
        string pgconnectionstring = "";
        public string PGConnectionstring
        {
            get { return pgconnectionstring; }
        }

        public DatabaseParas DBPara
        {
            get
            {
                DatabaseParas _dbpara = new DatabaseParas();
                _dbpara.HostName = HostName;
                _dbpara.DatabaseName = DatabaseName;
                _dbpara.UserName = UserName;
                _dbpara.Password = Password;
                return _dbpara;
            }
        }

        string HostName { set; get; }
        string DatabaseName { set; get; }
        string UserName { set; get; }
        string Password { set; get; }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "" ||
                comboBox1.Text == "" ||
                comboBox2.Text == "" ||
                textBox5.Text == "")
            {
                MessageBox.Show("填写相关信息");
                return;
            }
            HostName = textBox2.Text;
            DatabaseName = comboBox1.Text;
            UserName = comboBox2.Text;
            Password = textBox5.Text;
            //Server=localhost;Port=5432;Database=SurveryProductCheckDatabase;uid=postgres;password=123;
            pgconnectionstring = string.Format("host={0};Port=5432;Database={1};uid={2};password={3};", textBox2.Text, comboBox1.Text, comboBox2.Text, textBox5.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }

    public class Secret
    {
        static string encryptKey = "Key2";    //定义密钥 

        #region 加密字符串  
        /// <summary> /// 加密字符串   
        /// </summary>  
        /// <param name="str">要加密的字符串</param>  
        /// <returns>加密后的字符串</returns>  
        public static string Encrypt(string str)
        {
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象   

            byte[] key = Encoding.Unicode.GetBytes(encryptKey); //定义字节数组，用来存储密钥    

            byte[] data = Encoding.Unicode.GetBytes(str);//定义字节数组，用来存储要加密的字符串  

            MemoryStream MStream = new MemoryStream(); //实例化内存流对象      

            //使用内存流实例化加密流对象   
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);

            CStream.Write(data, 0, data.Length);  //向加密流中写入数据      

            CStream.FlushFinalBlock();              //释放加密流      

            return Convert.ToBase64String(MStream.ToArray());//返回加密后的字符串  
        }
        #endregion

        #region 解密字符串   
        /// <summary>  
        /// 解密字符串   
        /// </summary>  
        /// <param name="str">要解密的字符串</param>  
        /// <returns>解密后的字符串</returns>  
        public static string Decrypt(string str)
        {
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象    

            byte[] key = Encoding.Unicode.GetBytes(encryptKey); //定义字节数组，用来存储密钥    

            byte[] data = Convert.FromBase64String(str);//定义字节数组，用来存储要解密的字符串  

            MemoryStream MStream = new MemoryStream(); //实例化内存流对象      

            //使用内存流实例化解密流对象       
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);

            CStream.Write(data, 0, data.Length);      //向解密流中写入数据     

            CStream.FlushFinalBlock();               //释放解密流      

            return Encoding.Unicode.GetString(MStream.ToArray());       //返回解密后的字符串  
        }
        #endregion

    }
    public class DatabaseParas
    {
        public string HostName { set; get; }
        public string DatabaseName { set; get; }
        public string UserName { set; get; }
        public string Password { set; get; }
        public DatabaseParas()
        {

        }

        public bool Read(string KeyFilePath)
        {
            if (File.Exists(KeyFilePath) != true) return false;

            StreamReader sr = new StreamReader(KeyFilePath, Encoding.Default);
            string line;
            line = sr.ReadLine();
            string keys = Secret.Decrypt(line);
            string[] keylist = keys.Split(';');
            if (keylist.Length != 4) return false;

            HostName = keylist[0];
            DatabaseName = keylist[1];
            UserName = keylist[2];
            Password = keylist[3];
            return true;
        }

        public bool Write(string KeyFilePath)
        {
            string keystring = string.Format("{0};{1};{2};{3}", HostName, DatabaseName, UserName, Password);
            if (keystring.Length <= 4) return false;
            keystring = Secret.Encrypt(keystring);

            FileStream fs = new FileStream(KeyFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(keystring);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
            return true;
        }

    }
}
