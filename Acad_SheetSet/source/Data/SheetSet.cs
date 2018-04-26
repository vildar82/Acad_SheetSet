// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 04 25 15:10

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AcadLib.Errors;
using Acad_SheetSet.Data.Nodes;
using Acad_SheetSet.Props;
using ACSMCOMPONENTS20Lib;
using JetBrains.Annotations;
using NetLib.Monad;
using static Acad_SheetSet.Data.SheetSetExt;

namespace Acad_SheetSet.Data
{
    public class SheetSet
    {
        private readonly AcSmDatabase ssDb;
        private AcSmSheetSet ss;

        public SheetSet([NotNull] AcSmDatabase ssDb)
        {
            this.ssDb = ssDb;
            ss = ssDb.GetSheetSet();
            if (ss == null) throw new Exception("Пустая подшивка");
            Name = ss.GetName();
            File = ssDb.GetFileName();
        }

        public string File { get; set; }

        public string Name { get; set; }
        public ObservableCollection<ISSNode> Nodes { get; set; }
        public List<SSProp> Props { get; set; }

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

        public void Numeration(bool previewOnly)
        {
            using (new SSLock(ssDb))
            {
                Update();
                var crossNumber = 1;
                var setCrossNumber = true;
                if (!previewOnly)
                {
                    var propCrossNumber = ss.GetProperty(SheetNode.propCrossNumber);
                    if (propCrossNumber == null)
                    {
                        Inspector.AddError($"В подшивке нет свойства '{SheetNode.propCrossNumber}' для сквозного номера.");
                        setCrossNumber = false;
                    }
                }
                foreach (var node in Nodes)
                {
                    var number = 1;
                    var sheets = node.GetSheets();
                    foreach (var sheet in sheets)
                    {
                        sheet.NumberNew = number.ToString();
                        sheet.CrossNumberNew = crossNumber.ToString();
                        if (!previewOnly)
                        {
                            sheet.Try(s => s.SetNumber(), (s, e) =>
                                Inspector.AddError($"Ошибка записи номера '{s.Name}'={s.NumberNew} - {e.Message}"));
                            if (setCrossNumber)
                            {
                                sheet.Try(s => s.SetCrossNumber(), (s, e) =>
                                    Inspector.AddError(
                                        $"Ошибка записи сквозного номера '{s.Name}'={s.CrossNumberNew} - {e.Message}"));
                            }
                        }
                        number++;
                        crossNumber++;
                    }
                }
                if (!previewOnly) Update();
            }
        }

        private void Update()
        {
            var nodes = new ObservableCollection<ISSNode>();
            ss = ssDb.GetSheetSet();
            foreach (var item in SsToList(ss.GetSheetEnumerator(), e => e.Next())) nodes.Add(GetNode(item));
            Props = GetProps();
            Nodes = nodes;
        }

        [NotNull]
        private List<SSProp> GetProps()
        {
            return ss.GetCustomProperties();
        }

        public void CreateProps([CanBeNull] List<SSProp> props)
        {
            using (new SSLock(ssDb))
            {
                ss = ssDb.GetSheetSet();
                ss.GetCustomProperties(true);
                if (props?.Any() == true)
                {
                    var bag = ss.GetCustomPropertyBag();
                    foreach (var prop in props.AsEnumerable().Reverse())
                    {
                        bag.AddCustomProperty(ss, prop);
                    }
                }
                Update();
            }
        }
    }
}