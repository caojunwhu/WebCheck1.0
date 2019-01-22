
using System.Collections.Generic;
namespace Core
{ 
    public class Function
    {
        public Function()
        {
            Functions = new List<Function>();
            Configs = new List<Config>();
            RefDatabases = new List<RefDatabase>();
        }

        public List<Config> Configs { get; set; }
        public List<RefDatabase> RefDatabases { get; set; }
        public List<Function> Functions { get; set; }
        public string Method { get; set; }
        public string Tile { get; set; }
        public string Image { get; set; }
        public string Folder { get; set; }
        public string MainDll { get; set; }
        public string Class { get; set; }
        public string Paras { get; set; }
        public string Key { get; set; }
        public string ToolTip { get; set; }
    }
}
