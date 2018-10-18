namespace Acad_SheetSet.Batch.Nodes
{
    using System.Collections.Generic;

    public class NodeFile : NodeBase
    {
        public List<NodeLayout> Nodes { get; set; } = new List<NodeLayout>();

        public NodeFile NextFile { get; set; }

        public bool NeedCloseFile { get; set; }
    }
}