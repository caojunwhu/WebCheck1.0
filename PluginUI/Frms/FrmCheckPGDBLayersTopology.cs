using DatabaseDesignPlus;
using DevComponents.DotNetBar.SuperGrid;
using DLGCheckLib;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PluginUI.Frms
{
    public partial class FrmCheckPGDBLayersTopology : Form
    {
        private IMap localMap;
        private AxMapControl localMapControl;
        public RelativeLineCollection localRelativeChecklines { set; get; }
        public DLGCheckProjectClass localCheckProject { set; get; }
        public SearchTargetSetting localSearchTargetSetting { set; get; }

        public string SMapnumber
        {
            
            get
            {
                _sMapnumber = localCheckProject.CurrentMapnumber;
                return _sMapnumber;
            }

            set
            {
                _sMapnumber = value;
            }
        }

        DataTable _QualityErrorTable;
        string SampleerrorCollectionTableName = "检查意见记录表";
        BatchCheckClass bcheckclass = null;

        string _sMapnumber;
        string _sprojectid;
        DataTable dtvalue = null;

        public FrmCheckPGDBLayersTopology(DLGCheckProjectClass GlobeCheckProject, SearchTargetSetting GlobeSearchtargetSetting, AxMapControl mapControl, int sampleareaidex = -1, int sampleserial = -1, string szMapnumber = "")
        {
            InitializeComponent();

            _QualityErrorTable = new DataTable();
            _QualityErrorTable.Columns.Add("质量元素");
            _QualityErrorTable.Columns.Add("质量子元素");
            _QualityErrorTable.Columns.Add("错漏类别");
            _QualityErrorTable.Columns.Add("错漏描述");
            _QualityErrorTable.Columns.Add("处理意见");
            _QualityErrorTable.Columns.Add("复查情况");
            _QualityErrorTable.Columns.Add("修改情况");
            _QualityErrorTable.Columns.Add("检查者");
            _QualityErrorTable.Columns.Add("检查日期");

            SMapnumber = GlobeCheckProject.CurrentMapnumber;
            _sprojectid = GlobeCheckProject.ProjectID;
            localMap = mapControl.Map;
            localCheckProject = GlobeCheckProject;
            localMapControl = mapControl;

            try
            {
                DatabaseDesignPlus.IDatabaseReaderWriter datareader = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", CallClass.Databases["SurveryProductCheckDatabase"]);
                string sql = string.Format("select * from {0} where projectid ='{1}'", datareader.GetTableName("基础地理信息数据拓扑检查设置表"), localCheckProject.ProjectID);
                dtvalue = datareader.GetDataTableBySQL(sql);

                if(dtvalue!=null)
                {
                    LoadTopologyCheckItems();
                }
            }catch(Exception ex)
            {

            }


        }

        private void SelectDefineFile_Click(object sender, EventArgs e)
        {
            //获取当前路径和文件名
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "数据分层属性定义文件(*.xls)|*.xls|All Files(*.*)|*.*";
            dlg.Title = "Open LayerAttributeDefinefile Data file";
            if(dlg.ShowDialog()==DialogResult.OK)
            {
                string strFullPath = dlg.FileName;
                textBox1.Text = strFullPath;
                DatabaseDesignPlus.IDatabaseReaderWriter datareader =
                      DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("Excel", strFullPath);

                List<string> tablename = datareader.GetSchameDataTableNames();
                DatabaseDesignPlus.DatabaseReaderWriterFactory.FillCombox(tablename, comboBox1);
                //DatabaseDesignPlus.DatabaseReaderWriterFactory.FillCombox(tablename, comboBox2);


            }
        }

        //分层属性表选择好以后，即可已进行分层定义设置读取
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(dtvalue == null)
            {
                string strFullPath = textBox1.Text;
                if (strFullPath == "" || comboBox1.Text == "")
                    return;

                DatabaseDesignPlus.IDatabaseReaderWriter datareader =
                      DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("Excel", strFullPath);

                dtvalue = datareader.GetDataTable(comboBox1.Text);

            }

            LoadTopologyCheckItems();

        }
        //LoadTopologyCheckItems
        private void LoadTopologyCheckItems()
        {
            //首先验证数据集下是否存在TOPO数据集，并且将所有图层装载到TOPO中，重命名形式：原图层名+“_”
            if (localCheckProject.SampleFilePath == "" && 
                (localCheckProject.SampleFileFormat != "PGDB"||localCheckProject.SampleFileFormat != "FGDB"))
                return;

            IWorkspaceFactory pWorkspaceFactory;
            IWorkspace pWorkSpace;
            pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
            string TempFGDBPath = string.Format("{0}\\Template\\Default.gdb", Application.StartupPath);
            //清空File GDB中所有数据集，包括拓扑
            pWorkSpace = pWorkspaceFactory.OpenFromFile(TempFGDBPath, 0);

            DatabaseDesignPlus.IDatabaseReaderWriter datareader = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", CallClass.Databases["SurveryProductCheckDatabase"]);
            bcheckclass = new BatchCheckClass(dtvalue,  pWorkSpace, localMap, localMapControl, datareader, localCheckProject.ProjectID, SMapnumber);
            TopocheckRecordCollections topocheckrecordcollection = new TopocheckRecordCollections(localCheckProject.ProjectID, SMapnumber);
            topocheckrecordcollection.Read();

            superGridControl1.PrimaryGrid.DataSource = bcheckclass.checkItems;
            superGridControl2.PrimaryGrid.DataSource = topocheckrecordcollection.ResultList;
        }



        private void TopologyCheck()
        {
            if (bcheckclass == null)
            {
                MessageBox.Show("请设置拓扑检查项！");
                return;

            }
            DatabaseDesignPlus.IDatabaseReaderWriter datareader = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", CallClass.Databases["SurveryProductCheckDatabase"]);
            bcheckclass.Write(localCheckProject.ProjectID, datareader);

            IWorkspaceFactory pWorkspaceFactory;
            IWorkspace pWorkSpace;
            pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
            string TempFGDBPath = string.Format("{0}\\Template\\Default.gdb", Application.StartupPath);
            //清空File GDB中所有数据集，包括拓扑
            pWorkSpace = pWorkspaceFactory.OpenFromFile(TempFGDBPath, 0);
            IEnumDataset enumDataset = pWorkSpace.Datasets[esriDatasetType.esriDTAny];
            IDataset pDataset = enumDataset.Next();
            while (pDataset != null)
            {
                if (pDataset.Subsets != null)
                {
                    ITopologyContainer topologyContainer = pDataset as ITopologyContainer;
                    if (topologyContainer != null)
                    {
                        for (int i = 0; i < topologyContainer.TopologyCount; i++)
                        {
                            ITopology topology = topologyContainer.Topology[i];
                            IFeatureClassContainer topofeatures = topology as IFeatureClassContainer;
                            IEnumFeatureClass Temp_EnumFeatureClass = topofeatures.Classes;
                            //Temp_EnumFeatureClass.Reset();
                            IFeatureClass Temp_FeatureClass = Temp_EnumFeatureClass.Next();

                            while (Temp_FeatureClass != null)
                            {
                                topology.RemoveClass(Temp_FeatureClass);
                                Temp_FeatureClass = Temp_EnumFeatureClass.Next();
                            }
                            IDataset pTopoDs = topology as IDataset;
                            pTopoDs.Delete();
                        }

                    }
                }
                //删除母数据集
                if (pDataset != null) pDataset.Delete();

                pDataset = enumDataset.Next();
            }
            if (Directory.Exists(TempFGDBPath) == false)
                return;


            foreach (CheckItem ci in bcheckclass.checkItems)
            {
                if (ci.topocheckrecordcollection.ResultList.Count > 0)
                    ci.topocheckrecordcollection.ResultList.Clear();

                if (ci.TobeCheck == true)
                {
                  ci.Check(ci);
                    //导入拓扑检查记录
                  ci.topocheckrecordcollection.Write(localCheckProject.ProjectID, SMapnumber, localCheckProject.currentuser);
                    //导入检查意见记录表
                }
            }
            bcheckclass.WriteCheckProblemRecord(localCheckProject.ProjectID, SMapnumber, localCheckProject.currentuser);

            TopocheckRecordCollections topocheckrecordcollection = new TopocheckRecordCollections(localCheckProject.ProjectID, SMapnumber);
            topocheckrecordcollection.Read();

            superGridControl2.PrimaryGrid.DataSource = topocheckrecordcollection.ResultList;

            MessageBox.Show("检查完毕，请在检查结果表格中查看，如需定位，双击表格中的特定记录！");
        }


        private void SaveToDb()
        {
            /////////////更新到数据库
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string SampleerrorCollectionTableName = "检查意见记录表";
            //如果不存在表创建表，如果存在表，删除原有记录，插入新记录
            List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
            if (tableNames2.Contains(SampleerrorCollectionTableName) == false)
            {
                string sql_createTable = string.Format("create table {0}( ProjectID text,Mapnumber text,质量元素 text,质量子元素 text,错漏类别 text ,错漏描述 text ,处理意见 text, 修改情况 text, 复查情况 text,检查者 text,检查日期 text)", SampleerrorCollectionTableName);
                datareadwrite.ExecuteSQL(sql_createTable);
                //记录
            }
            //逐条记录到数据表SearchTargetSetting
            string sql_queryitem = string.Format("select Mapnumber from {0} where ProjectID = '{1}' and Mapnumber='{2}' ", SampleerrorCollectionTableName, _sprojectid, SMapnumber);
            object o = datareadwrite.GetScalar(sql_queryitem);
            if (o == null)//insert
            {
            }
            else // delete
            {
                string sql_delete = string.Format("delete from {0} where  ProjectID = '{1}' and Mapnumber='{2}' ", SampleerrorCollectionTableName, _sprojectid, SMapnumber);
                datareadwrite.ExecuteSQL(sql_delete);
            }


            //写入新的问题记录
            foreach (DataRow dr in _QualityErrorTable.Rows)
            {
                string sql_insert = string.Format("insert into {0} Values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", SampleerrorCollectionTableName, _sprojectid, SMapnumber,
                    dr["质量元素"] as string, dr["质量子元素"] as string, dr["错漏类别"] as string, dr["错漏描述"] as string, dr["处理意见"] as string, dr["修改情况"] as string, dr["复查情况"] as string, dr["检查者"] as string, dr["检查日期"] as string);
                datareadwrite.ExecuteSQL(sql_insert);
                //记录搜索到的目标
            }

            MessageBox.Show("提交数据库完毕！可以在打印检查意见记录表窗口中查看结果！");

        }
        //点击进行自动拓扑检查
        private void AutoCheckbutton1_Click_1(object sender, EventArgs e)
        {
            if (localMap.LayerCount == 0)
            {
                MessageBox.Show("请添加待检样本图幅！");
                return;
            }

            TopologyCheck();

        }
        private string GetTopoCheckFeatureType(string TopoCheckClassName)
        {
            string topocheckfeatueType = "All";

            DatabaseDesignPlus.IDatabaseReaderWriter datareader = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", CallClass.Databases["SurveryProductCheckDatabase"]);
            string sql = string.Format("select {0} from {1} where {2} ='{3}'",datareader.GetFieldString("FeatureType"), datareader.GetTableName("拓扑检查项注册表"), datareader.GetFieldString("ClassName"),TopoCheckClassName);
            topocheckfeatueType = datareader.GetScalar(sql) as string;
            return topocheckfeatueType;

        }
        //双击打开参数编辑器
        private void superGridControl1_CellDoubleClick(object sender, DevComponents.DotNetBar.SuperGrid.GridCellDoubleClickEventArgs e)
        {
            if (superGridControl1.PrimaryGrid.SelectedRowCount == 1)
            {
                SelectedElementCollection sel =
                superGridControl1.PrimaryGrid.GetSelectedRows();         
                int index = (sel[0] as GridRow).Index;

                CheckItem chkitem = bcheckclass.checkItems[index];

                string FeatureType = GetTopoCheckFeatureType(chkitem.ClassName) ;
                DLGCheckLib.Frms.FrmLayersChooser frm = new DLGCheckLib.Frms.FrmLayersChooser(FeatureType, localMap, chkitem.Paras);


                if (frm.ShowDialog()==DialogResult.OK)
                {
                    string selectLayers = frm.SelectedLayers;
                    if (selectLayers.Replace(',',' ').Length==1) return;
                    chkitem.Paras = selectLayers;
                }
            }
        }
        //双击拓扑质检结果，跳转到检验图形，并高亮显示
        private void superGridControl2_RowDoubleClick(object sender, GridRowDoubleClickEventArgs e)
        {
            if (localMap.LayerCount <= 0) return;

            if (superGridControl2.PrimaryGrid.SelectedRowCount == 1)
            {
                SelectedElementCollection sel =
                superGridControl2.PrimaryGrid.GetSelectedRows();
                int index = (sel[0] as GridRow).Index;

                TopocheckRecordCollections topocheckrecordcollection = new TopocheckRecordCollections(localCheckProject.ProjectID, SMapnumber);
                topocheckrecordcollection.Read();

                TopocheckRecord record = topocheckrecordcollection.ResultList[index];
                IFeatureLayer featurelayer= ArcGISHelper.GetFeatureLayerByName( record.FeatureClassA,localMap);
                if (featurelayer == null)
                    return;

                IFeatureClass pFeatureClass = featurelayer.FeatureClass;
                if (pFeatureClass == null)
                    return;

                IFeature featureA = ArcGISHelper.GetFeature(pFeatureClass, "OBJECTID", record.FeatureA);
                IGeometry pGeom = Utils.Converters.ConvertWKTToGeometry(record.GeomWkt);

                //localMap.MapScale = MapScalar == 0 ? localCheckProject.MapScale / 2.5 : MapScalar;

                IEnvelope envelope = localMapControl.Extent;
                envelope.CenterAt(pGeom.Envelope.LowerLeft);
                localMapControl.Extent = envelope;

                ////////////显示检测线
                try
                {
                    /*
                    ILine pLine = new LineClass();
                    pLine.FromPoint = item.StartPoint;
                    pLine.PutCoords(item.StartPoint, item.TargetPoint);
                    // QI到ISegment
                    ISegment pSegment = pLine as ISegment;
                    // 创建一个Path对象
                    ISegmentCollection pPath = new PathClass();
                    object o = Type.Missing;
                    // 通过Isegmentcoletcion接口为Path对象添加Segment对象
                    pPath.AddSegment(pSegment, ref o, ref o);
                    // 创建一个Polyline对象
                    IGeometryCollection pPolyline = new PolylineClass();
                    pPolyline.AddGeometry(pPath as IGeometry, ref o, ref o);
                    IPolyline pPLine = pPolyline as IPolyline;
                    */

                    localMapControl.FlashShape(pGeom);
                }
                catch (Exception ex)
                {

                }
            }
        }

        //
    }
}
