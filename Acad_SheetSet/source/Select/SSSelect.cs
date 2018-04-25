﻿using System.Collections.Generic;
using System.Linq;
using Acad_SheetSet.Data;
using Acad_SheetSet.Numeration;
using ACSMCOMPONENTS20Lib;
using JetBrains.Annotations;
using Microsoft.Win32;
using NetLib;
using NetLib.WPF;
using ReactiveUI;
using static Acad_SheetSet.Data.SheetSetExt;

namespace Acad_SheetSet.Select
{
    public class SSSelect : BaseModel
    {
        private AcSmSheetSetMgr mgr;

        public SSSelect(NumerationVM numerationVm) : base (numerationVm)
        {
            mgr = new AcSmSheetSetMgr();
            SheetSets = new ReactiveList<SheetSet>(GetSheetSets());
            SheetSet = SheetSets.FirstOrDefault();
            SelectFile = CreateCommand(SelectFileExec);
        }

        public SheetSet SheetSet { get; set; }

        public ReactiveList<SheetSet> SheetSets { get; set; }

        public ReactiveCommand SelectFile { get; set; }

        [NotNull]
        private IEnumerable<SheetSet> GetSheetSets()
        {
            return SsToList(mgr.GetDatabaseEnumerator(), e => e.Next()).SelectTry(s=> new SheetSet(s));
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
                AcSmDatabase ssDb= null;
                try
                {
                    ssDb = mgr.FindOpenDatabase(dialog.FileName);
                }
                catch
                {
                    //
                }
                if (ssDb != null)
                {
                    SheetSet = SheetSets.FirstOrDefault(s => s.File.EqualsIgnoreCase(dialog.FileName));
                }
                else
                {
                    ssDb =mgr.OpenDatabase(dialog.FileName);
                    var ss = new SheetSet(ssDb);
                    SheetSets.Add(ss);
                    SheetSet = ss;
                }
            }
        }
    }
}
