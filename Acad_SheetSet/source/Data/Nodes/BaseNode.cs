// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 04 25 16:53

using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using NetLib.WPF;

namespace Acad_SheetSet.Data.Nodes
{
    public abstract class BaseNode : BaseModel, ISSNode
    {
        public string Name { get; set; }
        public ObservableCollection<ISSNode> Nodes { get; set; } = new ObservableCollection<ISSNode>();

        [NotNull]
        public List<SheetNode> GetSheets()
        {
            var sheets = new List<SheetNode>();
            if (this is SheetNode sheet)
                sheets.Add(sheet);
            else
            {
                foreach (var node in Nodes)
                    sheets.AddRange(node.GetSheets());
            }
            return sheets;
        }

        public bool IsExpanded { get; set; }
    }
}