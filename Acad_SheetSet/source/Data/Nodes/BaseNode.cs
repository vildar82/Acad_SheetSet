using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Acad_SheetSet.Data.Nodes
{
    public abstract class BaseNode : ISSNode
    {
        public string Name { get; set; }
        public ObservableCollection<ISSNode> Nodes { get; set; } = new ObservableCollection<ISSNode>();
    }
}