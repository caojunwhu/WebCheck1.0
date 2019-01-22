using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBaseDesign
{
    public class GPAttribute
    {
        public string Index { set; get; }
        public string AttributeName { set; get; }
        public string Alias { set; get; }
        public string AttributeType { set; get; }
        public string Length { set; get; }
        public string Precision { set; get; }
        public string Constration { set; get; }
        public string ValueField { set; get; }
    }
}
