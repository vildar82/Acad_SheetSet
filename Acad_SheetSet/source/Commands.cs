using System.Diagnostics;
using AcadLib;
using Acad_SheetSet.Numeration;
using Autodesk.AutoCAD.Runtime;
using Commands = Acad_SheetSet.Commands;

[assembly: CommandClass(typeof(Commands))]

namespace Acad_SheetSet
{
    public static class Commands
    {
        private const string Group = AcadLib.Commands.Group;

        static Commands()
        {
#if DEBUG // Отключение отладочных сообщений биндинга (тормозит сильно)
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Off;
#endif
        }

        [CommandMethod(Group, nameof(PIK_SheetSetNumeration), CommandFlags.Modal)]
        public static void PIK_SheetSetNumeration()
        {
            CommandStart.Start(d => new SSNumeration().Numeration());
        }
    }
}
