namespace Acad_SheetSet.Numeration
{
    /// <summary>
    ///     Нумерация листов подшивки
    /// </summary>
    public class SSNumeration
    {
        public void Numeration()
        {
            var view = new NumerationView();
            var vm = new NumerationVM(view);

            view.Show();
        }
    }
}