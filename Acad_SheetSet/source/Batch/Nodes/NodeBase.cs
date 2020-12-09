namespace Acad_SheetSet.Batch.Nodes
{
    using System.Windows.Media;
    using MicroMvvm;

    public abstract class NodeBase : ModelBase
    {
        public string Name { get; set; }

        public bool ToBatch { get; set; } = true;        

        public bool IsExist { get; set; }

        public string BatchResult { get; set; }

        public Brush Color { get; set; }
    }
}
