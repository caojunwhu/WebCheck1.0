using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using DatabaseDesignPlus;

namespace CSoftAutoUpdater
{
    public class SoftItem
    {
        public SoftItem()
        {
            _softtable = new DataTable();
            _softtable.Columns.Add("softnumber");
            _softtable.Columns.Add("softname");
            _softtable.Columns.Add("softversion");
            _softtable.Columns.Add("updatedate");
            _softtable.Columns.Add("filefilter");
            _softtable.Columns.Add("comment");
            Modules = new List<ModuleItem>();

            _moduletable = new DataTable();
            _moduletable.Columns.Add("modulenumber");
            _moduletable.Columns.Add("softname");
            _moduletable.Columns.Add("softversion");
            _moduletable.Columns.Add("modulename");
            _moduletable.Columns.Add("moduletype");
            _moduletable.Columns.Add("moduleversion");
            _moduletable.Columns.Add("modulesourcepath");
            _moduletable.Columns.Add("moduleentity");
        }

        public string GetModuleType(string modulename)
        {
            return "";
        }

        public static string GetVersionName(string modulepath)
        {
            FileVersionInfo assem = System.Diagnostics.FileVersionInfo.GetVersionInfo(modulepath);            
            string versionname = assem.FileVersion;
            if (versionname == "" || versionname == null)
            {
                return File.GetLastWriteTime(modulepath).ToShortDateString();
             }
            return versionname;
        }
        public static string GetMajorMinorVersion(string version)
        {
            string[] vers = version.Split('.');
            string majorminorversion = string.Format("{0}.{1}", vers[0], vers[1]);
            return majorminorversion;
        }

        public static string[] GetModuleFilesPath(string softpath)
        {
            string[] modules = Directory.GetFiles(softpath);
            return modules;

        }

        public void  LoadSoftFromFile(string softpath)
        {
            _softtable.Rows.Clear();

            DataRow softdatarow = _softtable.NewRow();
            softdatarow["softnumber"] = SoftNumber;
            softdatarow["softname"]=SoftName;
            softdatarow["softversion"]=SoftVersion;
            softdatarow["updatedate"]=DateTime.Now.Date.ToShortDateString();
            softdatarow["filefilter"]=FileFilter;
            softdatarow["comment"]=Comment;

            _softtable.Rows.Add(softdatarow);

            string[] modules = GetModuleFilesPath(softpath);
            _moduletable.Rows.Clear();

            foreach (string modulepath in modules)
            {                
                if(FileFilter.IndexOf(Path.GetExtension(modulepath))<0)
                    continue;
                try
                {
                    ModuleItem moduleitem = new ModuleItem();
                    moduleitem.ModuleVersion = GetVersionName(modulepath);
                    moduleitem.SoftName = SoftName;
                    moduleitem.ModuleType = GetModuleType(moduleitem.ModuleName);
                    moduleitem.SoftVersion = this.SoftVersion;
                    moduleitem.ModuleSourcePath = Path.GetFullPath(modulepath);
                    moduleitem.ModuleName = Path.GetFileName(modulepath);
                    //FileStream fs = new FileStream(modulepath, FileMode.Open);
                    //byte[] bytes = new byte[fs.Length];
                    //fs.Read(bytes, 0, (int)fs.Length);
                    //moduleitem.ModuleEntity = CommonUtil.ByteArrayToString(bytes);
                    moduleitem.ModuleNumber = GetModuleNumber(moduleitem.ModuleName,moduleitem.ModuleVersion);
                    Modules.Add(moduleitem);

                    DataRow modulerow = _moduletable.NewRow();
                    modulerow["modulenumber"] = moduleitem.ModuleNumber;
                    modulerow["softname"] = SoftName;
                    modulerow["softversion"] = SoftVersion;
                    modulerow["modulename"] = moduleitem.ModuleName;
                    modulerow["moduletype"] = GetModuleType(moduleitem.ModuleName);
                    modulerow["moduleversion"] = moduleitem.ModuleVersion;
                    modulerow["modulesourcepath"] = moduleitem.ModuleSourcePath;
                    modulerow["moduleentity"] = moduleitem.ModuleEntity;                 

                    _moduletable.Rows.Add(modulerow);
                }
                catch
                {
                }

            }
         

        }
        public void LoadSoftFromFileForUpdate(string softpath)
        {
            DataRow softdatarow = _softtable.NewRow();
            softdatarow["softnumber"] = SoftNumber;
            softdatarow["softname"] = SoftName;
            softdatarow["softversion"] = SoftVersion;
            softdatarow["updatedate"] = DateTime.Now.Date.ToShortDateString();
            softdatarow["filefilter"] = FileFilter;
            softdatarow["comment"] = Comment;

            _softtable.Rows.Add(softdatarow);

            string[] modules = GetModuleFilesPath(softpath);
            int moduleindex = 0;
            foreach (string modulepath in modules)
            {
                if (FileFilter.IndexOf(Path.GetExtension(modulepath)) < 0)
                    continue;
                try
                {
                    ModuleItem moduleitem = new ModuleItem();
                    moduleitem.ModuleVersion = GetVersionName(modulepath);
                    moduleitem.SoftName = SoftName;
                    moduleitem.ModuleType = GetModuleType(moduleitem.ModuleName);
                    moduleitem.SoftVersion = this.SoftVersion;
                    moduleitem.ModuleSourcePath = Path.GetFullPath(modulepath).Remove(0,softpath.Length);
                    moduleitem.ModuleName = Path.GetFileName(modulepath);
                    FileStream fs = new FileStream(modulepath, FileMode.Open);
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, (int)fs.Length);
                    moduleitem.ModuleEntity = CommonUtil.ByteArrayToString(bytes);
                    moduleitem.ModuleNumber = GetUpdateModuleNumber(moduleitem.ModuleName, moduleitem.ModuleVersion) + moduleindex++;
                    Modules.Add(moduleitem);

                    DataRow modulerow = _moduletable.NewRow();
                    modulerow["modulenumber"] = moduleitem.ModuleNumber;
                    modulerow["softname"] = SoftName;
                    modulerow["softversion"] = SoftVersion;
                    modulerow["modulename"] = moduleitem.ModuleName;
                    modulerow["moduletype"] = GetModuleType(moduleitem.ModuleName);
                    modulerow["moduleversion"] = moduleitem.ModuleVersion;
                    modulerow["modulesourcepath"] = moduleitem.ModuleSourcePath;
                    modulerow["moduleentity"] = moduleitem.ModuleEntity;

                    _moduletable.Rows.Add(modulerow);
                }
                catch
                {
                }
            }
        }

       static string   _sSourceDbConnectionID = "";

        public static  string SSourceDbConnectionID
        {
            get { return _sSourceDbConnectionID; }
            set { _sSourceDbConnectionID = value; }
        }
        static string   sSourceCreateTableType = "PostgreSQL";

        public static  string SSourceCreateTableType
        {
            get { return sSourceCreateTableType; }
            set { sSourceCreateTableType = value; }
        }

       static  IDatabaseReaderWriter _dbReader = null;

        public static IDatabaseReaderWriter DbReader
        {
            get
            {
                _dbReader = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter(SSourceCreateTableType, SSourceDbConnectionID);
                return _dbReader; 
            }
            set { _dbReader = value; }
        }

        public static string GetSoftNewestVersion(string softname)
        {
            string softversion = "";
            string selectsql = string.Format("select softversion from {0} where softname = '{1}' order by softversion desc", "软件列表", softname);
            softversion = DbReader.GetScalar(selectsql) as string;
            return softversion;
        }

        public static string GetSoftNumber(string softname, string version)
        {
            string softnumber = "";
            string selectsql = string.Format("select softnumber from {0} where softname = '{1}' and softversion = '{2}'", "软件列表", softname, version);
            softnumber = DbReader.GetScalar(selectsql) as string;
            return softnumber;
        }
        public static string GetModuleNumber(string modulename, string version)
        {
            string modulenumber = "";
            string selectsql = string.Format("select modulenumber from {0} where modulename = '{1}' and moduleversion = '{2}'", "模块列表", modulename, version);
            modulenumber = DbReader.GetScalar(selectsql) as string;

            return modulenumber;
        }

        public static string GetUpdateSoftNumber(string softname, string version)
        {
            string softnumber = "";
            string selectsql = string.Format("select softnumber from {0} where softname = '{1}' and softversion = '{2}'", "软件列表", softname, version);
            softnumber = DbReader.GetScalar(selectsql) as string;
            if (softnumber == null)
            {
                string selectmaxsql = string.Format("select max(cast(softnumber as integer)) from {0} ", "软件列表");
                int snumber = Convert.IsDBNull(DbReader.GetScalar(selectmaxsql)) ? 0 : Convert.ToInt32(DbReader.GetScalar(selectmaxsql));
                softnumber = Convert.ToString(snumber + 1);
            }
            return softnumber;
        }
        public static string GetUpdateModuleNumber(string modulename, string version)
        {
            string modulenumber = "";
            string selectsql = string.Format("select modulenumber from {0} where modulename = '{1}' and moduleversion = '{2}'", "模块列表", modulename, version);
            modulenumber = DbReader.GetScalar(selectsql) as string;
            if (modulenumber == null)
            {
                string selectmaxsql = string.Format("select max(cast(modulenumber as integer)) from {0} ", "模块列表");
                int snumber = Convert.IsDBNull(DbReader.GetScalar(selectmaxsql)) ? 0 : Convert.ToInt32(DbReader.GetScalar(selectmaxsql));
                modulenumber = Convert.ToString(snumber + 1);
            }
            return modulenumber;
        }
        
        public void  LoadSoftFromDatabase(string databasetype,string databaseid,string softname,string version)
        {

            string selectsql = string.Format("select * from {0} where softname = '{1}' and softversion = '{2}'", "软件列表", softname,version);
            _softtable = DbReader.GetDataTableBySQL(selectsql);


            selectsql = string.Format("select * from {0} where softversion = '{1}' and softname ='{2}'", "模块列表", version, softname);
            _moduletable = DbReader.GetDataTableBySQL(selectsql);

            LoadSoftFromDatabase(_softtable, _moduletable);

        }
        public void LoadSoftFromDatabase(DataTable softtable  ,DataTable moduletable)
        {
            _softtable = softtable;
            _moduletable = moduletable;
            if (_softtable.Rows.Count == 0) return;

            DataRow softrow = _softtable.Rows[0];

            SoftNumber = Convert.ToString(softrow["softnumber"]);
            SoftName = Convert.ToString(softrow["softname"]);
            SoftVersion = Convert.ToString(softrow["softversion"]);
            UpdateDate = Convert.ToString(softrow["updatedate"]);
            FileFilter = Convert.ToString(softrow["filefilter"]);
            Comment = Convert.ToString(softrow["comment"]);

            Modules.Clear();
            foreach (DataRow dr in _moduletable.Rows)
            {
                ModuleItem moduleitem = new ModuleItem();
                moduleitem.ModuleNumber = Convert.ToString(dr["modulenumber"]);
                moduleitem.SoftName = Convert.ToString(dr["softname"]);
                moduleitem.SoftVersion = Convert.ToString(dr["softversion"]);
                moduleitem.ModuleName = Convert.ToString(dr["modulename"]);
                moduleitem.ModuleType = Convert.ToString(dr["moduletype"]);
                moduleitem.ModuleVersion = Convert.ToString(dr["moduleversion"]);
                moduleitem.ModuleSourcePath = Convert.ToString(dr["modulesourcepath"]);
                moduleitem.ModuleEntity = Convert.ToString(dr["moduleentity"]);

                Modules.Add(moduleitem);
            }

        }

        //版本比较分为含‘.’版本，‘/’日期比较
        //返回值，1大，-1小，0相等；
        public static  int VersionCompare(string localversion, string serverversion)
        {
            int beEqual = 0;
            if (localversion == "" || serverversion == "")
                return 0;
            if (localversion.IndexOf('.') > 0 && serverversion.IndexOf('.') > 0)
            {
                beEqual =  localversion.CompareTo(serverversion);

            }else if(localversion.IndexOf('/') > 0 && serverversion.IndexOf('/') > 0)
            {
                DateTime dtlocal = Convert.ToDateTime(localversion);
                DateTime dtserver = Convert.ToDateTime(serverversion);
                beEqual = dtlocal.CompareTo(dtserver);
            }
            return beEqual;
        }
        //根据服务器和本地程序中的模块列表，选出版本更新的模块列表，用作下载或者上载
        // this 代表localsoft
        public  DataTable CompareForDownLoad(SoftItem localsoft)
        {
            DataTable updatemoduletable = new DataTable();
            updatemoduletable.Columns.Add("模块序号");
            updatemoduletable.Columns.Add("更新模块");
            updatemoduletable.Columns.Add("现有版本");
            updatemoduletable.Columns.Add("更新版本");
            updatemoduletable.Columns.Add("存储路径");
            updatemoduletable.Columns.Add("文件实体");

            if (localsoft.SoftName != SoftName )//|| localsoft.SoftVersion != SoftVersion
                return updatemoduletable;
            //版本相同则求出差异部分作为更新部分，进行下载
            else if (localsoft.SoftName == SoftName )//&& localsoft.SoftVersion == SoftVersion
            {
                #region //1如果本地模块在服务器上也有，服务器中目标中比大小，将服务器中较新的版本添加到模块更新列表中
                DataTable localmoduletable = localsoft.ModuleTable.Clone();
                //遍历本地软件模块列表
                int indexer =0;
                foreach (DataRow dr in localsoft.ModuleTable.Rows)
                {
                    string loacalmname =  dr["modulename"] as string;
                    string localmversion = dr["moduleversion"] as string;
                    DataRow[] intarget = this.ModuleTable.Select(string.Format("modulename='{0}'", loacalmname), "moduleversion desc");               
                    if (intarget.Length >= 1)
                    {
                        string servermversion = intarget[0]["moduleversion"]as string;
                        if (VersionCompare(localmversion ,servermversion)==0)
                        {
                        }
                        else if (VersionCompare(localmversion, servermversion) < 0)
                        {
                            DataRow newrow = updatemoduletable.NewRow();
                            newrow["模块序号"] = ++indexer;
                            newrow[ "更新模块"]=loacalmname;
                            newrow["现有版本"]=localmversion;
                            newrow["更新版本"]=servermversion;
                            newrow["存储路径"]=intarget[0]["modulesourcepath"];
                            newrow["文件实体"] = intarget[0]["moduleentity"];
                            updatemoduletable.Rows.Add(newrow);
                        }
                    }
                }
                #endregion
                #region//另外下载服务器上本地没有的模块，作为新增处理；
                DataTable servermoduletable = ModuleTable;
                foreach (DataRow dr in servermoduletable.Rows)
                {
                    string servermname = dr["modulename"] as string;
                    string servermversion = dr["moduleversion"] as string;
                    DataRow[] intarget = localsoft.ModuleTable.Select(string.Format("modulename='{0}'", servermname), "moduleversion desc");
                    if (intarget.Length >= 1)
                    {
                        string localmversion = intarget[0]["moduleversion"] as string;
                        if (VersionCompare(localmversion, servermversion)==0)
                        {
                            //如果服务器模块与本地模块版本相同，忽略
                        }
                        else if (VersionCompare(localmversion, servermversion) < 0)
                        {
                            //如果已经添加到更新列表中则忽略
                            DataRow[] intarget2 = updatemoduletable.Select(string.Format("更新模块='{0}'", servermname), "更新版本 desc");
                            if (intarget2.Length > 0)
                            {

                            }
                        }
                    }
                    else if(intarget.Length==0)
                    {
                        //如果是新增模块，需要添加到更新模块列表中
                        DataRow newrow = updatemoduletable.NewRow();
                        newrow["模块序号"] = ++indexer;
                        newrow["更新模块"] = servermname;
                        newrow["现有版本"] = "";
                        newrow["更新版本"] = servermversion;
                        newrow["存储路径"] = dr["modulesourcepath"];
                        newrow["文件实体"] = dr["moduleentity"];
                        updatemoduletable.Rows.Add(newrow);
                    }
                }
                #endregion
            }
            return updatemoduletable;    
        }

        DataTable _softtable;
        public DataTable SoftTable { get{return _softtable;} }
        DataTable _moduletable;
        public DataTable ModuleTable { get { return _moduletable; } }

        public string SoftNumber { get; set; }
        public string SoftName { set; get; }
        public string SoftVersion { set; get; }
        public string UpdateDate { set; get; }
        public string FileFilter { set; get; }
        public string Comment { set; get; }
        public List<ModuleItem> Modules;


    }
}
