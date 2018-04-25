using ACSMCOMPONENTS20Lib;
using JetBrains.Annotations;

namespace Acad_SheetSet.Data.Nodes
{
    public class SheetNode : BaseNode
    {
        [NotNull] private readonly AcSmSheet sheet;

        public SheetNode([NotNull] AcSmSheet sheet)
        {
            this.sheet = sheet;
            Name = sheet.GetName();
            Number = sheet.GetNumber();
            CrossNumber = sheet.GetCustomPropertyValue("Сквозной номер").ToString();
        }

        /// <summary>
        /// Номер листа
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// Сквозной номер
        /// </summary>
        public string CrossNumber { get; set; }
    }
}