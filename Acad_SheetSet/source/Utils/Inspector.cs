namespace Acad_SheetSet.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Inspector
    {
        private static List<string> _errors = new List<string>();

        public static void Show()
        {
            if (_errors.Any())
                AcadHelper.ShowMessage(string.Join(Environment.NewLine, _errors));
        }

        public static void AddError(string error)
        {
            _errors.Add(error);
        }

        public static void Clear()
        {
            _errors = new List<string>();
        }
    }
}