using System.Diagnostics;
using Acad_SheetSet.Batch;
using Acad_SheetSet.Numeration;
using AcadLib;
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
#if DEBUG
            // Отключение отладочных сообщений биндинга (тормозит сильно)
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Off;
#endif
        }

        [CommandMethod(Group, nameof(PIK_SheetSetNumeration), CommandFlags.Session)]
        public static void PIK_SheetSetNumeration()
        {
            CommandStart.Start(d => new SSNumeration().Numeration());
        }

        [CommandMethod(Group, nameof(_InternalUse_SSBatchModal), CommandFlags.Modal)]
        public static void _InternalUse_SSBatchModal()
        {
            CommandStart.StartWoStat(BatchVM.InternalBatchModal);
        }

        [CommandMethod(Group, nameof(_InternalUse_SSBatchSession), CommandFlags.Session)]
        public static void _InternalUse_SSBatchSession()
        {
            CommandStart.StartWoStat(BatchVM.InternalBatchSession);
        }
    }
}