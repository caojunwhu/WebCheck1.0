using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBaseDesign
{
    public class GPDataset
    {
        public GPDataset()
        {
            GPLayers = new List<GPLayer>();
        }
        public string sDatasetName { set; get; }
        public string sYSDM{set;get;}
        public List<GPLayer> GPLayers { set;get; }
    }
}
