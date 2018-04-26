// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 04 25 16:37

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Acad_SheetSet.Data.Nodes
{
    public interface ISSNode
    {
        bool IsExpanded { get; set; }
        string Name { get; }
        ObservableCollection<ISSNode> Nodes { get; }
        List<SheetNode> GetSheets();
    }
}