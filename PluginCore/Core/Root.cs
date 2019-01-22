
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
namespace Core
{
    public class Root
    {
        public Root()
        {
            Functions = new List<Function>();
            Databases = new List<Database>();
        }

        public List<Function> Functions { get; set; }

        public List<Database> Databases { get; set; }

        public Function GetFunction(string title)
        {
            Function func = new Function();
            //嵌套查询
            //第一层
            foreach (Function f in Functions)
            {
                foreach (Function f0 in f.Functions)
                {
                    if (f0.Tile == title)
                        func = f0;
                }
            }
            return func;
        }

        public static Root FromXML(string filepath)
        {
            //var doc = XDocument.Load(Path.Combine(Application.StartupPath, "Functions.xml"));
            var doc = XDocument.Load(filepath);
            IEnumerable<XElement> eles = doc.Elements();
            IEnumerator enumerator = eles.GetEnumerator();
            enumerator.MoveNext();
            XElement root = enumerator.Current as XElement;

            Root xRoot = new Root();

            foreach (XElement ele in root.Elements())
            {
                if (ele.Name.LocalName.ToUpper() == "FUNCTIONS")
                {
                    foreach (XElement ele2 in ele.Elements())
                    {
                        InitFunction(xRoot, ele2);
                    }
                }
                else if (ele.Name.LocalName.ToUpper() == "DATABASES")
                {
                    foreach (XElement ele2 in ele.Elements())
                    {
                        InitDatabase(xRoot, ele2);
                    }
                }
            }

            return xRoot;
        }

        static void InitFunction(Root xRoot, XElement ele)
        {
             Function o = new Function();
            Type tp = typeof(Function);
            SetValue(tp, o, ele);
            SetFunctionNext(o, ele);
            xRoot.Functions.Add(o);
        }

        static void InitFunction(Function xFunc, XElement ele)
        {
            Function o = new Function();
            Type tp = typeof(Function);
            SetValue(tp, o, ele);
            SetFunctionNext(o, ele);
            xFunc.Functions.Add(o);
        }

        static void SetFunctionNext(Function func, XElement ele)
        {
            foreach (XElement ele2 in ele.Elements())
            {
                if (ele2.Name.LocalName.ToUpper() == "CONFIGS")
                {
                    foreach (XElement ele3 in ele2.Elements())
                    {
                        InitConfig(func, ele3);
                    }
                }
                else if (ele2.Name.LocalName.ToUpper() == "REFDATABASES")
                {
                    foreach (XElement ele3 in ele2.Elements())
                    {
                        InitRefDatabase(func, ele3);
                    }
                }
                else if (ele2.Name.LocalName.ToUpper() == "FUNCTION")
                {
                    InitFunction(func, ele2);
                }

            }
        }

        static void InitDatabase(Root xRoot, XElement ele)
        {
            Database o = new Database();
            Type tp = typeof(Database);
            SetValue(tp, o, ele);
            xRoot.Databases.Add(o);
        }
        static void InitRefDatabase(Function func, XElement ele)
        {
            RefDatabase o = new RefDatabase();
            Type tp = typeof(RefDatabase);
            SetValue(tp, o, ele);
            func.RefDatabases.Add(o);
        }
        static void InitConfig(Function func, XElement ele)
        {
            Config o = new Config();
            Type tp = typeof(Config);
            SetValue(tp, o, ele);
            func.Configs.Add(o);
        }

        static void SetValue(Type type, object o, XElement ele)
        {
            foreach (XAttribute attr in ele.Attributes())
            {
                var property = type.GetProperty(attr.Name.LocalName);
                if (property != null) property.SetValue(o, attr.Value, null);
            }
        }
    }
}
