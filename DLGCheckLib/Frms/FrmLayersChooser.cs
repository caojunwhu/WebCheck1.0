using DLGCheckLib;
using ESRI.ArcGIS.Carto;
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
    public partial class FrmLayersChooser : Form
    {
        string _FeatureType = "All";
        IMap _Map = null;
        public string SelectedLayers { get { return GetSelectedLayers(); } }

        private string GetSelectedLayers()
        {
            if (FilterLayers == null)
                return null;

            string selectlayers = "";
            foreach(SelectLayerPara slp in FilterLayers)
            {
                if (slp.Selected == false)
                    continue;
                selectlayers += slp.LayerName + ",";
            }
            return selectlayers;
        }

        List<SelectLayerPara> FilterLayers = null;
        public FrmLayersChooser(string FeatureType,IMap map,string InitParas)
        {
            InitializeComponent();

            _FeatureType = FeatureType;
            _Map = map;
            if(_Map!=null)
            {
                FilterLayers = new List<SelectLayerPara>();
                for (int i=0;i<_Map.LayerCount;i++)
                {
                    ILayer layer = _Map.Layer[i];
                    if(layer is IFeatureLayer)
                    {
                        if(_FeatureType=="All" || _FeatureType == GetLayerType(layer))
                        {
                            SelectLayerPara sl = new SelectLayerPara();
                            if(InitParas!=""||InitParas!="")
                            {
                                if (InitParas.IndexOf(layer.Name) >= 0)
                                    sl.Selected = true;
                            }else
                            sl.Selected = false;

                            sl.LayerName = layer.Name;
                             FilterLayers.Add(sl);
                        }
                    }
                }
                superGridControl1.PrimaryGrid.DataSource = FilterLayers;
            }
        }

         string GetLayerType(ILayer layer)
        {
            string Type="All";

            IFeatureLayer ftlayer = layer as IFeatureLayer;
            if(ftlayer!=null)
            {
                if(ftlayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
                {
                    Type = "Point";
                }
                else if (ftlayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                {
                    Type = "Polyline";
                }
                if (ftlayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
                {
                    Type = "Polygon";
                }
            }

            return Type;
        }

        private void FrmLayersChooser_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (FilterLayers == null) return;

            if(checkBox1.Checked==true)
            {
                foreach(SelectLayerPara slp in FilterLayers)
                {
                    slp.Selected = true;
                }
            }else
            {
                foreach (SelectLayerPara slp in FilterLayers)
                {
                    slp.Selected = false;
                }
            }
            superGridControl1.PrimaryGrid.DataSource = FilterLayers;
        }
    }
}
