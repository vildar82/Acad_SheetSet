using Acad_SheetSet.Batch;
using Acad_SheetSet.Numeration;
using Autodesk.AutoCAD.Runtime;
using Commands = Acad_SheetSet.Commands;

[assembly: CommandClass(typeof(Commands))]

namespace Acad_SheetSet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Utils;

    public static class Commands
    {
        private const string Group = "PIK";
        private static List<DllResolve> _dllResolves;

        /// <summary>
        /// Конструктор
        /// </summary>
        static Commands()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        [CommandMethod(Group, nameof(SheetSetNumeration), CommandFlags.Session)]
        public static void SheetSetNumeration()
        {
            new SSNumeration().Numeration();
        }

        [CommandMethod(Group, nameof(_InternalUse_SSBatchModal), CommandFlags.Modal)]
        public static void _InternalUse_SSBatchModal()
        {
            BatchVM.InternalBatchModal(AcadHelper.Doc);
        }

        [CommandMethod(Group, nameof(_InternalUse_SSBatchSession), CommandFlags.Session)]
        public static void _InternalUse_SSBatchSession()
        {
            BatchVM.InternalBatchSession(AcadHelper.Doc);
        }

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            _dllResolves ??= GetResolvers();
            var dllResolver = _dllResolves.FirstOrDefault(r => r.IsResolve(args.Name));
            return dllResolver?.LoadAssembly();
        }

        private static List<DllResolve> GetResolvers()
        {
            var dir = Path.GetDirectoryName(typeof(Commands).Assembly.Location);
            return DllResolve.GetDllResolve(dir, SearchOption.TopDirectoryOnly);
        }
    }
}