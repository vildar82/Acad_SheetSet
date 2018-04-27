// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 04 25 15:10

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AcadLib.Errors;
using Acad_SheetSet.Data.Nodes;
using Acad_SheetSet.Options;
using Acad_SheetSet.Props;
#if v2016
using ACSMCOMPONENTS20Lib;
#elif v2017
using ACSMCOMPONENTS21Lib;
#elif v2018
using ACSMCOMPONENTS22Lib;
#endif
using JetBrains.Annotations;
using NetLib.Monad;
using static Acad_SheetSet.Data.SheetSetExt;

namespace Acad_SheetSet.Data
{
    public class SheetSet
    {
        public readonly AcSmDatabase ssDb;
        public readonly SSOptions options;
        public AcSmSheetSet ss;

        public SheetSet([NotNull] AcSmDatabase ssDb, SSOptions options)
        {
            this.ssDb = ssDb;
            this.options = options;
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
        public ISSNode GetNode([NotNull] IAcSmComponent item)
        {
            ISSNode node = null;
            var typeName = item.GetTypeName();
            switch (typeName)
            {
                case "AcSmSheet":
                    node = new SheetNode((AcSmSheet) item, this);
                    break;
                case "AcSmSubset":
                    node = new SubsetNode((AcSmSubset) item, this);
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
                var propCrossNumber = ss.GetProperty(options.PropCrossNumberName);
                if (propCrossNumber == null)
                {
                    Inspector.AddError($"В подшивке нет свойства '{options.PropCrossNumberName}' для сквозного номера.");
                    setCrossNumber = false;
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
                Update();
                ss.GetCustomProperties(true, this);
                if (props?.Any() == true)
                {
                    var bag = ss.GetCustomPropertyBag();
                    foreach (var prop in props.AsEnumerable().Reverse())
                    {
                        bag.AddCustomProperty(ss, prop, this);
                    }
                }
                Update();
            }
        }
    }
}