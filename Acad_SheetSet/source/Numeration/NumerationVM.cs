namespace Acad_SheetSet.Numeration
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using Batch;
    using Data;
    using Data.Nodes;
    using Options;
    using Props;
    using Select;
    using MahApps.Metro.IconPacks;
    using MicroMvvm;
    using Newtonsoft.Json.Linq;
    using Utils;

    public class NumerationVM : ViewModelBase
    {
        public NumerationVM(NumerationView view)
        {
            Window = view;
            view.DataContext = this;
            Options = new SSOptionsVM();
            Select = new SSSelect(this);
            Update = new RelayCommand(() => UpdateExec(Select.SheetSet, true));
            Numeration = new RelayCommand(() => UpdateExec(Select.SheetSet, false));
            Collapse = new RelayCommand(() => ExpandTreeView = false);
            Expand = new RelayCommand(() => ExpandTreeView = true);
            PropsVM = new PropsVM(this);
            BatchVM = new BatchVM(this);

            Window.Initialized += OnInitialize;
            Window.Closed += OnClosed;
            OnInitialize(null, null);
        }

        public bool ExpandTreeView { get; set; } = true;

        public bool IsSelected { get; set; }

        public SSOptionsVM Options { get; set; }

        public ICommand Collapse { get; set; }

        public ICommand Expand { get; set; }

        public ObservableCollection<ISSNode> Nodes { get; set; }

        public ICommand Numeration { get; set; }

        public bool HasCrossNumProp { get; set; }

        public SSSelect Select { get; set; }

        public ICommand Update { get; set; }

        public PropsVM PropsVM { get; set; }

        public BatchVM BatchVM { get; set; }

        public NumerationView Window { get; set; }

        public void OnClosed(object sender, EventArgs e)
        {
            Options.Save();
        }

        private void OnInitialize(object sender, EventArgs e)
        {
            UpdateSelectedSheetSet();
            Select.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(SSSelect.SheetSet))
                {
                    UpdateSelectedSheetSet();
                }
            };

            Window.AddWindowButton("Настройки", new PackIconMaterial {Kind = PackIconMaterialKind.CogOutline}, ShowOptions);
        }

        private void UpdateSelectedSheetSet()
        {
            IsSelected = Select?.SheetSet != null;
            UpdateExec(Select?.SheetSet, true);
        }

        private void ShowOptions()
        {
            var optionsView = new SSOptionsView(Options);
            optionsView.ShowDialog();
            Options.Save();
        }

        public void UpdateExec(SheetSet ss, bool previewOnly)
        {
            Inspector.Clear();
            try
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
            }
            catch (Exception ex)
            {
                ex.ShowMessage();
            }

            Inspector.Show();
        }
    }
}