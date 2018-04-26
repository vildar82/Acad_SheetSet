// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 04 25 15:14

using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using AcadLib.Errors;
using Acad_SheetSet.Data;
using Acad_SheetSet.Data.Nodes;
using Acad_SheetSet.Options;
using Acad_SheetSet.Props;
using Acad_SheetSet.Select;
using JetBrains.Annotations;
using MahApps.Metro.IconPacks;
using NetLib.WPF;
using ReactiveUI;

namespace Acad_SheetSet.Numeration
{
    public class NumerationVM : BaseViewModel
    {
        public bool ExpandTreeView { get; set; } = true;

        public NumerationVM()
        {
            Options = new SSOptionsVM();
            Select = new SSSelect(this);
            Update = CreateCommand(() => UpdateExec(Select.SheetSet, true));
            Numeration = CreateCommand(() => UpdateExec(Select.SheetSet, false));
            Collapse = CreateCommand(() => ExpandTreeView = false);
            Expand = CreateCommand(() => ExpandTreeView = true);
            PropsVM = new PropsVM(this);
        }

        public bool IsSelected { get; set; }
        public SSOptionsVM Options { get; set; }

        public ReactiveCommand Collapse { get; set; }

        public ReactiveCommand Expand { get; set; }

        public ObservableCollection<ISSNode> Nodes { get; set; }

        public ReactiveCommand Numeration { get; set; }

        public SSSelect Select { get; set; }
        public ReactiveCommand Update { get; set; }

        public bool IsBimUser { get; set; } = AcadLib.General.IsBimUser;

        public PropsVM PropsVM { get; set; }

        public override void OnInitialize()
        {
            var ssSelected = Select.WhenAnyValue(v => v.SheetSet);
            ssSelected.Subscribe(s=>
            {
                IsSelected = s != null;
                UpdateExec(s, true);
            });
            if (AcadLib.General.IsBimUser)
            {
                AddWindowButton("Настройки", new PackIconMaterial {Kind = PackIconMaterialKind.Settings}, ShowOptions);
            }
        }

        private void ShowOptions()
        {
            var optionsView = new SSOptionsView(Options);
            optionsView.ShowDialog();
            Options.Save();
        }

        public void UpdateExec([CanBeNull] SheetSet ss, bool previewOnly)
        {
            Nodes = new ObservableCollection<ISSNode>();
            if (ss == null) return;
            ss.Numeration(previewOnly);
            Nodes = ss.Nodes;
            PropsVM.SSProps = ss.Props;
            Inspector.Show();
        }
    }
}