namespace Acad_SheetSet.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Nodes;
    using Options;
    using Props;
    using Utils;
    using static SheetSetExt;
#if v2017
    using ACSMCOMPONENTS21Lib;
#elif v2019
    using ACSMCOMPONENTS23Lib;
#endif

    public class SheetSet
    {
        public readonly AcSmDatabase ssDb;
        public readonly SSOptions options;
        public AcSmSheetSet ss;

        public SheetSet(AcSmDatabase ssDb, SSOptions options)
        {
            this.ssDb = ssDb;
            this.options = options;
            ss = ssDb.GetSheetSet();
            if (ss == null)
            {
                throw new Exception("Пустая подшивка");
            }

            Name = ss.GetName();
            File = ssDb.GetFileName();
        }

        public string File { get; set; }

        public string Name { get; set; }

        public ObservableCollection<ISSNode> Nodes { get; set; }

        public List<SSProp> Props { get; set; }

        public ISSNode GetNode(IAcSmComponent item)
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
                            try
                            {
                                sheet.SetNumber();

                            }
                            catch (Exception ex)
                            {
                                Inspector.AddError(
                                    $"Ошибка записи номера '{sheet.Name}'={sheet.NumberNew} - {ex.Message}");
                            }

                            if (setCrossNumber)
                            {
                                try
                                {
                                    sheet.SetCrossNumber();
                                }
                                catch (Exception ex)
                                {
                                    Inspector.AddError(
                                        $"Ошибка записи сквозного номера '{sheet.Name}'={sheet.CrossNumberNew} - {ex.Message}");
                                }
                            }
                        }

                        number++;
                        crossNumber++;
                    }
                }

                if (!previewOnly)
                {
                    Update();
                }
            }
        }

        private void Update()
        {
            var nodes = new ObservableCollection<ISSNode>();
            ss = ssDb.GetSheetSet();
            foreach (var item in SsToList(ss.GetSheetEnumerator(), e => e.Next()))
            {
                nodes.Add(GetNode(item));
            }

            Props = GetProps();
            Nodes = nodes;
        }

        private List<SSProp> GetProps()
        {
            return ss.GetCustomProperties();
        }

        public void CreateProps(List<SSProp> props)
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
