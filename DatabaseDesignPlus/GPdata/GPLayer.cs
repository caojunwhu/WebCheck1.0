using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBaseDesign
{
    public class GPLayer
    {
        public GPLayer()
        {
            LayerInfo = new GPLayerInfo();
            Attributes = new List<GPAttribute>();
            Elements = new List<GPElement>();
        }
        public GPLayerInfo LayerInfo { set; get; }
        public List<GPAttribute> Attributes;
        public List<GPElement> Elements;

    }

    public class GPLayerInfo
    {
        public string LayerName { set; get; }
        public string Alias { set; get; }
        public string Dataset { set; get; }
        public string LayerType { set; get; }
        public string LayerClass { set; get; }
        public string Remark { set; get; }
    }
}
