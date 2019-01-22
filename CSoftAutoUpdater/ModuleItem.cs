using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSoftAutoUpdater
{
    public class ModuleItem
    {
        public ModuleItem()
        {
        }

        public string ModuleNumber { set; get; }
        public string SoftName { set; get; }
        public string SoftVersion{set;get;}
        public string ModuleName{set;get;}
	    public string ModuleType{set;get;}
	    public string ModuleVersion{set;get;}
	    public string ModuleSourcePath{set;get;}
        public string ModuleEntity { set; get; }

    }
}
