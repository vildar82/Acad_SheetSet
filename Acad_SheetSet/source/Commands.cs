﻿// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 04 25 14:45

using System.Diagnostics;
using AcadLib;
using Acad_SheetSet.Batch;
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

        [CommandMethod(Group, nameof(PIK_SheetSetNumeration), CommandFlags.Session)]
        public static void PIK_SheetSetNumeration()
        {
            CommandStart.Start(d => new SSNumeration().Numeration());
        }

        [CommandMethod(Group, nameof(_InternalUse_SheetSetBatch), CommandFlags.Modal)]
        public static void _InternalUse_SheetSetBatch()
        {
            CommandStart.StartWoStat(BatchVM.InternalBatch);
        }
    }
}