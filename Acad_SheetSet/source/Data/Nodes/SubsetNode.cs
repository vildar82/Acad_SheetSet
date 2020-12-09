namespace Acad_SheetSet.Data.Nodes
{
    using System.Collections.ObjectModel;
#if v2017
    using ACSMCOMPONENTS21Lib;
#elif v2019
    using ACSMCOMPONENTS23Lib;
#endif
    using static SheetSetExt;

    public class SubsetNode : BaseNode
    {
        private readonly AcSmSubset subset;

        public SubsetNode(AcSmSubset subset, SheetSet ss)
            : base (ss)
        {
            this.subset = subset;
            Name = subset.GetName();
            Nodes = GetNodes();
        }

        private ObservableCollection<ISSNode> GetNodes()
        {
            var nodes = new ObservableCollection<ISSNode>();
            foreach (var comp in SsToList(subset.GetSheetEnumerator(), e => e.Next()))
            {
                nodes.Add(ss.GetNode(comp));
            }

            return nodes;
        }
    }
}
