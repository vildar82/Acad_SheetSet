namespace Acad_SheetSet.Options
{
    using NetLib;
    using NetLib.WPF;
    using Path = AcadLib.IO.Path;

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
                    PropCrossNumberName = "СквознойНомер"
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
