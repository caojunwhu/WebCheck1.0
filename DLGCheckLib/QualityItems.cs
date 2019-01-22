using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLGCheckLib
{
    public class QualityItems
    {
        List<string> _QualityItemNames;
        public List<string> QualityItemNames {
            set { _QualityItemNames = value; }
            get
            {
                _QualityItemNames = new List<string>();
                foreach(QualityItem qitem in QualityItemList)
                {
                    if(_QualityItemNames.IndexOf(qitem.QualityItemName)<0)
                    {
                        _QualityItemNames.Add(qitem.QualityItemName);
                    }
                }
                return _QualityItemNames;
            }
        }
        public string QualityItemNameString
        {
            get
            {
                string _s="";
                foreach(string s in QualityItemNames)
                {
                    _s += s+ ";";
                }
                return _s;
            }
        }
        public string  QualityName { set; get; }
        public List<QualityItem> QualityItemList { set; get; }
        private Dictionary<string, string> _DicQItems;
        public int Count
        {
            get
            {
                if (_DicQItems != null)
                {

                    return count=_DicQItems.Count;
                }
                count = 0;
                _DicQItems = new Dictionary<string, string>();
                foreach (QualityItem qitem in this.QualityItemList)
                {
                    foreach (SubQualityItem sqitem in qitem.SubQualitys)
                    {
                        List<string> keyList = (from q in _DicQItems
                                                where q.Value == qitem.QualityItemName &&
                                                q.Key == sqitem.SubQualityItemName select q.Key).ToList<string>();
                        
                        if (keyList.Count<=0)
                        {
                            _DicQItems.Add(sqitem.SubQualityItemName,qitem.QualityItemName);
                            count++;
                        }
                    }
                }
                return count;
            }

            set
            {

                count = value;
            }
        }

        //用于存储质量子元素/质量元素键值对，质量子元素为键，质量元素为值，定义对应关系
        public Dictionary<string, string> DicQItems
        {
            get
            {
                if (_DicQItems != null) return _DicQItems;

                _DicQItems = new Dictionary<string, string>();
                foreach (QualityItem qitem in this.QualityItemList)
                {
                    foreach (SubQualityItem sqitem in qitem.SubQualitys)
                    {
                        List<string> keyList = (from q in _DicQItems
                                                where q.Value == qitem.QualityItemName &&
                              q.Key == sqitem.SubQualityItemName
                                                select q.Key).ToList<string>();

                        if (keyList.Count <= 0)
                        {
                            _DicQItems.Add(sqitem.SubQualityItemName, qitem.QualityItemName);
                            count++;
                        }
                    }
                }
                return _DicQItems;
            }

            set
            {
                _DicQItems = value;
            }
        }

        private int count;
        public string ToJson()
        {
            string json;
            StringWriter sw = new StringWriter();
            JsonTextWriter jw = new JsonTextWriter(sw);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(jw, this);
            json = sw.ToString();
            return json;
        }

        public static QualityItems FromJson(string json)
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            QualityItems qitems = (QualityItems)serializer.Deserialize(new JsonTextReader(sr), typeof(QualityItems));
            return qitems;
        }
    }
    public class QualityItem
    {
        public string QualityItemName;
        public List<SubQualityItem> SubQualitys { set; get; }
    }
    public class SubQualityItem
    {
        public string SubQualityItemName;
        public string CheckItem;

        //public List<QualityCheckItem> Checkitems;
    }
    //class QualityCheckItem
    //{
     //   public string CheckItem { set; get; }
    //}
}
