namespace Acad_SheetSet.Data.Nodes
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public interface ISSNode
    {
        bool IsExpanded { get; set; }

        string Name { get; }

        ObservableCollection<ISSNode> Nodes { get; }

        List<SheetNode> GetSheets();
    }
}