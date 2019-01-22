using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.SuperGrid;
using DLGCheckLib;
using ESRI.ArcGIS.Controls;

namespace PluginUI.Frms
{
    public partial class FrmSampleCheckStation : Form
    {
        DLGCheckProjectClass GlobleProject;
        public FrmSampleCheckStation()
        {
            InitializeComponent();
        }
        public int SampleAreaIndex { get; set; }
        public int SampleSerial { get; set; }
        public string MapNumber { get; set; }
        public DLGCheckProjectClass localCheckProject { set; get; }
        public SearchTargetSetting localSearchTargetSetting { set; get; }
        private AxMapControl localmapControl;

        public FrmSampleCheckStation(DLGCheckProjectClass oProject,SearchTargetSetting GlobeSearchtargetSetting, AxMapControl mapControl)
        {
            InitializeComponent();
            GlobleProject = oProject;
            localCheckProject = oProject;
            localSearchTargetSetting = GlobeSearchtargetSetting;
            localmapControl = mapControl;

            //GlobleProject.MapSampleSetting.OrderBy<MapSampleItemSetting>()

            superGridControl1.PrimaryGrid.DataSource = GlobleProject.MapSampleStation;

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
                GlobleProject.JumpToPlainHeightCheckStation(SampleAreaIndex,SampleSerial,MapNumber);
                this.Hide();
            }
        }
        //跳转到间距边长检测平台
        private void JumpToRelativeCheckStationbutton2_Click(object sender, EventArgs e)
        {
            if(MapNumber != "")
            {
                this.DialogResult = DialogResult.OK;
                GlobleProject.JumpToRelativeCheckStation(SampleAreaIndex, SampleSerial, MapNumber);
                this.Hide();

            }
        }

        private void RefeshCheckStatebutton1_Click(object sender, EventArgs e)
        {
            GlobleProject.ReadSampleCheckState(GlobleProject.ProjectID);
            this.ShowSampleCheckState();
        }
        //遍历检测台每一行记录，提取分区、流水号、图幅号，查询到散点、加载检测图层，进行自动匹配，并提交到数据库中
        //这是对单幅操作的自动化集成，如果查询到该图幅有检测线记录，则认为该图幅经过了检测，自动化配准时不处理
        private void AutoMatchScaterbutton1_Click(object sender, EventArgs e)
        {
            GridItemsCollection rows = superGridControl1.PrimaryGrid.Rows;

            int index = 0;
            if (GlobleProject.ShowProgress != null)
            {
                GlobleProject.ShowProgress("SETMIN", 0);
                GlobleProject.ShowProgress("SETMAX", rows.Count);

            }
            foreach (GridElement row in rows)
            {
                GridRow gr = row as GridRow;
                MapNumber = gr.Cells[2].Value as string;
                SampleAreaIndex = Convert.ToInt32(gr.Cells[0].Value);
                SampleSerial = Convert.ToInt32(gr.Cells[1].Value);

                AutoMatchScater automatch = new AutoMatchScater(GlobleProject, localSearchTargetSetting, localmapControl, SampleAreaIndex, SampleSerial, MapNumber);
                //加载样本
                automatch.LoadDwgFile();
                //加载检测线
                automatch.LoadCheckLines();
                //自动匹配
                automatch.AutoMatch();
                //

                if (GlobleProject.ShowProgress != null)
                {
                    GlobleProject.ShowProgress("SETVALUE", ++index);
                    if (index == rows.Count)
                    {
                        GlobleProject.ShowProgress("SETVALUE", 0);
                    }
                }

            }

            //检测完成后，进行自动刷新
            GlobleProject.ReadSampleCheckState(GlobleProject.ProjectID);
            this.ShowSampleCheckState();

        }
    }
}
