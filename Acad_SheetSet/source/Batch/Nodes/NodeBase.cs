using System.Windows.Media;
using NetLib.WPF;

namespace Acad_SheetSet.Batch.Nodes
{
    public abstract class NodeBase : BaseModel
    {
        public string Name { get; set; }
        public bool ToBatch { get; set; } = true;        
        public bool IsExist { get; set; }
        public string BatchResult { get; set; }
        public Brush Color { get; set; }
    }
}
