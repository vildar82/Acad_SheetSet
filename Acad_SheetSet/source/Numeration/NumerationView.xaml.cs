namespace Acad_SheetSet.Numeration
{
    using System;
    using System.Windows.Controls;
    using MahApps.Metro.IconPacks;
    using Pik.Metro.Controls;

    /// <summary>
    ///     Interaction logic for NumerationView.xaml
    /// </summary>
    public partial class NumerationView
    {
        public NumerationView()
        {
            InitializeComponent();
        }

        public void AddWindowButton(string toolTip, PackIconBase icon, Action onClick)
        {
            var button = new Button
            {
                Content = icon,
                ToolTip = toolTip
            };

            button.Click += (o, s) => onClick();
            AddWindowButton(button);
        }

        private void AddWindowButton(object button)
        {
            RightWindowCommands ??= new WindowCommands();
            RightWindowCommands.Items.Add(button);
        }
    }
}