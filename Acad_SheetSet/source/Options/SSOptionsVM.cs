namespace Acad_SheetSet.Options
{
    using MicroMvvm;
    using Utils;

    public class SSOptionsVM : ViewModelBase
    {
        private LocalFileData<SSOptions> data;

        public SSOptionsVM()
        {
            var localFile = AcadHelper.GetUserPluginFile("SheetSetOptions");
            data = new LocalFileData<SSOptions>(localFile, false);
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
            data.TrySave();
        }
    }
}
