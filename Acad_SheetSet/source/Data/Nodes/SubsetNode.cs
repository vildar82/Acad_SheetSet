using System.Collections.Generic;
using System.Collections.ObjectModel;
using ACSMCOMPONENTS20Lib;
using JetBrains.Annotations;
using static Acad_SheetSet.Data.SheetSetExt;

namespace Acad_SheetSet.Data.Nodes
{
    public class SubsetNode : BaseNode
    {
        [NotNull] private readonly AcSmSubset subset;

        public SubsetNode([NotNull] AcSmSubset subset)
        {
            this.subset = subset;
            Name = subset.GetName();
            Nodes = GetNodes(subset);
        }

        [NotNull]
        private static ObservableCollection<ISSNode> GetNodes([NotNull] AcSmSubset acSmSubset)
        {
            var nodes = new ObservableCollection<ISSNode>();
            foreach (var comp in SsToList(acSmSubset.GetSheetEnumerator(), e => e.Next()))
            {
                nodes.Add(SheetSet.GetNode(comp));
            }
            return nodes;
        }
    }
}