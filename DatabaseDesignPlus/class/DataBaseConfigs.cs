using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DatabaseDesignPlus
{
    public  class DataBaseConfigs
    {
        private static string databaseEngineType = "PostgreSQL";

        public static string DatabaseEngineType
        {
            get
            {
                return databaseEngineType;
            }

            set
            {
                databaseEngineType = value;
            }
        }
        public static string GetGISDbDatasource(string databaseEngineType)
        {
            string dbconnection = "";
            if (databaseEngineType == "PostgreSQL")
            {
                dbconnection = System.Configuration.ConfigurationManager.AppSettings["PGDatabase"];
                dbconnection = DataBaseConfigs.RePlaceConfig(dbconnection, "PostgreSQL");

            }
            else if (databaseEngineType == "SQLite")
            {
                dbconnection = System.Configuration.ConfigurationManager.AppSettings["SQLiteDatabase"];

            }
            else if (databaseEngineType == "Spatialite")
            {
                dbconnection = System.Configuration.ConfigurationManager.AppSettings["SQLiteDatabase"];

            }
            return dbconnection;

        }

        static public string RePlaceConfig(string dbconnection, string DatabaseEngineType = "PostgreSQL")
        {
            string dbConfigPath = string.Format("{0}//Database.Config", Application.StartupPath);
            DatabaseParas dbpara = new DatabaseParas();
            if (File.Exists(dbConfigPath) == true)
            {
                if (dbpara.Read(dbConfigPath) == true)
                {

                }
                else
                {
                    FrmPostgresSetting fps = new FrmPostgresSetting();
                    if (fps.ShowDialog() == DialogResult.OK)
                    {
                        dbpara = fps.DBPara;
                        dbpara.Write(dbConfigPath);
                    }
                }
            }
            else
            {
                FrmPostgresSetting fps = new FrmPostgresSetting();
                if (fps.ShowDialog() == DialogResult.OK)
                {
                    dbpara = fps.DBPara;
                    dbpara.Write(dbConfigPath);
                }
            }

            if (dbpara.HostName == "" || dbpara.DatabaseName == "" || dbpara.UserName == "" || dbpara.Password == "")
                return null;

            if (dbconnection == "")
                return null;

            dbconnection = dbconnection.Replace("{HostName}", dbpara.HostName);
            dbconnection = dbconnection.Replace("{DatabaseName}", dbpara.DatabaseName);
            dbconnection = dbconnection.Replace("{UserName}", dbpara.UserName);
            dbconnection = dbconnection.Replace("{Password}", dbpara.Password);

            return dbconnection;
        }
        static public string RePlaceConfig(string dbconnection)
        {
            string dbConfigPath = string.Format("{0}//Database.Config", Application.StartupPath);
            DatabaseParas dbpara = new DatabaseParas();
            if (File.Exists(dbConfigPath) == true)
            {
                if (dbpara.Read(dbConfigPath) == true)
                {

                }
                else
                {
                    FrmPostgresSetting fps = new FrmPostgresSetting();
                    if (fps.ShowDialog() == DialogResult.OK)
                    {
                        dbpara = fps.DBPara;
                        dbpara.Write(dbConfigPath);
                    }
                }
            }
            else
            {
                FrmPostgresSetting fps = new FrmPostgresSetting();
                if (fps.ShowDialog() == DialogResult.OK)
                {
                    dbpara = fps.DBPara;
                    dbpara.Write(dbConfigPath);
                }
            }

            if (dbpara.HostName == "" || dbpara.DatabaseName == "" || dbpara.UserName == "" || dbpara.Password == "")
                return null;

            if (dbconnection == "")
                return null;

            dbconnection = dbconnection.Replace("{HostName}", dbpara.HostName);
            dbconnection = dbconnection.Replace("{DatabaseName}", dbpara.DatabaseName);
            dbconnection = dbconnection.Replace("{UserName}", dbpara.UserName);
            dbconnection = dbconnection.Replace("{Password}", dbpara.Password);

            return dbconnection;
        }
    }
}
