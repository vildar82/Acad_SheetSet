using System.ComponentModel;

namespace Acad_SheetSet.Props
{
    public class SSProp
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public PropType Type { get; set; }
    }

    public enum PropType
    {
        /// <summary>
        /// Подшивка
        /// </summary>
        [Description("Подшивка")]
        SheetSet,
        /// <summary>
        /// Лист
        /// </summary>
        [Description("Лист")]
        Sheet
    }
}
