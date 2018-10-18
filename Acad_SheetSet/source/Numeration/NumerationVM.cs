namespace Acad_SheetSet.Numeration
{
    using System;
    using System.Collections.ObjectModel;
    using Batch;
    using Data;
    using Data.Nodes;
    using Options;
    using Props;
    using Select;
    using AcadLib.Errors;
    using JetBrains.Annotations;
    using MahApps.Metro.IconPacks;
    using NetLib.WPF;
    using ReactiveUI;

    public class NumerationVM : BaseViewModel
    {
        public NumerationVM()
        {
            Options = new SSOptionsVM();
            Select = new SSSelect(this);
            Update = CreateCommand(() => UpdateExec(Select.SheetSet, true));
            Numeration = CreateCommand(() => UpdateExec(Select.SheetSet, false));
            Collapse = CreateCommand(() => ExpandTreeView = false);
            Expand = CreateCommand(() => ExpandTreeView = true);
            PropsVM = new PropsVM(this);
            BatchVM = new BatchVM(this);
        }

        public bool ExpandTreeView { get; set; } = true;

        public bool IsSelected { get; set; }

        public SSOptionsVM Options { get; set; }

        public ReactiveCommand Collapse { get; set; }

        public ReactiveCommand Expand { get; set; }

        public ObservableCollection<ISSNode> Nodes { get; set; }

        public ReactiveCommand Numeration { get; set; }

        public bool HasCrossNumProp { get; set; }

        public SSSelect Select { get; set; }

        public ReactiveCommand Update { get; set; }

        public bool IsBimUser { get; set; } = AcadLib.General.IsBimUser;

        public PropsVM PropsVM { get; set; }

        public BatchVM BatchVM { get; set; }

        public override void OnClosed()
        {
            Options.Save();
        }

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
            if (ss == null)
            {
                return;
            }

            ss.Numeration(previewOnly);
            Nodes = ss.Nodes;
            PropsVM.SSProps = ss.Props;
            BatchVM.Update();
            Inspector.Show();
        }
    }
}