using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Core;
using Authentication.Class;
using ESRI.ArcGIS.Controls;
using DLGCheckLib;
using SuperDog;

namespace Core
{
    public class MInvoke
    {
        static List<string> AssemblyPath = new List<string>();
        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string path = Path.Combine(Application.StartupPath, _function.Folder);
            path = Path.Combine(path, args.Name.Split(',')[0] + ".dll");
            Assembly assembly = GetAssembly(args);
            return assembly;
        }


        Function _function;
        Root _root;
        //string _LoginUserName = "";
        UserObject GlobleLoginUser;
        DLGCheckLib.DLGCheckProjectClass GlobleProject;
        SearchTargetSetting GlobeSearchTargetSetting;

        AxMapControl _Map;
        public MInvoke(Function function, Root root, UserObject oLoginUser,DLGCheckLib.DLGCheckProjectClass oProject, SearchTargetSetting oSearchtargetSetting, AxMapControl map)
        {
            _function = function;
            _root = root;
            //_LoginUserName = sLoginUserName;
            GlobleLoginUser = oLoginUser;
            GlobleProject = oProject;
            GlobeSearchTargetSetting = oSearchtargetSetting;
            _Map = map;
        }

        Assembly GetAssembly(ResolveEventArgs args)
        {
            string path = Path.Combine(Application.StartupPath, _function.Folder);

            path = path.ToUpper().ToUpper().Trim();
            if (!AssemblyPath.Contains(path)) AssemblyPath.Add(path);

            string dll = args.Name.Split(',')[0] + ".dll";
            foreach (string p in AssemblyPath)
            {
                string[] files = Directory.GetFiles(p, dll);
                foreach (string f in files)
                {
                    Assembly asm = Assembly.LoadFrom(f);
                    if (asm.FullName == args.Name)
                        return asm;
                }
            }
            return null;
        }

        public bool Run()
        {
            SuperDogChecker superdogchecker = new SuperDogChecker();
            //if (superdogchecker.DecryptString() != DogStatus.StatusOk)
           // {
                //MessageBox.Show("未找到软件授权狗！");
            //    return false;
            //}

            if (superdogchecker.CheckExpiredDate() != DogStatus.StatusOk)
            {
                MessageBox.Show("软件授权失败，请联系250922107@qq.com！");
                return false;
            }

            if (GlobleProject==null)
            {
                MessageBox.Show("请先打开或新建一个项目后操作！");
                return false;
            }
            AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            try
            {
                string path = Path.Combine(Application.StartupPath, _function.Folder);
                string dll = Path.Combine(path, _function.MainDll);

                Assembly assembly = Assembly.LoadFile(Path.Combine(path, _function.MainDll));
                string[] files = Directory.GetFiles(path, "*.dll");


                object o = assembly.CreateInstance(_function.Class);
                Type type = assembly.GetType(_function.Class);

                MethodInfo method = type.GetMethod(_function.Method);
                object para = _function.Paras;

                Dictionary<string, string> configs = new Dictionary<string, string>();
                Dictionary<string, string> databases = new Dictionary<string, string>();
                foreach (Config c in _function.Configs)
                {
                    configs.Add(c.Key, c.Value);
                }

                foreach (RefDatabase r in _function.RefDatabases)
                {
                    foreach (Database d in _root.Databases)
                    {
                        if (d.Key.ToUpper() == r.Key.ToUpper())
                        {
                            d.Value = DatabaseDesignPlus.DataBaseConfigs.RePlaceConfig(d.Value);
                            databases.Add(d.Key, d.Value.Replace("{*}", Application.StartupPath));
                            continue;
                        }
                    }
                }

                Form frm = method.Invoke(o, new object[] { configs, databases, path, para ,GlobleLoginUser,GlobleProject,GlobeSearchTargetSetting, _Map}) as Form;
                if (frm != null)
                {
                    frm.Text = _function.Tile;
                    frm.FormClosed += frm_FormClosed;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("调用此功能出错，有待后续按项目要求完善！系统提示：{0}",ex.Message), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        void frm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ApplicationClosed != null) ApplicationClosed(this, new EventArgs());
        }

        public event EventHandler ApplicationClosed;
    }
}
