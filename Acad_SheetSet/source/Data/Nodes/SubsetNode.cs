namespace Acad_SheetSet.Data.Nodes
{
    using System.Collections.ObjectModel;
#if v2016
    using ACSMCOMPONENTS20Lib;
#elif v2017
    using ACSMCOMPONENTS21Lib;
#elif v2018
    using ACSMCOMPONENTS22Lib;
#endif
    using JetBrains.Annotations;
    using static SheetSetExt;

    public class SubsetNode : BaseNode
    {
        [NotNull] private readonly AcSmSubset subset;

        public SubsetNode([NotNull] AcSmSubset subset, SheetSet ss)
            : base (ss)
        {
            this.subset = subset;
            Name = subset.GetName();
            Nodes = GetNodes();
        }

        [NotNull]
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