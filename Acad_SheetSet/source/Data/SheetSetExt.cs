namespace Acad_SheetSet.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Props;
#if v2017
    using ACSMCOMPONENTS21Lib;
#elif v2019
    using ACSMCOMPONENTS23Lib;
#endif

    public static class SheetSetExt
    {
        public static object GetCustomPropertyValue(this IAcSmComponent comp, string propName)
        {
            var prop = comp.GetProperty(propName);
            return prop?.GetValue();
        }

        public static AcSmCustomPropertyValue GetProperty(this IAcSmComponent comp, string propName)
        {
            var bag = comp.GetCustomPropertyBag();
            return bag.GetProperty(propName);
        }

        public static void SetCustomPropertyValue(this IAcSmComponent comp, string propName, object value)
        {
            var prop = comp.GetProperty(propName);
            if (prop == null)
            {
                throw new KeyNotFoundException($"Не найден параметр '{propName}'");
            }

            prop.SetValue(value);
        }

        public static IEnumerable<TItem> SsToList<TItem, TEnum>(TEnum enumerator, Func<TEnum, TItem> next)
        {
            while (true)
            {
                var item = next(enumerator);
                if (item == null)
                {
                    break;
                }

                yield return item;
            }
        }

        public static List<SSProp> GetCustomProperties(this IAcSmComponent comp, bool removeProps = false,
            SheetSet ss = null)
        {
            var cpb = comp.GetCustomPropertyBag();
            var props = new List<SSProp>();
            var cpbPropNames = new List<string>();
            var cpbList = SsToList(cpb.GetPropertyEnumerator(), e=>
            {
                e.Next(out var propName, out var item);
                cpbPropNames.Add(propName);
                return item;
            }).ToList();
            for (var i = 0; i < cpbList.Count; i++)
            {
                var cpbItem = cpbList[i];
                var propName = cpbPropNames[i];
                var flags = cpbItem.GetFlags();
                if (flags != PropertyFlags.CUSTOM_SHEETSET_PROP && flags != PropertyFlags.CUSTOM_SHEET_PROP)
                {
                    continue;
                }

                var value = cpbItem.GetValue()?.ToString();
                var prop = new SSProp
                {
                    Name = propName,
                    Value = value,
                    Type = GetPropType(flags)
                };
                props.Add(prop);
                if (removeProps)
                {
                    if (flags == PropertyFlags.CUSTOM_SHEET_PROP && ss != null)
                    {
                        // Удалить это свойство во всех листах
                        var sheets = ss.Nodes.SelectMany(s => s.GetSheets());
                        foreach (var sheet in sheets)
                        {
                            try
                            {
                                sheet.sheet.GetCustomPropertyBag().SetProperty(propName, null);
                            }
                            catch
                            {
                                //
                            }
                        }
                    }

                    cpb.SetProperty(propName,null);
                }
            }

            return props;
        }

        private static PropType GetPropType(PropertyFlags flags)
        {
            return flags == PropertyFlags.CUSTOM_SHEET_PROP ? PropType.Sheet : PropType.SheetSet;
        }

        private static PropertyFlags GetPropType(PropType flags)
        {
            return flags == PropType.SheetSet ? PropertyFlags.CUSTOM_SHEETSET_PROP : PropertyFlags.CUSTOM_SHEET_PROP;
        }

        public static void AddCustomProperty(this AcSmCustomPropertyBag bag, IAcSmComponent comp,
            SSProp prop, SheetSet ss = null)
        {
            var customProp = new AcSmCustomPropertyValue();
            customProp.InitNew(comp);
            customProp.SetFlags(GetPropType(prop.Type));
            customProp.SetValue(prop.Value);
            bag.SetProperty(prop.Name, customProp);
            if (prop.Type == PropType.Sheet && ss != null)
            {
                foreach (var sheet in ss.Nodes.SelectMany(s=>s.GetSheets()))
                {
                    var sheetBag = sheet.sheet.GetCustomPropertyBag();
                    sheetBag?.AddCustomProperty(sheet.sheet, prop);
                }
            }
        }
    }
}
