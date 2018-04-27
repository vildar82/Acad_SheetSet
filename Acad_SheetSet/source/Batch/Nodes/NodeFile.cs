using System.Collections.Generic;
using ReactiveUI;

namespace Acad_SheetSet.Batch.Nodes
{
    public class NodeFile : NodeBase
    {
        public List<NodeLayout> Nodes { get; set; } = new List<NodeLayout>();
        public NodeFile NextFile { get; set; }
        public bool NeedCloseFile { get; set; }
    }
}