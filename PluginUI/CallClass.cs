using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Authentication.Class;
//using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using GoogleMapDownLoader;
using DLGCheckLib;
using PluginUI.Frms;

namespace PluginUI
{
    public class CallClass
    {
        internal static string AppPath { get; private set; }
        internal static Dictionary<string, string> Configs { get; private set; }
        internal static Dictionary<string, string> Databases { get; private set; }
        public UserObject GlobleLoginUser;
        public DLGCheckLib.DLGCheckProjectClass GlobleProject;
        public DLGCheckLib.SearchTargetSetting GlobeSearchTargetSetting;

        AxMapControl _MapControl;
        public Form RunPython(Dictionary<string, string> configs, Dictionary<string, string> databases, string appPath, string paras, UserObject oLoginUser,AxMapControl map)
        {
            _MapControl = map;
            AppPath = appPath;
            Configs = new Dictionary<string, string>();
            Databases = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kv in configs)
            {
                Configs.Add(kv.Key, kv.Value);
            }
            //此处增加用户名
            GlobleLoginUser = oLoginUser;
            Configs.Add("LoginUserName", GlobleLoginUser.username);

            foreach (KeyValuePair<string, string> kv in databases)
            {
                Databases.Add(kv.Key, kv.Value);
            }
            ////////////////////////////////////////////////////////////////
            /*ScriptEngine engine= Python.CreateEngine();
            var scope = engine.CreateScope();
            scope.SetVariable("Ipy_this", this);
            string pythonfile = string.Format("{0}\\{1}", appPath, paras);
            scope.SetVariable("Map", _Map);
            ArcGISHelper arcgis = new ArcGISHelper();
            scope.SetVariable("arcgis", arcgis);
            ScriptSource code = engine.CreateScriptSourceFromFile(pythonfile);
            IFeatureClass featClass = code.Execute<IFeatureClass>(scope);

            Dictionary<string, object> options = new Dictionary<string, object>();
            options["Debug"] = true;
            ScriptRuntime pyRuntime = Python.CreateRuntime(options);
            dynamic scriptScope = pyRuntime.UseFile(pythonfile);
            scriptScope.SetVariable("Ipy_this", this);
            scriptScope.SetVariable("Map", _Map);
            //code.Execute(scope);
            ArcGISHelper arcgis = new ArcGISHelper();
            scriptScope.SetVariable("arcgis", arcgis);

            Form hideForm = new Form();
            IFeatureClass featClass = scriptScope.Execute<IFeatureClass>();

            //Object o = scriptScope.ShowMembers();
            //hideForm.Text = o.ToString();
            */

            Form hideForm = new Form();
            hideForm.FormClosed += mf_FormClosed;
            hideForm.Show();

            return hideForm;
        }

        public void testPython()
        {
            MessageBox.Show(GlobleLoginUser.ToString());
        }

        /// <summary>
        /// ///////////////调用一般的插入窗口
        /// </summary>
        /// <param name="configs"></param>
        /// <param name="databases"></param>
        /// <param name="appPath"></param>
        /// <param name="paras"></param>
        /// <param name="oLoginUser"></param>
        /// <returns></returns>
        public Form Run(Dictionary<string, string> configs, Dictionary<string, string> databases, string appPath, string paras, UserObject oLoginUser,DLGCheckLib.DLGCheckProjectClass oProject, SearchTargetSetting oSearchtargetSetting, AxMapControl map)
        {


            _MapControl = map;

            AppPath = appPath;
            Configs = new Dictionary<string, string>();
            Databases = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kv in configs)
            {
                Configs.Add(kv.Key, kv.Value);
            }
            //此处增加用户名
            GlobleLoginUser = oLoginUser;
            GlobleProject = oProject;
            GlobeSearchTargetSetting = oSearchtargetSetting;
            Configs.Add("LoginUserName", GlobleLoginUser.username);
            
            Configs.Add("ProjectID", GlobleProject.ProjectID);
            Configs.Add("ProjectName", GlobleProject.ProjectName);

            foreach (KeyValuePair<string, string> kv in databases)
            {
                Databases.Add(kv.Key, kv.Value);
            }

            Form callForm = null;

            if (paras == "位置精度检测项目信息入库" || paras == "平面及高程精度检测点成果入库" 
                || paras == "间距边长精度检测点成果表入库" || paras == "检测项目信息入库"
                || paras == "新建检测项目信息"                )
            {
                if(GlobleProject.currentuser!=GlobleProject.owner)
                {
                    MessageBox.Show("你无权操作该功能，请联系项目所有者：" + GlobleProject.owner);
                    return null;
                }
                callForm = new DataTableImportButtonForm(GlobleLoginUser,paras, GlobleProject.ProjectID); callForm.ShowDialog();

            }
            else if(paras == "基础地理信息数据分层属性定义入库"||
                paras== "基础地理信息数据分层属性枚举值入库"||
                paras == "基础地理信息数据拓扑检查设置入库")
            {
                if (GlobleProject.currentuser != GlobleProject.owner)
                {
                    MessageBox.Show("你无权操作该功能，请联系项目所有者：" + GlobleProject.owner);
                    return null;
                }
                if(GlobleProject.SampleFileFormat!="FGDB"&&GlobleProject.SampleFileFormat!="MDB")
                {
                    MessageBox.Show("该项目样本数据类型为非基础地理信息数据集，不可以检查属性结构及属性值！：" + GlobleProject.owner);
                    return null;
                }
                callForm = new DataTableImportButtonForm(GlobleLoginUser, paras, GlobleProject.ProjectID); callForm.ShowDialog();

            }
            else if(paras == "导入实测点文件")
            {
                if (GlobleProject.currentuser != GlobleProject.owner)
                {
                    MessageBox.Show("你无权操作该功能，请联系项目所有者：" + GlobleProject.owner);
                    return null;
                }
                callForm = new FrmUpdateScater(GlobleProject, paras,Databases["PGDatasourceConnectionID"]); callForm.ShowDialog();

            }
            else if (paras == "导入抽样分区文件")
            {
                if (GlobleProject.currentuser != GlobleProject.owner)
                {
                    MessageBox.Show("你无权操作该功能，请联系项目所有者：" + GlobleProject.owner);
                    return null;
                }
                callForm = new FrmUpdateSampleArea(GlobleProject, paras, Databases["PGDatasourceConnectionID"]); callForm.ShowDialog();
            }
            else if (paras == "导入结合表文件")
            {
                if (GlobleProject.currentuser != GlobleProject.owner)
                {
                    MessageBox.Show("你无权操作该功能，请联系项目所有者：" + GlobleProject.owner);
                    return null;
                }
                callForm = new FrmUpdateMapbindingTable(GlobleProject, paras, Databases["PGDatasourceConnectionID"]); callForm.ShowDialog();
            }
            else if(paras == "打印样本质量问题记录表")
            {
                callForm = new FrmQualityErrorReport(GlobleProject, paras); callForm.ShowDialog();
            }
            else  if (paras == "检测点精度统计表打印" || paras == "间距边长误差统计表打印" || paras == "检测中误差及得分计算" || 
                paras == "样本图幅检测精度统计表打印" )
            {
                callForm = new ReportPrintButtonForm(GlobleLoginUser,GlobleProject, paras); callForm.ShowDialog();
            }
            else if(paras == "加载样本结合图表")
            {
                callForm = new FrmOpenPGLayer(configs, databases, oLoginUser, map); callForm.ShowDialog();
            }
            else if(paras == "下载卫星影像")
            {
                callForm = new FrmDownloadGoogleMap(Databases["SurveryProductCheckDatabase"]); callForm.ShowDialog();
            }
            else if (paras == "查询监督检查单位信息")
            {
                callForm = new FrmJDCCCompanyInfo(configs, databases, oLoginUser, map); callForm.ShowDialog();
            }
            else if(paras == "搜索检测线")
            {
                if (GlobleProject.SampleFileFormat == "DWG" || GlobleProject.SampleFileFormat == "DOM")
                {
                    callForm = new Frms.FrmSearchCheckLines(GlobleProject, GlobeSearchTargetSetting, _MapControl);
                    callForm.Show();
                }else
                {
                    MessageBox.Show("暂不支持非DWG、DOM格式数据搜索检测线！");
                    return null;
                }
            }
            else if (paras == "间距边长测量")
            {
                if(GlobleProject.SampleFileFormat=="DWG")
                {
                    callForm = new Frms.FrmRelativeCheckLines(GlobleProject, GlobeSearchTargetSetting, _MapControl);
                    callForm.Show();
                }
                else
                {
                    MessageBox.Show("暂不支持非DWG格式数据间距边长测量！");
                    return null;
                }
            }
            else if(paras== "标注样本错漏")
            {
                if (GlobleProject.SampleFileFormat == "DWG")
                {
                    callForm = new Frms.FrmPintErrors(GlobleProject, GlobeSearchTargetSetting, _MapControl);
                    callForm.Show();
                }
                else
                {
                    MessageBox.Show("暂不支持非DWG格式数据标注错漏！");
                    return null;
                }
            }else if(paras == "样本质量评价")
            {
                if (GlobleProject.SampleFileFormat == "DWG")
                {
                    callForm = new Frms.FrmSampleEveluate(GlobleProject, GlobeSearchTargetSetting, _MapControl);
                    callForm.Show();
                }
                else
                {
                    MessageBox.Show("暂不支持非DWG格式数据质量评价！");
                    return null;
                }
            }
            else if(paras == "打开样本检测工作平台")
            {
                GlobleProject.ReadSampleCheckState(GlobleProject.ProjectID);
                callForm = new Frms.FrmSampleCheckStation(GlobleProject, GlobeSearchTargetSetting, _MapControl);
                callForm.Show();

            }
            else if (paras == "检查1:10000基础地理信息数据分层与属性定义")
            {
                if (GlobleProject.SampleFileFormat != "FGDB" && GlobleProject.SampleFileFormat != "MDB")
                {
                    MessageBox.Show("该项目样本数据类型为非基础地理信息数据集，不可以检查属性结构及属性值！：" + GlobleProject.owner);
                    return null;
                }
                GlobleProject.ReadSampleCheckState(GlobleProject.ProjectID);
                callForm = new Frms.FrmCheckPGDBLayersAtributes(GlobleProject, GlobeSearchTargetSetting, _MapControl);
                callForm.Show();

            }
            else if (paras == "检查1:10000基础地理信息数据图层拓扑关系")
            {
                if (GlobleProject.SampleFileFormat != "FGDB" && GlobleProject.SampleFileFormat != "MDB")
                {
                    MessageBox.Show("该项目样本数据类型为非基础地理信息数据集，不可以检查属性结构及属性值！：" + GlobleProject.owner);
                    return null;
                }
                GlobleProject.ReadSampleCheckState(GlobleProject.ProjectID);
                callForm = new Frms.FrmCheckPGDBLayersTopology(GlobleProject, GlobeSearchTargetSetting, _MapControl);
                callForm.Show();

            }
            callForm.TopMost = true;
            callForm.BringToFront();
            callForm.FormClosed += mf_FormClosed;

            return callForm;
        }


        void mf_FormClosed(object sender, FormClosedEventArgs e)
        {

            (sender as Form).Dispose();
        }


    }
}
