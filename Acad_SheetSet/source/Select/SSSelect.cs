namespace Acad_SheetSet.Select
{
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using JetBrains.Annotations;
    using Microsoft.Win32;
    using NetLib;
    using NetLib.WPF;
    using Numeration;
    using ReactiveUI;
    using ReactiveUI.Legacy;
    using static Data.SheetSetExt;
#if v2016
    using ACSMCOMPONENTS20Lib;
#elif v2017
    using ACSMCOMPONENTS21Lib;
#elif v2018
    using ACSMCOMPONENTS22Lib;
#endif

    public class SSSelect : BaseModel
    {
        private readonly NumerationVM model;
        private dynamic mgr;

        public SSSelect(NumerationVM model)
            : base(model)
        {
            this.model = model;
            mgr = new AcSmSheetSetMgr();
            SheetSets = new ReactiveList<SheetSet>(GetSheetSets());
            SheetSet = SheetSets.FirstOrDefault();
            SelectFile = CreateCommand(SelectFileExec);
        }

        public ReactiveCommand SelectFile { get; set; }

        public SheetSet SheetSet { get; set; }

        public ReactiveList<SheetSet> SheetSets { get; set; }

        [NotNull]
        private IEnumerable<SheetSet> GetSheetSets()
        {
            return SsToList((IAcSmEnumDatabase)mgr.GetDatabaseEnumerator(), e => e.Next())
                .SelectTry(s => new SheetSet(s, model.Options.Options));
        }

        private void SelectFileExec()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                Filter = "Подшивка (*.dst)|*.dst",
                Title = "Выбор файла подшивки"
            };
            if (dialog.ShowDialog() == true)
            {
                AcSmDatabase ssDb = null;
                try
                {
                    ssDb = mgr.FindOpenDatabase(dialog.FileName);
                }
                catch
                {
                    //
                }

                if (ssDb != null)
                    SheetSet = SheetSets.FirstOrDefault(s => s.File.EqualsIgnoreCase(dialog.FileName));
                else
                {
                    ssDb = mgr.OpenDatabase(dialog.FileName);
                    var ss = new SheetSet(ssDb, model.Options.Options);
                    SheetSets.Add(ss);
                    SheetSet = ss;
                }
            }
        }
    }
}