using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Acad_SheetSet.Data.Nodes;
using Acad_SheetSet.Numeration;
using ACSMCOMPONENTS20Lib;
using JetBrains.Annotations;
using static Acad_SheetSet.Data.SheetSetExt;

namespace Acad_SheetSet.Data
{
    public class SheetSet
    {
        private AcSmDatabase ssDb;

        public SheetSet([NotNull] AcSmDatabase ssDb)
        {
            this.ssDb = ssDb;
            var ss = ssDb.GetSheetSet();
            if (ss == null) throw new Exception("Пустая подшивка");
            Name = ss.GetName();
            File = ssDb.GetFileName();
        }

        public string Name { get; set; }
        public string File { get; set; }
        public ObservableCollection<ISSNode> Nodes { get; set; }

        public void Update()
        {
            var nodes = new ObservableCollection<ISSNode>();
            var ss = ssDb.GetSheetSet();
            foreach (var item in SsToList(ss.GetSheetEnumerator(), e => e.Next()))
            {
                nodes.Add(GetNode(item));
            }
            Nodes = nodes;
        }

        [CanBeNull]
        public static ISSNode GetNode([NotNull] IAcSmComponent item)
        {
            ISSNode node = null;
            var typeName = item.GetTypeName();
            switch (typeName)
            {
                case "AcSmSheet":
                    node = new SheetNode((AcSmSheet) item);
                    break;
                case "AcSmSubset":
                    node = new SubsetNode((AcSmSubset) item);
                    break;
            }
            return node;
        }
    }
}
