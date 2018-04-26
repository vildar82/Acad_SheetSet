// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 04 25 16:37

using System;
using ACSMCOMPONENTS20Lib;
using JetBrains.Annotations;
using ReactiveUI;

namespace Acad_SheetSet.Data.Nodes
{
    public class SheetNode : BaseNode
    {
        public const string propCrossNumber = "Сквозной номер";
        [NotNull] private readonly AcSmSheet sheet;

        public SheetNode([NotNull] AcSmSheet sheet)
        {
            this.sheet = sheet;
            Name = sheet.GetTitle();
            Number = sheet.GetNumber();
            CrossNumber = sheet.GetCustomPropertyValue(propCrossNumber)?.ToString();
            this.WhenAnyValue(v => v.NumberNew)
                .Subscribe(s => HasNewNumber = NumberNew != null && NumberNew != Number);
            this.WhenAnyValue(v => v.CrossNumberNew)
                .Subscribe(s => HasNewCrossNumber = CrossNumberNew != null && CrossNumberNew != CrossNumber);
        }

        /// <summary>
        ///     Сквозной номер
        /// </summary>
        public string CrossNumber { get; set; }

        /// <summary>
        ///     Новый сквозной номер согласно нумерации
        /// </summary>
        public string CrossNumberNew { get; set; }

        public bool HasNewCrossNumber { get; set; }

        public bool HasNewNumber { get; set; }

        /// <summary>
        ///     Номер листа
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        ///     Новый номер листа согласно нумерации
        /// </summary>
        public string NumberNew { get; set; }

        public void SetCrossNumber()
        {
            sheet.SetCustomPropertyValue(propCrossNumber, CrossNumberNew);
        }

        public void SetNumber()
        {
            sheet.SetNumber(NumberNew);
        }
    }
}