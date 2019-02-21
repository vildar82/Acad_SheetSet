namespace Acad_SheetSet.Props
{
    using System.ComponentModel;

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
