namespace Acad_SheetSet.Options
{
    /// <summary>
    /// Interaction logic for SSOptions.xaml
    /// </summary>
    public partial class SSOptionsView
    {
        public SSOptionsView(SSOptionsVM vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
