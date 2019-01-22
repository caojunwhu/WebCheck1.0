using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using DatabaseDesignPlus;

namespace CSoftAutoUpdater
{
    public class SoftUpdate
    {

        string _sSourceDbConnectionID = "";

        public string SSourceDbConnectionID
        {
            get { return _sSourceDbConnectionID; }
            set { _sSourceDbConnectionID = value; }
        }
        string sSourceCreateTableType = "PostgreSQL";
        string _softname = "";
        public string SoftName
        {
            get { return _softname; }
        }

        string _softversion = "";
        public CSoftAutoUpdater.SoftItem serversoft;
        public List<CSoftAutoUpdater.SoftItem> localsofts;

        DataTable _softtable = null;
        public DataTable SoftTable
        {
            get { return _softtable; }
        }
        public SoftUpdate()
        {
        }
        public bool CheckUpdate(string softpath)
        {
            bool hasupdate = false;
                        //选出可有更新软件目录
            //从本地文件中挑选出exe后缀且名称为更新软件的信息进行本地软件信息填充
            string[] modules = SoftItem.GetModuleFilesPath(softpath);
            string filter = "*.exe";
            foreach (string modulepath in modules)
            {
                if (filter.IndexOf(Path.GetExtension(modulepath)) < 0)
                    continue;
                string localsoftversion = SoftItem.GetVersionName(modulepath);
                localsoftversion = SoftItem.GetMajorMinorVersion(localsoftversion);
                string softname = Path.GetFileNameWithoutExtension(modulepath);
                string softnumber = SoftItem.GetSoftNumber(softname, localsoftversion);

                string serversoftversion = SoftItem.GetSoftNewestVersion(softname);
                //比较软件版本
                if (softnumber == null || serversoftversion == null || SoftItem.VersionCompare(serversoftversion,localsoftversion) < 0)//无更新
                {
                    //如果找不到最新版本或者服务器版与本地版本一致，则无需更新
                    continue;
                }
                else
                {
                    _softname = Path.GetFileNameWithoutExtension(modulepath);
                    _softversion = SoftItem.GetMajorMinorVersion(SoftItem.GetVersionName(modulepath));
                    localsofts = new List<CSoftAutoUpdater.SoftItem>();
                    CSoftAutoUpdater.SoftItem localsoft = new SoftItem();

                    localsoft.SoftName = _softname;
                    localsoft.SoftVersion = SoftItem.GetMajorMinorVersion(_softversion);  
               
                    localsofts.Add(localsoft);
                    hasupdate = true;
                }
            }
            if (hasupdate == false)
                return hasupdate;

            IDatabaseReaderWriter dbReader = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter(sSourceCreateTableType, _sSourceDbConnectionID);
            //按版本进行排序
            string selectSoftList = string.Format("select * from {0} order by {1} desc", "软件列表", "softversion");
            _softtable = dbReader.GetDataTableBySQL(selectSoftList);
            

            DataTable newModules = GetUpdatedModules(softpath, _softname, _softversion);
            if (newModules.Rows.Count == 0)
                hasupdate = false;

            return hasupdate;
        }

        public SoftItem GetUpdatedSoftItem(string softname, string softversion)
        {
            string newestversion = SoftItem.GetSoftNewestVersion(softname);
            _softname = softname;
            _softversion = softversion;
            if (SoftItem.VersionCompare(softversion, newestversion) < 0)
            {
                _softversion = softversion = newestversion;
            }

            SoftItem serversoft = new CSoftAutoUpdater.SoftItem();
            serversoft.LoadSoftFromDatabase(sSourceCreateTableType, _sSourceDbConnectionID, softname, softversion);
            return serversoft;
        }
        void MakeDirectory(string MainPath, string SubPath)
        {
            string destdir = MainPath;
            string[] subpaths = SubPath.Split('\\');
            foreach (string sub in subpaths)
            {
                if (sub != "")
                {
                    destdir = "\\" + sub;
                    if (Directory.Exists(destdir) == false)
                    {
                        Directory.CreateDirectory(destdir);
                    }

                }
            }
        }

        public bool Update(DataTable newModuleTable,string mianpath)
        {
            foreach (DataRow dr in newModuleTable.Rows)
            {
                string loacalmname = dr["更新模块"] as string;
                string localpath = dr["存储路径"] as string;
                string moduleentity = dr["文件实体"] as string;
                byte[] bytes = DatabaseDesignPlus.CommonUtil.HexStringToBytes(moduleentity);

                MakeDirectory(mianpath, localpath);
                string fullpath = mianpath + localpath;
                FileStream fs = File.Open(fullpath, FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();

            } return true;

            
        }

        public DataTable GetUpdatedModules(string mainpath,string ssoftname,string ssoftversion)
        {
            DataTable newModules = null;

            if (localsofts.Count >= 1)
            {
                for (int i = 0; i < localsofts.Count; i++)
                {
                    string softname = localsofts[i].SoftName;
                    string softversion = localsofts[i].SoftVersion;
 
                    if (ssoftname != softname )//|| softversion != ssoftversion
                        continue;

                    serversoft = GetUpdatedSoftItem(softname, SoftItem.GetMajorMinorVersion(softversion));

                    SoftItem localsoft = null;
                    //如果本地没有该软件,返回服务器软件列表，按版本号倒序排列
                    if (localsofts.Count == 0)
                    {
                        localsoft = new CSoftAutoUpdater.SoftItem();
                        localsoft.SoftName = softname;
                        localsoft.SoftVersion = SoftItem.GetMajorMinorVersion(softversion);
                        break;
                    }
                    else
                    {
                        foreach (SoftItem sf in localsofts)
                        {
                            if (sf.SoftName == softname)
                                localsoft = sf;
                        }
                    }
                    localsoft.FileFilter = serversoft.FileFilter;
                    localsoft.LoadSoftFromFile(mainpath);
                    CSoftAutoUpdater.SoftItem newsoft = new CSoftAutoUpdater.SoftItem();
                    newModules = serversoft.CompareForDownLoad(localsoft);
                }
            }
            return newModules;
        }
    }
}
