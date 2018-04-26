using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.IO;
using NetLib;
using NetLib.WPF;

namespace Acad_SheetSet.Options
{
    public class SSOptionsVM : BaseViewModel
    {
        private FileData<SSOptions> data;

        public SSOptionsVM()
        {
            var serverFile = Path.GetSharedCommonFile("SheetSet", "SheetSetOptions");
            var localFile = Path.GetUserPluginFile("SheetSet", "SheetSetOptions");
            data = new FileData<SSOptions>(serverFile,localFile, false);
            data.TryLoad();
            if (data.Data == null)
            {
                data.Data = new SSOptions
                {
                    PropCrossNumberName = "Сквозной номер"
                };
            }
            Options = data.Data;
        }

        public SSOptions Options { get; set; }

        public void Save()
        {
            data.Save();
        }
    }
}
