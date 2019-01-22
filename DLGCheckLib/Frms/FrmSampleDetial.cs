using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.SuperGrid;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;

namespace DLGCheckLib.Frms
{
    public partial class FrmSampleDetial : Form
    {
        DLGCheckProjectClass GlobleProject;
        SearchTargetSetting GlobeSearchtargetSetting;
        public FrmSampleDetial()
        {
            InitializeComponent();
        }
        public int SampleAreaIndex { get; set; }
        public int SampleSerial { get; set; }
        public string MapNumber { get; set; }
        ESRI.ArcGIS.Carto.IMap localMap = null;
        public FrmSampleDetial(DLGCheckProjectClass oProject, SearchTargetSetting oSearchtargetSetting=null, ESRI.ArcGIS.Carto.IMap map = null)
        {
            InitializeComponent();
            GlobleProject = oProject;
            GlobeSearchtargetSetting = oSearchtargetSetting;
            localMap = map;

            //GlobleProject.MapSampleSetting.OrderBy<MapSampleItemSetting>()
            
            superGridControl1.PrimaryGrid.DataSource = GlobleProject.MapSampleSetting;

        }

        public void  ShowSampleSetting()
        {
            //GlobleProject.ReadSampleDetail(GlobleProject.ProjectID);
            superGridControl1.PrimaryGrid.DataSource = GlobleProject.MapSampleSetting;
        }
        public void ShowSampleQuality()
        {
            //GlobleProject.ReadSampleDetail(GlobleProject.ProjectID);
            superGridControl1.PrimaryGrid.DataSource = GlobleProject.MapSampleQuality;
        }

        public void ShowSampleCheckState()
        {
            //GlobleProject.ReadSampleDetail(GlobleProject.ProjectID);
            superGridControl1.PrimaryGrid.DataSource = GlobleProject.MapSampleStation;
        }

        private void superGridControl1_DataBindingComplete(object sender, DevComponents.DotNetBar.SuperGrid.GridDataBindingCompleteEventArgs e)
        {
            if(superGridControl1.PrimaryGrid.DataSource is List<MapSampeItemQuality>)
            {
                List<MapSampeItemQuality> items = (superGridControl1.PrimaryGrid.DataSource as List<MapSampeItemQuality>);
                foreach (MapSampeItemQuality item in items)
                {
/*                    int index = items.IndexOf(item);
                    var cell = superGridControl1.PrimaryGrid.GetCell(index, 1);
                    var list = new List<string>();
                    list.Add(string.Format("MeanError:{0:N2}", item.PositionError.MeanError));
                    list.Add(string.Format("ErrorRatio:{0:N2}", item.PositionError.ErrorRatio));
                    list.Add(string.Format("ErrorScore:{0:N2}", item.PositionError.ErrorScore));
                    list.Add(string.Format("ErrorComment:{0:N2}", item.PositionError.ErrorComment));

                    cell.EditorType = typeof(MyComboBox);
                    cell.EditorParams = new object[] { list };

                    cell = superGridControl1.PrimaryGrid.GetCell(index, 2);
                    list = new List<string>();
                    list.Add(string.Format("MeanError:{0:N2}", item.HeightError.MeanError));
                    list.Add(string.Format("ErrorRatio:{0:N2}", item.HeightError.ErrorRatio));
                    list.Add(string.Format("ErrorScore:{0:N2}", item.HeightError.ErrorScore));
                    list.Add(string.Format("ErrorComment:{0:N2}", item.HeightError.ErrorComment));

                    cell.EditorType = typeof(MyComboBox);
                    cell.EditorParams = new object[] { list };


                    cell = superGridControl1.PrimaryGrid.GetCell(index, 3);
                    list = new List<string>();
                    list.Add(string.Format("MeanError:{0:N2}", item.CountourError.MeanError));
                    list.Add(string.Format("ErrorRatio:{0:N2}", item.CountourError.ErrorRatio));
                    list.Add(string.Format("ErrorScore:{0:N2}", item.CountourError.ErrorScore));
                    list.Add(string.Format("ErrorComment:{0:N2}", item.CountourError.ErrorComment));

                    cell.EditorType = typeof(MyComboBox);
                    cell.EditorParams = new object[] { list };

                    cell = superGridControl1.PrimaryGrid.GetCell(index, 4);
                    list = new List<string>();
                    list.Add(string.Format("MeanError:{0:N2}", item.RelativeError.MeanError));
                    list.Add(string.Format("ErrorRatio:{0:N2}", item.RelativeError.ErrorRatio));
                    list.Add(string.Format("ErrorScore:{0:N2}", item.RelativeError.ErrorScore));
                    list.Add(string.Format("ErrorComment:{0:N2}", item.RelativeError.ErrorComment));

                    cell.EditorType = typeof(MyComboBox);
                    cell.EditorParams = new object[] { list };
*/
                }
            }


        }
        public class MyComboBox : GridComboBoxExEditControl
        {
            public MyComboBox(object source)
            {
                DataSource = source;
                //DisplayMembers = "MeanError";
            }
        }
        //点击列表，选出图幅号
        private void superGridControl1_Click(object sender, EventArgs e)
        {
            if(superGridControl1.PrimaryGrid.SelectedRowCount==1)
            {
                SelectedElementCollection sel =
                superGridControl1.PrimaryGrid.GetSelectedRows();

                textBox1.Text = (sel[0] as GridRow).Cells[2].Value as string;
                MapNumber = textBox1.Text;

                SampleAreaIndex = Convert.ToInt32((sel[0] as GridRow).Cells[0].Value);
                SampleSerial = Convert.ToInt32((sel[0] as GridRow).Cells[1].Value);

            }
        }
        //跳转到平面高程检测平台
        private void JumpToPHCheckStationbutton1_Click(object sender, EventArgs e)
        {
            if(MapNumber != "")
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
                GlobleProject.JumpToPlainHeightCheckStation(SampleAreaIndex,SampleSerial,MapNumber);
            }
        }
        //跳转到间距边长检测平台
        private void JumpToRelativeCheckStationbutton2_Click(object sender, EventArgs e)
        {
            if(MapNumber != "")
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
                GlobleProject.JumpToRelativeCheckStation(SampleAreaIndex, SampleSerial, MapNumber);

            }
        }

        private void superGridControl1_DoubleClick(object sender, EventArgs e)
        {
            string samplepath = string.Format("{0}\\{1}.dwg", GlobleProject.SampleFilePath, MapNumber);
            LoadSample(samplepath);
            DLGCheckLib.Frms.FrmSetMapBindSearchParameters frm = new FrmSetMapBindSearchParameters(localMap, GlobleProject.ProjectID, MapNumber, GlobleProject.currentuser);
            if (frm.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void btn_EDITE_Click(object sender, EventArgs e)
        {
            DLGCheckLib.Frms.FrmSetMapBindSearchParameters frm = new FrmSetMapBindSearchParameters(localMap, GlobleProject.ProjectID, MapNumber, GlobleProject.currentuser);
            if (frm.ShowDialog() == DialogResult.OK)
            {

            }

        }

        private void btn_DELETE_Click(object sender, EventArgs e)
        {
            MapSampleItemSetting mapitem = GlobleProject.GetMapSampleItemSetting(textBox1.Text);
            if(GlobleProject.DeleteMapSampleItemSetting(mapitem)==true)
            {
                MessageBox.Show("成功删除！请重新打开项目以便参数生效。");
            }
        }
        //批量从文件夹中读取样本文件，批量设置参数：流水号、图幅号、结合表、比例尺、平面精度限差、高程精度限差等
        private void btn_batchSetting_Click(object sender, EventArgs e)
        {
            //browser the folder
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog()==DialogResult.OK)
            {
                string[] dwgfiles = Directory.GetFiles(fbd.SelectedPath,"*.dwg");
                Array.Sort(dwgfiles);
                if(dwgfiles.Length==0)
                {
                    MessageBox.Show("该目录下无样本文件");
                    return;
                }
                //get the first for the template
                string dwgfirst = dwgfiles[0];
                LoadSample(dwgfirst);
                FrmSetMapBindSearchParameters dlg = new FrmSetMapBindSearchParameters(localMap, GlobleProject.ProjectID, GlobleProject.CurrentMapnumber, GlobleProject.currentuser);
                MapSampleItemSetting lastMapsamplesetting;
                if (dlg.ShowDialog()==DialogResult.OK)
                {
                    lastMapsamplesetting = dlg.Mapsampleitem;
                    foreach(string dwgfile in dwgfiles)
                    {

                        string CurrentMapNumber = GetMapNumberFromPath(dwgfile);
                        if (CurrentMapNumber == lastMapsamplesetting.MapNumber)
                            continue;

                        //更新单位成果检验参数
                        MapSampleItemSetting currentMapsamplesetting = lastMapsamplesetting;
                        currentMapsamplesetting.MapNumber = CurrentMapNumber;
                        currentMapsamplesetting.SampleSerial = lastMapsamplesetting.SampleSerial + 1;

                        //DLGCheckLib.DLGCheckProjectClass localproject = new DLGCheckProjectClass(GlobleProject.ProjectID, GlobleProject.currentuser);
                        //localproject.ReadSampleSetting(localproject.ProjectID);

                        if (GlobleProject.UpdateMapSampleItemSetting(currentMapsamplesetting) == true)
                        {

                        }

                    }
                }

            }
        }

        private string GetMapNumberFromPath(string sSamplePath)
        {
            //获取当前路径和文件名
            string strFullPath = sSamplePath;
            if (strFullPath == "") return "";
            int Index = strFullPath.LastIndexOf("\\");
            string filePath = strFullPath.Substring(0, Index);
            string fileName = strFullPath.Substring(Index + 1);

            string sCurrentMapNumber = System.IO.Path.GetFileNameWithoutExtension(strFullPath);

            return sCurrentMapNumber;
        }
        private void LoadSample(string sSamplePath)
        {
            //判断：加载样本有效
            if (PluginUI.ArcGISHelper.GetFeatureLayerByName("Point", localMap) != null ||
                PluginUI.ArcGISHelper.GetFeatureLayerByName("Polyline", localMap) != null)
            {
                DialogResult dlgresult = MessageBox.Show("已经加载了样本！是否加载新样本？", "提示", MessageBoxButtons.YesNo);
                if (dlgresult == DialogResult.Yes)
                {
                    IFeatureLayer layer = PluginUI.ArcGISHelper.GetFeatureLayerByName("Point", localMap);
                    if (layer != null) localMap.DeleteLayer(layer);
                    layer = PluginUI.ArcGISHelper.GetFeatureLayerByName("Polyline", localMap);
                    if (layer != null) localMap.DeleteLayer(layer);
                    layer = PluginUI.ArcGISHelper.GetFeatureLayerByName("Annotation", localMap);
                    if (layer != null) localMap.DeleteLayer(layer);


                }
                else if (dlgresult == DialogResult.No)
                {

                    return;
                }
            }

            IEnvelope currentViewBox = null;
            IWorkspaceFactory pWorkspaceFactory;
            IFeatureWorkspace pFeatureWorkspace;
            IFeatureLayer pFeatureLayer;
            IFeatureDataset pFeatureDataset;

            string strFullPath = sSamplePath;
            //获取当前路径和文件名
            if (GlobleProject.SampleFilePath == null)
            {
                //获取当前路径和文件名
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.FileName = MapNumber;
                dlg.Filter = "CAD(*.dwg)|*.dwg|All Files(*.*)|*.*";
                dlg.Title = "Open CAD Data file";
                dlg.ShowDialog();
                strFullPath = dlg.FileName;
            }
            if (strFullPath == "") return;
            int Index = strFullPath.LastIndexOf("\\");
            string filePath = strFullPath.Substring(0, Index);
            string fileName = strFullPath.Substring(Index + 1);

            string sCurrentMapNumber = System.IO.Path.GetFileNameWithoutExtension(strFullPath);
            // GlobeProject.SampleFileFormat = "DWG";
            GlobleProject.CurrentMapnumber = sCurrentMapNumber;
            GlobleProject.SampleFilePath = filePath;

            //打开CAD数据集
            pWorkspaceFactory = new CadWorkspaceFactoryClass();
            pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFactory.OpenFromFile(filePath, 0);
            //打开一个要素集
            pFeatureDataset = pFeatureWorkspace.OpenFeatureDataset(fileName);
            //IFeaturClassContainer可以管理IFeatureDataset中的每个要素类   
            IFeatureClassContainer pFeatClassContainer = (IFeatureClassContainer)pFeatureDataset;
            //对CAD文件中的要素进行遍历处理 
            for (int i = 0; i < pFeatClassContainer.ClassCount; i++)
            {
                IFeatureClass pFeatClass = pFeatClassContainer.get_Class(i);
                string sLayerName = pFeatClass.AliasName;
                if (pFeatClass.FeatureType == esriFeatureType.esriFTCoverageAnnotation)
                {
                    //如果是注记，则添加注记层
                    pFeatureLayer = new CadAnnotationLayerClass();
                    pFeatureLayer.FeatureClass = pFeatClass;
                    pFeatureLayer.Name = pFeatClass.AliasName;
                    localMap.AddLayer(pFeatureLayer);
                }
                else//如果是点、线、面，则添加要素层
                {
                    pFeatureLayer = new FeatureLayerClass();
                    pFeatureLayer.Name = pFeatClass.AliasName;
                    // 暂不处理MultiPatch/Polygon图层
                    if (pFeatureLayer.Name == "MultiPatch" || pFeatureLayer.Name == "Polygon")
                        continue;

                    pFeatureLayer.FeatureClass = pFeatClass;
                    GlobeSearchtargetSetting.RenderSearchTargetLayer(pFeatureLayer);
                    localMap.AddLayer(pFeatureLayer);

                    currentViewBox = pFeatureLayer.AreaOfInterest;
                }
            }

        }
    }
}
