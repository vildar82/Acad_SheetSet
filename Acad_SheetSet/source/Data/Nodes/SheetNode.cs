// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 04 25 16:37

using System;
#if v2016
using ACSMCOMPONENTS20Lib;
#elif v2017
using ACSMCOMPONENTS21Lib;
#elif v2018
using ACSMCOMPONENTS22Lib;
#endif
using JetBrains.Annotations;
using ReactiveUI;

namespace Acad_SheetSet.Data.Nodes
{
    public class SheetNode : BaseNode
    {
        [NotNull] public readonly AcSmSheet sheet;

        public SheetNode([NotNull] AcSmSheet sheet, [NotNull] SheetSet ss) : base(ss)
        {
            this.sheet = sheet;
            Name = sheet.GetTitle();
            Number = sheet.GetNumber();
            CrossNumber = sheet.GetCustomPropertyValue(ss.options.PropCrossNumberName)?.ToString();
            this.WhenAnyValue(v => v.NumberNew)
                .Subscribe(s => HasNewNumber = NumberNew != null && NumberNew != Number);
            this.WhenAnyValue(v => v.CrossNumberNew)
                .Subscribe(s => HasNewCrossNumber = CrossNumberNew != null && CrossNumberNew != CrossNumber);
            var layout = sheet.GetLayout();
            Layout = layout.GetName();
            File = layout.GetFileName();
        }

        public string Layout { get; set; }
        public string File { get; set; }
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
            sheet.SetCustomPropertyValue(ss.options.PropCrossNumberName, CrossNumberNew);
        }

        public void SetNumber()
        {
            sheet.SetNumber(NumberNew);
        }
    }
}