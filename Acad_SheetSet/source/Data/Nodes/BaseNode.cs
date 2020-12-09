namespace Acad_SheetSet.Data.Nodes
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using MicroMvvm;

    public abstract class BaseNode : ModelBase, ISSNode
    {
        protected readonly SheetSet ss;

        protected BaseNode(SheetSet ss)
        {
            this.ss = ss;
        }

        public string Name { get; set; }

        public ObservableCollection<ISSNode> Nodes { get; set; } = new ObservableCollection<ISSNode>();

        public List<SheetNode> GetSheets()
        {
            var sheets = new List<SheetNode>();
            if (this is SheetNode sheet)
            {
                sheets.Add(sheet);
            }
            else
            {
                foreach (var node in Nodes)
                {
                    sheets.AddRange(node.GetSheets());
                }
            }

            return sheets;
        }

        public bool IsExpanded { get; set; }
    }
}