using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLGCheckLib.Frms
{
    public partial class FrmSearchSetting : Form
    {
        public SearchTargetSetting localSearchtargetSetting { set; get; }
        string localporjectid = "";
        string localcurrentuser = "";
        public FrmSearchSetting(ESRI.ArcGIS.Carto.IMap map, SearchTargetSetting searctarget,string projectid,string curentuser)
        {
            InitializeComponent();
            localporjectid = projectid;
            LoadLayerInfo(map,searctarget);
            localcurrentuser = curentuser;
        }
        public void LoadLayerInfo(ESRI.ArcGIS.Carto.IMap map, SearchTargetSetting searctarget)
        {
            //如果是第一次刷新搜索目标，需要新建一个搜索目标，如过已经存在，直接应用
            if(searctarget == null||searctarget.DwglayerinfoList.Count==0)
            {
                localSearchtargetSetting = new SearchTargetSetting(map,localporjectid);
                searctarget = localSearchtargetSetting;
            }
            else
            {
                localSearchtargetSetting = searctarget;
            }            

            dataGridViewX1.DataSource = localSearchtargetSetting.DwglayerinfoList;
        }

        private void FrmSearchSetting_FormClosed(object sender, FormClosedEventArgs e)
        {
            DLGCheckLib.DLGCheckProjectClass localproject = new DLGCheckProjectClass(localporjectid, localcurrentuser);
            if(localcurrentuser!=localproject.owner)
            {
                MessageBox.Show("提示：您不是项目拥有者，不可以将搜索配置保存到数据库，但可以临时使用！");
            }
            else
            {
                //将配置记录存储到数据库中
                localSearchtargetSetting.Write();
                MessageBox.Show("提示：搜索配置已更新到数据库，搜索配置即时可用！");
            }
            this.DialogResult = DialogResult.OK;
        }
    }
}
