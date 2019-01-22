using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections;

namespace DataBaseDesign
{
	public class GPDataDictionary
	{

        public GPDataset TheGPDataset { set; get; }

        public GPDataDictionary()
        {
            TheGPDataset = new GPDataset();
        }

        public static GPDataDictionary FromXML(string filepath)
        {
            //var doc = XDocument.Load(Path.Combine(Application.StartupPath, "Functions.xml"));
            var doc = XDocument.Load(filepath);
            IEnumerable<XElement> eles = doc.Elements();
            IEnumerator enumerator = eles.GetEnumerator();
            enumerator.MoveNext();
            XElement root = enumerator.Current as XElement;

            GPDataDictionary gpdict = new GPDataDictionary();
            gpdict.TheGPDataset.sDatasetName = root.FirstAttribute.Value;
            gpdict.TheGPDataset.sYSDM = root.LastAttribute.Value;

            foreach (XElement ele in root.Elements())
            {
                InitGPDataset(gpdict.TheGPDataset, ele);
            }

            return gpdict;
        }

        public static void InitGPDataset(GPDataset gpds, XElement ele)
        {
            GPLayer o = new GPLayer();
            Type tp = typeof(GPLayer);
            foreach (XElement ele2 in ele.Elements())
            {
                InitGPLayer(o, ele2);
            }
            gpds.GPLayers.Add(o);
        }


        public static void InitGPLayer(GPLayer gplayer, XElement ele)
        {
            //foreach (XElement ele2 in ele.Elements())
            {
                if (ele.Name == "LayerInfo")
                {
                    SetValue(typeof(GPLayerInfo), gplayer.LayerInfo, ele);
                }
                else if (ele.Name == "AttributeInfo")
                {
                    foreach (XElement ele3 in ele.Elements())
                    {
                        GPAttribute o = new GPAttribute();
                        SetValue(typeof(GPAttribute), o, ele3);
                        gplayer.Attributes.Add(o);
                    }
                }
                else if (ele.Name == "ElementInfo")
                {
                    foreach (XElement ele3 in ele.Elements())
                    {
                        GPElement o = new GPElement();
                        SetValue(typeof(GPElement), o, ele3);
                        gplayer.Elements.Add(o);
                    }
                }
            }
        }



        public static void SetValue(Type type, object o, XElement ele)
        {
            foreach (XAttribute attr in ele.Attributes())
            {
                var property = type.GetProperty(attr.Name.LocalName);
                if (property != null) property.SetValue(o, attr.Value, null);
            }
        }
	}
}
