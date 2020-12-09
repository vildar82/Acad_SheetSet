namespace Acad_SheetSet.Data.Nodes
{
    using System;
#if v2017
    using ACSMCOMPONENTS21Lib;
#elif v2019
    using ACSMCOMPONENTS23Lib;
#endif

    public class SheetNode : BaseNode
    {
        public readonly AcSmSheet sheet;
        private string _numberNew;
        private string _crossNumberNew;

        public SheetNode(AcSmSheet sheet, SheetSet ss)
            : base(ss)
        {
            this.sheet = sheet;
            Name = sheet.GetTitle();
            Number = sheet.GetNumber();
            CrossNumber = sheet.GetCustomPropertyValue(ss.options.PropCrossNumberName)?.ToString();
            dynamic layout = sheet.GetLayout();
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
        public string CrossNumberNew
        {
            get => _crossNumberNew;
            set
            {
                _crossNumberNew = value;
                HasNewCrossNumber = CrossNumberNew != null && CrossNumberNew != CrossNumber;
                RaisePropertyChanged();
            }
        }

        public bool HasNewCrossNumber { get; set; }

        public bool HasNewNumber { get; set; }

        /// <summary>
        ///     Номер листа
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        ///     Новый номер листа согласно нумерации
        /// </summary>
        public string NumberNew
        {
            get => _numberNew;
            set
            {
                _numberNew = value;
                HasNewNumber = NumberNew != null && NumberNew != Number;
                RaisePropertyChanged();
            }
        }

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
