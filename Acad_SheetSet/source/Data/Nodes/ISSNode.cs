using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Acad_SheetSet.Data.Nodes
{
    public interface ISSNode
    {
        string Name { get; }
        ObservableCollection<ISSNode> Nodes { get; }
    }
}