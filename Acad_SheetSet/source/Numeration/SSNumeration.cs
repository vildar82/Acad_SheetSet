// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 04 25 14:50

namespace Acad_SheetSet.Numeration
{
    /// <summary>
    ///     Нумерация листов подшивки
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