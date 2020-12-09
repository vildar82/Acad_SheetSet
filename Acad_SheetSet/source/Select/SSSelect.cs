namespace Acad_SheetSet.Select
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using Data;
    using MicroMvvm;
    using Microsoft.Win32;
    using Numeration;
    using Utils;
    using static Data.SheetSetExt;
#if v2017
    using ACSMCOMPONENTS21Lib;
#elif v2019
    using ACSMCOMPONENTS23Lib;
#endif

    public class SSSelect : ModelBase
    {
        private readonly NumerationVM model;
        private dynamic mgr;

        public SSSelect(NumerationVM model)
        {
            this.model = model;
            mgr = new AcSmSheetSetMgr();
            var sheetSets = GetSheetSets().ToList();
            SheetSets = new ObservableCollection<SheetSet>(sheetSets);
            SheetSet = sheetSets.FirstOrDefault();
            SelectFile = new RelayCommand(SelectFileExec);
        }

        public ICommand SelectFile { get; set; }

        public SheetSet SheetSet { get; set; }

        public ObservableCollection<SheetSet> SheetSets { get; set; }

        private IEnumerable<SheetSet> GetSheetSets()
        {
            return SsToList((IAcSmEnumDatabase)mgr.GetDatabaseEnumerator(), e => e.Next())
                .Select(s =>
                {
                    try
                    {
                        return new SheetSet(s, model.Options.Options);
                    }
                    catch
                    {
                        return null;
                    }
                }).Where(w => w != null);
        }

        private void SelectFileExec()
        {
            try
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
            catch (Exception ex)
            {
                ex.ShowMessage();
            }
        }
    }
}
