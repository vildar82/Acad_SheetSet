using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Acad_SheetSet.Data;
using Acad_SheetSet.Data.Nodes;
using Acad_SheetSet.Select;
using Autodesk.AutoCAD.ApplicationServices;
using JetBrains.Annotations;
using NetLib.WPF;
using ReactiveUI;

namespace Acad_SheetSet.Numeration
{
    public class NumerationVM : BaseViewModel
    {
        public NumerationVM()
        {
            Select = new SSSelect(this);
            Select.WhenAnyValue(v => v.SheetSet).Subscribe(UpdateExec);
            Update = CreateCommand(() => UpdateExec(Select.SheetSet));
        }

        public SSSelect Select { get; set; }
        public ReactiveCommand Update { get; set; }

        public ObservableCollection<ISSNode> Nodes { get; set; }

        private void UpdateExec ([CanBeNull] SheetSet ss)
        {
            Nodes = new ObservableCollection<ISSNode>();
            if (ss == null) return;
            ss.Update();
            Nodes = ss.Nodes;
        }
    }
}
