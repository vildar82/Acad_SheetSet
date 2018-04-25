using Acad_SheetSet.Select;
using ACSMCOMPONENTS20Lib;
using Autodesk.AutoCAD.ApplicationServices;

namespace Acad_SheetSet.Numeration
{
    /// <summary>
    /// Нумерация листов подшивки
    /// </summary>
    public class SSNumeration
    {
        public void Numeration()
        {
            var vm = new NumerationVM();
            var view = new NumerationView(vm);
            view.Show();
        }
    }
}
